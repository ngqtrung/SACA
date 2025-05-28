using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SACA_Common.DTOs.JPlag;
using SACA_Common.DTOs.LeaderBoard.Request;
using SACA_FE.Services;

namespace SACA_FE.Controllers
{
    [Authorize(AuthenticationSchemes = "SACA")]
    [Route("Contests/{ContestId}/Ranking")]
    public class RankingController : Controller
    {
        private readonly ILeadboardService _leaderBoardService;
        private readonly IContestService _contestService;
        private readonly IPlagiarismService _plagiarismService;
        public RankingController
        (
            ILeadboardService leaderBoardService
            , IContestService contestService,
            IPlagiarismService plagiarismService
        )
        {
            _leaderBoardService = leaderBoardService;
            _contestService = contestService;
            _plagiarismService = plagiarismService;
        }

        public async Task<IActionResult> Index([FromRoute] string ContestId, [FromQuery] LeaderBoardTableFilter filter)
        {
            try
            {
                filter.contest_id = ContestId;
                var leaderBoard = await _leaderBoardService.Search(filter);
                ViewBag.ContestId = ContestId;
                var contest = await _contestService.GetDetailAsync(ContestId);
                ViewBag.AvailLanguages = contest?.programming_languages ?? new List<SACA_Common.Enums.eType_ContestProgrammingLanguage>();
                ViewBag.Problems = contest?.problems;
                ViewBag.ContestEndTime = contest?.end_at.ToString("o");
                return View(leaderBoard);
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "MySubmission", new { ContestId = ContestId });
            }
        }
        [HttpPost]
        public async Task<IActionResult> CheckPlagiarism(CheckPlagiarismRequest request, [FromRoute] string ContestId)
        {
            try
            {
                request.contest_id = ContestId;
                ViewBag.ContestId = ContestId;
                var contest = await _contestService.GetDetailAsync(ContestId);
                ViewBag.AvailLanguages = contest?.programming_languages ?? new List<SACA_Common.Enums.eType_ContestProgrammingLanguage>();
                ViewBag.Problems = contest?.problems;
                ViewBag.ContestEndTime = contest?.end_at.ToString("o");
                if(request.problem_id == null || request.programing_language == null)
                    await _plagiarismService.CheckPlagiarismByContestId(request.contest_id);
                else await _plagiarismService.CheckPlagiarism(request.contest_id, request.problem_id, request.programing_language ?? 0);
                TempData["APIMessage"] = "Successfully.";
                return RedirectToAction("Index", "Ranking", new { ContestId = ContestId });
            }
            catch(Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Ranking", new { ContestId = ContestId });
            }
        }
    }
}
