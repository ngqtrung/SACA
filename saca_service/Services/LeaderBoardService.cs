using FlexCel.Core;
using FlexCel.Report;
using FlexCel.XlsAdapter;
using Microsoft.EntityFrameworkCore;
using SACA_Common.DTOs.LeaderBoard.Request;
using SACA_Common.DTOs.LeaderBoard.Response;
using SACA_Common.Enums;
using SACA_Common.Exceptions;
using SACA_Infra.Const;
using SACA_Infra.Context;
using SACA_Infra.Utils;
using System.Dynamic;

namespace SACA_Service.Services
{
    public interface ILeaderBoardService
    {
        Task<LeaderBoardTableView> Search(LeaderBoardTableFilter filter, bool getAll = false, bool isLecture = false);
        Task<byte[]> ExportLeaderBoard(LeaderBoardTableFilter filter);
    }
    public class LeaderBoardService : ILeaderBoardService
    {
        private readonly SACA_Context _context;
        private readonly IContestService _contestService;
        public LeaderBoardService
        (
            SACA_Context context,
            IContestService contestService
        )
        {
            _context = context;
            _contestService = contestService;
        }


        public async Task<byte[]> ExportLeaderBoard(LeaderBoardTableFilter filter)
        {
            var contest = await _contestService.GetDetailAsync(filter.contest_id);
            var leaderBoard = await Search(filter, true);

            // Tạo file Excel
            var xls = new XlsFile();

            xls.NewFile(1, TExcelFileFormat.v2019);
            xls.SheetName = "LeaderBoard";
            var headerFormat = xls.GetDefaultFormat;
            headerFormat.Font.Size20 = 20 * 20;
            headerFormat.Font.Style = TFlxFontStyles.Bold;
            var headerFormatIndex = xls.AddFormat(headerFormat);

            var normalFormat = xls.GetDefaultFormat;
            normalFormat.Font.Size20 = 13 * 20;
            var normalFormatIndex = xls.AddFormat(normalFormat);

            var tableHeaderFormat = xls.GetDefaultFormat;
            tableHeaderFormat.Font.Size20 = 13 * 20;
            tableHeaderFormat.Font.Style = TFlxFontStyles.Bold;
            tableHeaderFormat.HAlignment = THFlxAlignment.center;
            tableHeaderFormat.VAlignment = TVFlxAlignment.center;
            var tableHeaderFormatIndex = xls.AddFormat(tableHeaderFormat);
            int row = 1;
            int col = 1;


            // Phần thông tin cuộc thi
            xls.SetCellValue(row, col, $"Mã cuộc thi: {contest.code}");
            xls.SetCellFormat(row, col, headerFormatIndex);
            row++;

            xls.SetCellValue(row, col, $"Tiêu đề cuộc thi: {contest.title}");
            xls.SetCellFormat(row, col, headerFormatIndex);
            row += 2;

            xls.SetCellValue(row, col, $"Mô tả: {contest.description ?? "N/A"}");
            xls.SetCellFormat(row, col, normalFormatIndex);
            row++;

            xls.SetCellValue(row, col, $"Thời gian bắt đầu: {contest.start_at.ToString("dd/MM/yyyy HH:mm:ss")}");
            xls.SetCellFormat(row, col, normalFormatIndex);
            xls.SetCellValue(row, col + 3, $"Thời hạn: {contest.duration} (phút)");
            xls.SetCellFormat(row, col + 3, normalFormatIndex);
            row++;

            xls.SetCellValue(row, col, $"Thời gian kết thúc: {contest.end_at.ToString("dd/MM/yyyy HH:mm:ss")}");
            xls.SetCellFormat(row, col, normalFormatIndex);
            row++;

            xls.SetCellValue(row, col, $"Mã môn học: {contest.subject_code ?? "N/A"}");
            xls.SetCellFormat(row, col, normalFormatIndex);
            row++;

            xls.SetCellValue(row, col, $"Loại cuộc thi: {EnumHelper.GetDescription((eType_Contest)contest.contest_type)}");
            xls.SetCellFormat(row, col, normalFormatIndex);
            row++;

            xls.SetCellValue(row, col, $"Cách chấm điểm: {EnumHelper.GetDescription((eType_Grading)contest.grading_type)}");
            xls.SetCellFormat(row, col, normalFormatIndex);
            row++;

            var contestProgrammingLanguages = new List<string>();
            foreach (var programmingLanguage in contest.programming_languages)
            {
                contestProgrammingLanguages.Add(EnumHelper.GetDescription(programmingLanguage));
            }
            xls.SetCellValue(row, col, $"Ngôn ngữ lập trình: {string.Join(",", contestProgrammingLanguages)}");
            xls.SetCellFormat(row, col, normalFormatIndex);
            row += 2;

            // Phần tiêu đề bảng xếp hạng
            xls.SetCellValue(row, col, "Bảng xếp hạng");
            xls.SetCellFormat(row, col, headerFormatIndex);
            row += 2;

            // Header bảng xếp hạng
            int startCol = col;
            xls.SetCellValue(row, startCol, "STT");
            xls.SetCellFormat(row, startCol++, tableHeaderFormatIndex);
            xls.SetCellValue(row, startCol, "Account");
            xls.SetCellFormat(row, startCol++, tableHeaderFormatIndex);

            var problemColumns = leaderBoard.problems.Select(p => new
            {
                ColumnName = $"Problem_{p.id}",
                ColumnDisplayName = p.code
            }).ToList();

            foreach (var problem in problemColumns)
            {
                xls.SetCellValue(row, startCol++, problem.ColumnDisplayName);
            }

            xls.SetCellValue(row, startCol, "Total Score");
            row++;

            // Dữ liệu bảng xếp hạng
            foreach (var (leaderRow, index) in leaderBoard.rows.Select((r, i) => (r, i + 1)))
            {
                col = 1;
                xls.SetCellValue(row, col++, index); // STT
                xls.SetCellValue(row, col++, $"{leaderRow.fullname}\n{leaderRow.username}"); // Account

                // Dữ liệu các cột động
                foreach (var problem in problemColumns)
                {
                    var score = leaderRow.details.FirstOrDefault(d => d.problem_id == problem.ColumnName.Replace("Problem_", ""))?.score ?? 0;
                    xls.SetCellValue(row, col++, score);
                }

                xls.SetCellValue(row, col, leaderRow.total_score); // Total Score
                row++;
            }

            // Định dạng bảng và tự động căn chỉnh
            xls.AutofitCol(2, col, false, 1.1);

            // Xuất file
            using (var memoryStream = new MemoryStream())
            {
                xls.Save(memoryStream);
                //var filePath = Path.Combine(Path.GetTempPath(), $"{contest.code}_LeaderBoard_test.xlsx");
                //xls.Save(filePath);//có thể lưu trực tiếp bằng excel
                return memoryStream.ToArray();
            }
        }

