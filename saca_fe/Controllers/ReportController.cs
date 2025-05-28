using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SACA_Common.DTOs.LeaderBoard.Request;
using SACA_Common.DTOs.Report.ScoreBoard.Request;
using SACA_FE.Services;

namespace SACA_FE.Controllers
{
    [Authorize(AuthenticationSchemes = "SACA")]
    [Route("Report")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IContestService _contestService;
        public ReportController
        (
            IReportService reportService,
            IContestService contestService
        )
        {
            _reportService = reportService;
            _contestService = contestService;
        }

        public async Task<IActionResult> Index([FromQuery] GetScoreBoardRequest filter)
        {
            try
            {
                var contests = await _contestService.GetAllContestAsync();
                var subjectCodes = contests.Where(e => !string.IsNullOrEmpty(e.subject_code)).Select(x => x.subject_code).ToList();
                var classCodes = contests.Where(e => !string.IsNullOrEmpty(e.class_code)).Select(x => x.class_code).ToList();
                var scoreBoard = await _reportService.GetScoreBoard(filter);
                ViewBag.Contests = contests;
                ViewBag.SubjectCodes = subjectCodes;
                ViewBag.ClassCodes = classCodes;
                return View(scoreBoard);
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "ContestManagement");
            }
        }
    }
}
