using AutoMapper;
using FlexCel.Report;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SACA_Common.DTOs;
using SACA_Common.DTOs.Account.Response;
using SACA_Common.DTOs.Contest;
using SACA_Common.DTOs.Contest.Request;
using SACA_Common.DTOs.Contest.Response;
using SACA_Common.DTOs.LeaderBoard.Request;
using SACA_Common.DTOs.Problem;
using SACA_Common.DTOs.Report.Contest;
using SACA_Common.DTOs.Report.ScoreBoard.Request;
using SACA_Common.DTOs.Report.ScoreBoard.Response;
using SACA_Common.DTOs.Report.ScoreDistribution.Request;
using SACA_Common.DTOs.Report.ScoreDistribution.Response;
using SACA_Common.Enums;
using SACA_Common.Exceptions;
using SACA_Infra.Const;
using SACA_Infra.Context;
using SACA_Infra.Utils;
using SACA_Service.DTO.Report;
using System.IO.Compression;
using TaxNet_Report.Report.Sale;
using ZstdSharp.Unsafe;

namespace SACA_Service.Services
{
    public interface IReportService
    {
        Task<byte[]> ExportContestsAsZipAsync(ExportContestRequest form);
        byte[] ExportContestParticipants(ContestView contest, FileStream templateStream);
        Task<byte[]> ExportContestParticipants(string contestId);
        Task<byte[]> ExportContestProblems(string contestId);
        Task<ReportResult> ExportScoreBoardAsync(ReportFilter_BangDiem_ScoreBoard filter);
        Task<PagedResponse<ScoreBoardResponse>> GetScoreBoardAsync(GetScoreBoardRequest form);
        Task<List<ScoreDistributionDetailView>> GetScoreDistributionAsync(GetScoreDistributionRequest form);
    }
    public class ReportService : IReportService
    {
        private readonly SACA_Context _context;
        private readonly ILeaderBoardService _leaderBoardService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public ReportService(SACA_Context context, ILeaderBoardService leaderBoardService, IMapper mapper, IWebHostEnvironment env)
        {
            _context = context;
            _leaderBoardService = leaderBoardService;
            _mapper = mapper;
            _env = env;
        }
        public async Task<byte[]> ExportContestsAsZipAsync(ExportContestRequest form)
        {
            using var zipStream = new MemoryStream();
            var contests = await _context.contests.AsNoTracking()
                .Include(e => e.problems)
                    .ThenInclude(e => e.file)
                .Include(e => e.problems)
                    .ThenInclude(e => e.test_cases)
                .Include(e => e.contest_participants)
                    .ThenInclude(e => e.account)
                .Where(e => form.contest_ids.Contains(e.id))
                .Select(e => _mapper.Map<ContestView>(e))
                .ToListAsync();


            // Tạo template contest member file stream
            string contestMemberTemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "ContestParticipantTemplate.xlsx");
            var templateStream = new FileStream(contestMemberTemplatePath, FileMode.Open, FileAccess.Read);
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                foreach (var contestId in form.contest_ids)
                {
                    var contest = contests.FirstOrDefault(e => e.id == contestId);
                    if (contest != null)
                    {

                        var contestFolder = contest.code;

                        //Add contest_info.json
                        var contestInfo = new ContestInfo
                        {
                            code = contest.code,
                            title = contest.title,
                            description = contest.description,
                            start_at = contest.start_at,
                            end_at = contest.end_at,
                            duration = contest.duration,
                            subject_code = contest.subject_code,
                            contest_type_name = EnumHelper.GetDescription((eType_Contest)contest.contest_type),
                            contest_type = contest.contest_type,
                            grading_type_name = EnumHelper.GetDescription((eType_Grading)contest.grading_type),
                            grading_type = contest.grading_type,
                            programming_language_names = contest.programming_languages.Select(p => EnumHelper.GetDescription(p)).ToList(),
                            programming_languages = contest.programming_languages.Select(p => (int)p).ToList(),
                            leaderboard_enabled = contest.leaderboard_enabled,
                            penalty_time = contest.penalty_time,
                            plagiarism_detection_enabled = contest.plagiarism_detection_enabled,
                            status_name = EnumHelper.GetDescription((eStatus_Contest)contest.status),
                            status = contest.status
                        };
                        var contestInfoEntry = archive.CreateEntry($"{contestFolder}/contest_info.json");
                        using (var writer = new StreamWriter(contestInfoEntry.Open()))
                        {
                            await writer.WriteAsync(JsonConvert.SerializeObject(contestInfo, Formatting.Indented));
                        }

                        // Add members.xlsx
                        var membersData = ExportContestParticipants(contest, templateStream);
                        var membersEntry = archive.CreateEntry($"{contestFolder}/members.xlsx");
                        using (var membersStream = membersEntry.Open())
                        {
                            await membersStream.WriteAsync(membersData, 0, membersData.Length);
                        }

                        // Add leader_board.xlsx
                        var leaderboardData = await _leaderBoardService.ExportLeaderBoard(new LeaderBoardTableFilter { contest_id = contestId });
                        var leaderboardEntry = archive.CreateEntry($"{contestFolder}/leader_board.xlsx");
                        using (var leaderboardStream = leaderboardEntry.Open())
                        {
                            await leaderboardStream.WriteAsync(leaderboardData, 0, leaderboardData.Length);
                        }

                        // Add Submissions
                        //foreach (var submission in contest.submissions)
                        //{
                        //    var userFolder = $"{contestFolder}/Submissions/{submission.fullname}_{submission.username}";
                        //    foreach (var problem in submission.problems)
                        //    {
                        //        var gradingDetailEntry = archive.CreateEntry($"{userFolder}/{problem.code}/grading_detail.txt");
                        //        using (var writer = new StreamWriter(gradingDetailEntry.Open()))
                        //        {
                        //            await writer.WriteAsync($"Score: {problem.score}\nFeedback: {problem.feedback}");
                        //        }
                        //    }
                        //}

                        // Add Problems
                        foreach (var problem in contest.problems)
                        {
                            var problemFolder = $"{contestFolder}/Problems/{problem.code}";

                            // Add problem_info.json
                            var problemInfo = _mapper.Map<ProblemInfo>(problem);
                            var problemInfoEntry = archive.CreateEntry($"{problemFolder}/problem_info.json");
                            using (var writer = new StreamWriter(problemInfoEntry.Open()))
                            {
                                await writer.WriteAsync(JsonConvert.SerializeObject(problemInfo, Formatting.Indented));
                            }

                            // Add Testcases
                            foreach (var testCase in problem.test_cases)
                            {
                                var testCaseFolder = $"{problemFolder}/Testcases/{testCase.code}";
                                var inputEntry = archive.CreateEntry($"{testCaseFolder}/{testCase.code}.inp");
                                using (var writer = new StreamWriter(inputEntry.Open()))
                                {
                                    await writer.WriteAsync(testCase.input);
                                }
                                var outputEntry = archive.CreateEntry($"{testCaseFolder}/{testCase.code}.out");
                                using (var writer = new StreamWriter(outputEntry.Open()))
                                {
                                    await writer.WriteAsync(testCase.output);
                                }
                            }
                        }
                    }

                }
            }
            await templateStream.DisposeAsync();
            zipStream.Position = 0; // Reset stream position
            return zipStream.ToArray();
        }
        public byte[] ExportContestParticipants(ContestView contest, FileStream templateStream)
        {

            var report = new FlexCelReport();
            report.AddTable("participants", contest.participants);

            using (var outputStream = new MemoryStream())
            {
                templateStream.Position = 0;
                report.Run(templateStream, outputStream);
                return outputStream.ToArray();
            }
        }
        public async Task<byte[]> ExportContestParticipants(string contestId)
        {
            var members = await _context.contest_participants.AsNoTracking()
                .Where(e => e.contest_id == contestId)
                .Include(e => e.account)
                .Select(e => _mapper.Map<AccountView>(e.account))
                .ToListAsync();
            var report = new FlexCelReport();
            report.AddTable("participants", members);


            string contestMemberTemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "ContestParticipantTemplate.xlsx");
            using var templateStream = new FileStream(contestMemberTemplatePath, FileMode.Open, FileAccess.Read);
            using (var outputStream = new MemoryStream())
            {
                templateStream.Position = 0;
                report.Run(templateStream, outputStream);
                return outputStream.ToArray();
            }
        }

        public async Task<PagedResponse<ScoreBoardResponse>> GetScoreBoardAsync(GetScoreBoardRequest filter)
        {
            var scoreBoardQuery = await ReportServiceExtension.GetScoreBoardQuery(filter, _context);
            return new PagedResponse<ScoreBoardResponse>
            {
                page_index = filter.page_index,
                page_size = filter.page_size,
                total_items = await scoreBoardQuery.CountAsync(),
                Items = await scoreBoardQuery.Paged(filter.page_index, filter.page_size).ToListAsync()
            };
        }

        public async Task<byte[]> ExportContestProblems(string contestId)
        {
            using var zipStream = new MemoryStream();

            var contest = await _context.contests.AsNoTracking()
                .Include(c => c.problems)
                    .ThenInclude(p => p.test_cases)
                .FirstOrDefaultAsync(c => c.id == contestId);

            if (contest == null)
            {
                throw new NotFoundException(ErrorMessage.NotFound);
            }

            // Tạo file zip
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                foreach (var problem in contest.problems)
                {
                    var problemFolder = $"Problems/{problem.code}";

                    // Thêm file problem_info.json
                    var problemInfo = _mapper.Map<ProblemInfo>(problem);
                    var problemInfoEntry = archive.CreateEntry($"{problemFolder}/problem_info.json");
                    using (var writer = new StreamWriter(problemInfoEntry.Open()))
                    {
                        await writer.WriteAsync(JsonConvert.SerializeObject(problemInfo, Formatting.Indented));
                    }

                    // Thêm test cases
                    foreach (var testCase in problem.test_cases)
                    {
                        var testCaseFolder = $"{problemFolder}/Testcases/{testCase.code}";
                        var inputEntry = archive.CreateEntry($"{testCaseFolder}/{testCase.code}.in");
                        using (var writer = new StreamWriter(inputEntry.Open()))
                        {
                            await writer.WriteAsync(testCase.input);
                        }
                        var outputEntry = archive.CreateEntry($"{testCaseFolder}/{testCase.code}.out");
                        using (var writer = new StreamWriter(outputEntry.Open()))
                        {
                            await writer.WriteAsync(testCase.output);
                        }
                    }
                }
            }

            zipStream.Position = 0; // Reset vị trí stream
            return zipStream.ToArray();
        }

        public async Task<ReportResult> ExportScoreBoardAsync(ReportFilter_BangDiem_ScoreBoard filter)
        {
            var scoreBoardFilter = new GetScoreBoardRequest
            {
                class_code = filter.class_code,
                contest_id = filter.contest_id,
                keyword = filter.keyword,
                subject_code = filter.subject_code,
            };
            var scoreBoardQuery = await ReportServiceExtension.GetScoreBoardQuery(scoreBoardFilter, _context);
            var scoreBoard = await scoreBoardQuery.ToListAsync();
            var template = Path.Combine(_env.ContentRootPath, "Templates");
            var reportDetails = scoreBoard.Select((e, index) => new BangDiem_ScoreBoard
            {
                stt = ++index,
                email = e.email,
                fullname = e.fullname,
                roll_number = e.roll_number,
                score = e.score
            }).ToList();
            var report = new ScoreBoard<ReportFilter_BangDiem_ScoreBoard>(template, reportDetails);
            var bytes = report.ExportReport(filter, filter.Extension);
            return new ReportResult
            {
                filebytes = bytes,
                filename = $"ScoreBoard.{filter.Extension}"
            };
        }

        public async Task<List<ScoreDistributionDetailView>> GetScoreDistributionAsync(GetScoreDistributionRequest form)
        {
            if (!form.problem_ids.Any())
            {
                form.problem_ids = await _context.problems.AsNoTracking()
                    .Where(e => e.contest_id == form.contest_id)
                    .Select(e => e.id)
                    .ToListAsync();
            }
            var scoreBoard = await _context.best_submissions.AsNoTracking()
               .Include(e => e.sys_account)
               .Where(e => form.problem_ids.Contains(e.problem_id))
               .GroupBy(e => new { e.sys_account.fullname, e.sys_account.email, e.sys_account.roll_number })
               .Select(g => new ScoreBoardResponse
               {
                   email = g.Key.email,
                   fullname = g.Key.fullname,
                   score = g.Sum(x => x.score),
                   roll_number = g.Key.roll_number
               })
               .OrderByDescending(e => e.score)
               .ToListAsync();
            var result = scoreBoard.GroupBy(e => e.score)
                .Select(g => new ScoreDistributionDetailView
                {
                    count = g.Count(),
                    score = g.Key,
                    participants = g.Select(x => new ScoreDistributionParticipant
                    {
                        email = x.email,
                        full_name = x.fullname,
                        roll_number = x.roll_number
                    }).ToList()
                })
                .ToList();
            return result;
        }
    }
    public static class ReportServiceExtension
    {
        public static async Task<IQueryable<ScoreBoardResponse>> GetScoreBoardQuery(GetScoreBoardRequest filter, SACA_Context _context)
        {
            var problemIds = await _context.contests.AsNoTracking()
                .Where(e => filter.contest_id == null || filter.contest_id == e.id)
                .Where(e => filter.class_code == null || filter.class_code == e.class_code)
                .Where(e => filter.subject_code == null || filter.subject_code == e.subject_code)
                .SelectMany(e => e.problems.Select(x => x.id))
                .ToListAsync();
            var scoreBoardQuery = _context.best_submissions.AsNoTracking()
               .Include(e => e.sys_account)
               .Where(e => problemIds.Contains(e.problem_id))
               .GroupBy(e => new { e.sys_account.fullname, e.sys_account.email, e.sys_account.roll_number })
               .Select(g => new ScoreBoardResponse
               {
                   email = g.Key.email,
                   fullname = g.Key.fullname,
                   score = g.Sum(x => x.score),
                   roll_number = g.Key.roll_number
               })
               .OrderByDescending(e => e.score)
               .Where(e => filter.keyword == null ||
                      e.email.ToLower().Trim().Contains(filter.keyword.ToLower().Trim()) ||
                      e.roll_number != null && e.roll_number.ToLower().Trim().Contains(filter.keyword.ToLower().Trim()) ||
                      e.fullname.ToLower().Trim().Contains(filter.keyword.ToLower().Trim()));
            return scoreBoardQuery;
        }
    }
}