        public async Task<LeaderBoardTableView> Search(LeaderBoardTableFilter filter, bool getAll = false, bool isLecture = false)
        {
            var contest = await _context.contests.Include(e => e.problems)
                           .FirstOrDefaultAsync(e => e.id == filter.contest_id);
            if (contest == null)
            {
                throw new NotFoundException(ErrorMessage.NotFound);
            }
            if(!contest.leaderboard_enabled && !isLecture)
            {
                return new LeaderBoardTableView();
            }
            var leaderBoard = new LeaderBoardTableView
            {
                contest_id = filter.contest_id,
                is_fronzen = contest.is_frozen,
                page_index = filter.page_index,
                page_size = filter.page_size,
                problems = contest.problems
                .OrderByDescending(e => e.created_on)
                .Select(e => new LeaderBoardTableView_Problem
                {
                    id = e.id,
                    code = e.code,
                    score = e.score
                }).ToList()
            };
            var problemIds = leaderBoard.problems.Select(e => e.id).ToList();
            var submissionsQuery = _context.best_submissions
                                  .Include(e => e.sys_account)
                                  .Include(e => e.problem)
                                  .Where(e => problemIds.Contains(e.problem_id))
                                  .AsEnumerable()
                                  .GroupBy(e => new { e.user_id })
                                  .Select(g => new LeaderBoardTableView_Row
                                  {
                                      user_id = g.Key.user_id,
                                      fullname = g.First().sys_account.fullname,
                                      username = g.First().sys_account.username,
                                      total_score = g.Sum(e => isLecture ? e.score : e.frozen_score),
                                      total_penaty = 0,
                                      details = g.Select(e => new LeaderBoardTableView_Row_ProblemDetail
                                      {
                                          problem_id = e.problem_id,
                                          complete_time = e.submited_at - contest.start_at,
                                          score = isLecture ? e.score : e.frozen_score,
                                          frozen_score = e.frozen_score,
                                          number_of_attempts = e.number_of_attempts,
                                          submission_id = e.problem_submission_id,
                                          plagiarism_avg = e.plagiarism_avg,
                                          plagiarism_max = e.plagiarism_max,
                                          plagiarism_submission_id = e.plagiarism_submission_id
                                      }).ToList()
                                  })
                                  .OrderByDescending(e => e.total_score);
            if (!getAll)
            {
                leaderBoard.rows = submissionsQuery
                    .Skip((filter.page_index - 1) * filter.page_size)
                    .Take(filter.page_size)
                    .ToList();
            }
            else
            {
                leaderBoard.rows = submissionsQuery.ToList();
            }
            return leaderBoard;
        }

    }
}
