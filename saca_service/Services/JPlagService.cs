using Microsoft.EntityFrameworkCore;
using SACA_Common.DTOs.JPlag;
using SACA_Common.Exceptions;
using SACA_Common.Models;
using SACA_Infra.Const;
using SACA_Infra.Context;
using SACA_Infra.Models;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SACA_Service.Services
{
    public interface IJPlagService
    {
        Task<bool> CheckPlagiarismAsync(string contestId, string problemId, int languageCode);
        Task<bool> CheckPlagiarismByContestIdAsync(string contestId, string userId);
    }
    public class JPlagService : IJPlagService
    {
        private readonly SACA_Context _context;

        public JPlagService(SACA_Context context)
        {
            _context = context;
        }
        public async Task<bool> CheckPlagiarismAsync(string contestId, string problemId, int languageCode)
        {
            var contest = await _context.contests.AsNoTracking()
                .Where(e => e.id == contestId)
                .FirstOrDefaultAsync();

            if (contest == null)
            {
                throw new NotFoundException(ErrorMessage.NotFound);
            }

            var problem = await _context.problems.AsNoTracking()
                .Where(e => e.contest_id == contestId && e.id == problemId)
                .FirstOrDefaultAsync();

            var bestSubmissions = await _context.best_submissions.AsNoTracking()
                .Include(e => e.problem_submission)
                .Where(e => e.problem.contest_id == contestId && e.problem.id == problemId && e.problem_submission.programming_language == languageCode)
                .ToListAsync();

            var accountIds = bestSubmissions.Select(e => e.problem_submission.account_id).ToList();
            var accountDicts = await _context.sys_accounts.AsNoTracking().Where(e => accountIds.Contains(e.id)).ToDictionaryAsync(e => e.id);

            var JPlagBestSubmissions = bestSubmissions
                .Where(e => accountDicts.ContainsKey(e.problem_submission.account_id))
                .Select(e => new JPlagBestSubmission
                {
                    src_code = e.problem_submission.source_code ?? "",
                    account_id = e.problem_submission.account_id,
                    username = accountDicts[e.problem_submission.account_id].username,
                    fullname = accountDicts[e.problem_submission.account_id].fullname
                })
                .ToList();

            if (JPlagBestSubmissions == null || JPlagBestSubmissions.Count < 2) return false;
            
            // Kill process JPlag on port 1997
            JPlagServiceExtension.KillProcessOnPort(1997);
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string workingDir = Path.Combine(baseDir, "jplag_work");
            string resultZip = Path.Combine(baseDir, "result.zip");

            // Dọn sạch thư mục làm việc
            if (Directory.Exists(workingDir))
                Directory.Delete(workingDir, true);
            Directory.CreateDirectory(workingDir);

            // Ghi code vào thư mục
            for (int i = 0; i < JPlagBestSubmissions.Count; i++)
            {
                string studentFolder = Path.Combine(workingDir, JPlagBestSubmissions[i].username + " - " + JPlagBestSubmissions[i].fullname);
                Directory.CreateDirectory(studentFolder);

                string fileExtension = JPlagServiceExtension.GetFileExtensionByLanguage(languageCode);
                string filePath = Path.Combine(studentFolder, $"main{fileExtension}");
                File.WriteAllText(filePath, JPlagBestSubmissions[i].src_code);
            }

            // Cấu hình chạy JPlag
            string jarPath = Path.Combine(baseDir, "Libraries", "jplag-6.0.0-jar-with-dependencies.jar");
            var psi = new ProcessStartInfo
            {
                FileName = "java",
                Arguments = $"-jar \"{jarPath}\" -l {JPlagServiceExtension.GetLanguageName(languageCode)} \"{workingDir}\" -r \"{resultZip}\" --mode=RUN_AND_VIEW --port={1997} --overwrite",
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(psi);
            return true;
        }

        public async Task<bool> CheckPlagiarismByContestIdAsync(string contestId, string userId)
        {
            var contest = await _context.contests.AsNoTracking()
                .FirstOrDefaultAsync(e => e.id == contestId)
                ?? throw new NotFoundException(ErrorMessage.NotFound);

            var problems = await _context.problems.AsNoTracking()
                .Where(e => e.contest_id == contestId)
                .ToListAsync();

            var bestSubmissions = await _context.best_submissions.AsNoTracking()
                .Include(e => e.problem_submission)
                .Include(e => e.problem)
                .Where(e => e.problem.contest_id == contestId)
                .ToListAsync();


            var bestSubmissionDict = await _context.best_submissions.AsNoTracking()
                .Include(e => e.problem_submission)
                .Where(e => e.problem.contest_id == contestId)
                .ToDictionaryAsync(e => e.id);

            var processedSubmissions = new HashSet<string>();

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string jarPath = Path.Combine(baseDir, "Libraries", "jplag-6.0.0-jar-with-dependencies.jar");
            string workingDir = Path.Combine(baseDir, "jplag_work");
            string reportDir = Path.Combine(baseDir, "jplag_report");
            string resultZip = Path.Combine(baseDir, "result.zip");

            foreach (var problem in problems)
            {
                var submissionsByProblem = bestSubmissions
                    .Where(e => e.problem.id == problem.id)
                    .ToList();

                if (submissionsByProblem.Count < 2) continue;

                var groupedByLang = submissionsByProblem
                    .GroupBy(e => e.problem_submission.programming_language);

                foreach (var langGroup in groupedByLang)
                {
                    var langSubmissions = langGroup.ToList();
                    if (langSubmissions.Count < 2) continue;

                    JPlagServiceExtension.KillProcessOnPort(1997);
                    PrepareDirectories(workingDir, reportDir, resultZip);

                    foreach (var submission in langSubmissions)
                    {
                        string studentFolder = Path.Combine(workingDir, submission.id);
                        Directory.CreateDirectory(studentFolder);

                        string ext = JPlagServiceExtension.GetFileExtensionByLanguage(langGroup.Key);
                        string filePath = Path.Combine(studentFolder, $"main{ext}");
                        File.WriteAllText(filePath, submission.problem_submission.source_code);
                    }

                    RunJPlag(jarPath, langGroup.Key, workingDir, resultZip);
                    JPlagServiceExtension.KillProcessOnPort(1997);

                    if (!File.Exists(resultZip)) continue;

                    ZipFile.ExtractToDirectory(resultZip, reportDir);

                    string overviewPath = Path.Combine(reportDir, "overview.json");
                    if (!File.Exists(overviewPath)) continue;

                    var json = await File.ReadAllTextAsync(overviewPath);
                    var overview = JsonSerializer.Deserialize<OverviewData>(json);
                    if (overview == null) continue;

                    foreach (var cmp in overview.TopComparisons)
                    {
                        UpdatePlagiarism(cmp.FirstSubmission, cmp.SecondSubmission, bestSubmissionDict, userId, processedSubmissions, cmp);
                        UpdatePlagiarism(cmp.SecondSubmission, cmp.FirstSubmission, bestSubmissionDict, userId, processedSubmissions, cmp);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private static void PrepareDirectories(string workingDir, string reportDir, string resultZip)
        {
            if (Directory.Exists(workingDir)) Directory.Delete(workingDir, true);
            if (Directory.Exists(reportDir)) Directory.Delete(reportDir, true);
            if (File.Exists(resultZip)) File.Delete(resultZip);
            Directory.CreateDirectory(workingDir);
            Directory.CreateDirectory(reportDir);
        }

        private static void RunJPlag(string jarPath, int language, string workingDir, string resultZip)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "java",
                Arguments = $"-jar \"{jarPath}\" -l {JPlagServiceExtension.GetLanguageName(language)} \"{workingDir}\" -r \"{resultZip}\" --mode=RUN --port=1997 --overwrite",
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            process?.WaitForExit();
        }

        private void UpdatePlagiarism(
            string sourceId,
            string targetId,
            Dictionary<string, best_submission> submissionDict,
            string userId,
            HashSet<string> processed,
            Comparison comparison)
        {
            if (processed.Contains(sourceId)) return;

            processed.Add(sourceId);
            var source = submissionDict[sourceId];
            var target = submissionDict[targetId];

            source.plagiarism_submission_id = target.problem_submission.id;
            source.plagiarism_avg = comparison.Similarities.Avg;
            source.plagiarism_max = comparison.Similarities.Max;
            source.Modified(userId);

            _context.best_submissions.Update(source);
        }

    }

    public static class JPlagServiceExtension
    {
        public static string GetFileExtensionByLanguage(int language) => language switch
        {
            43 => ".txt", // PlainText
            52 or 53 or 54 or 76 => ".cpp", // C++
            71 or 70 => ".py", // Python
            48 or 49 or 50 or 75 => ".c", // C
            63 => ".js", // JavaScript
            _ => ".txt"
        };


        public static string GetLanguageName(int language) => language switch
        {
            52 or 53 or 54 or 76 => "cpp", // C++
            71 or 70 => "python3", // Python
            48 or 49 or 50 or 75 => "c", // C
            63 => "javascript", // JavaScript
            _ => "text"
        };


        public static void KillProcessOnPort(int port)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c netstat -aon | findstr :{port}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                if (process == null) return;

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                // Không có tiến trình nào dùng port
                if (string.IsNullOrWhiteSpace(output))
                {
                    return;
                }

                // Lấy PID từ dòng output
                var match = Regex.Match(output, @$":{port}\s+.*?\s+.*?\s+(\d+)");
                if (match.Success && int.TryParse(match.Groups[1].Value, out int pid))
                {
                    try
                    {
                        var proc = Process.GetProcessById(pid);
                        proc.Kill();
                        proc.WaitForExit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Không thể kill PID {pid}: {ex.Message}");
                    }
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xử lý port {port}: {ex.Message}");
            }
        }
    }
}
