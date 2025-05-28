using Microsoft.AspNetCore.Mvc;
using SACA_Common.Controllers;
using SACA_Common.DTOs;
using SACA_Common.DTOs.Contest.Request;
using SACA_Common.DTOs.Contest.Response;
using SACA_Common.DTOs.LeaderBoard.Request;
using SACA_Common.DTOs.LeaderBoard.Response;
using SACA_Common.DTOs.Report.ScoreBoard.Request;
using SACA_Common.DTOs.Report.ScoreDistribution.Request;
using SACA_Common.DTOs.Submission.Request;
using SACA_Common.Routes;
using SACA_Common.Utils;
using SACA_Service.Services;
using TaxNet_Report.Report.Sale;

namespace SACA_API.Controllers
{
    [Route(ReportRoute.INDEX)]
    [ApiController]
    public class ReportController : AuthorizeController
    {
        private readonly IReportService _reportService;
        private readonly IContestService _contestService;
        private readonly IProblemService _problemService;
        public ReportController
        (
            IReportService reportService,
            IContestService contestService,
            IProblemService problemService,
            ILogger<IReportService> logger
        ) : base(logger)
        {
            _reportService = reportService;
            _contestService = contestService;
            _problemService = problemService;
        }
        [HttpPost(ReportRoute.ACTION.Contest)]
        public async Task<IActionResult> ExportContest([FromBody] ExportContestRequest form)
        {
            try
            {
                var zipData = await _reportService.ExportContestsAsZipAsync(form);
                return File(zipData, "application/zip", "SACA_Contests.zip");
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpPost(ReportRoute.ACTION.ContestParticipants)]
        public async Task<IActionResult> ExportContestParticipants([FromBody] string contest_id)
        {
            try
            {
                var contest = await _contestService.GetDetailAsync(contest_id);
                var fileBytes = await _reportService.ExportContestParticipants(contest_id);
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{contest.code}_participants");
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpGet(ReportRoute.ACTION.ScoreBoard)]
        public async Task<IActionResult> ScoreBoard([FromQuery] GetScoreBoardRequest request)
        {
            try
            {
                return Ok(await _reportService.GetScoreBoardAsync(request));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpPost(ReportRoute.ACTION.ContestProblems)]
        public async Task<IActionResult> ExportContestProblems([FromBody] string contest_id)
        {
            try
            {
                var contest = await _contestService.GetDetailAsync(contest_id);
                var fileBytes = await _reportService.ExportContestProblems(contest_id);
                return File(fileBytes, "application/zip", $"{contest.code}_problems");
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpPost(ReportRoute.ACTION.ExportScoreBoard)]
        public async Task<IActionResult> ExportScoreBoard([FromBody] ReportFilter_BangDiem_ScoreBoard filter)
        {
            try
            {
                var result = await _reportService.ExportScoreBoardAsync(filter);
                return new FileContentResult(result.filebytes, Path.GetExtension(result.filename).GetContextType())
                {
                    FileDownloadName = result.filename
                };
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpGet(ReportRoute.ACTION.ScoreDistribution)]
        public async Task<IActionResult> GetScoreDistribution([FromQuery] GetScoreDistributionRequest request)
        {
            try
            {
                return Ok(await _reportService.GetScoreDistributionAsync(request));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpPost(ReportRoute.ACTION.TestCases)]
        public async Task<IActionResult> ExportTestCase([FromBody] string problemId)
        {
            try
            {
                var problem = await _problemService.GetDetailAsync(problemId);
                var fileBytes = await _reportService.ExportTestCasesAsync(problemId);
                return File(fileBytes, "application/zip", $"{problem.code}_TestCases");
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
