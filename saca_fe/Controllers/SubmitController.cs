using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SACA_Common.DTOs.Submission.Request;
using SACA_FE.Services;
using System.Net.WebSockets;

namespace SACA_FE.Controllers
{
    [Authorize(AuthenticationSchemes = "SACA")]
    [Route("Contests/{ContestId}/Submit")]
    public class SubmitController : Controller
    {
        private readonly IProblemService _problemService;
        private readonly ISubmissionService _submissionService;
        private readonly IContestService _contestService;
        public SubmitController
        (
            IProblemService problemService, 
            ISubmissionService submissionService,
            IContestService contestService
        )
        {
            _problemService = problemService;
            _submissionService = submissionService;
            _contestService = contestService;
        }
        public async Task<IActionResult> Index(string ContestId)
        {
            var problems = await _problemService.Search(new SACA_Common.DTOs.Problem.Request.ProblemTableFilter
            {
                contest_id = ContestId
            });
            ViewData["problems"] = problems.Items;
            ViewBag.ContestId = ContestId;
            var contest = await _contestService.GetDetailAsync(ContestId);
            ViewBag.AvailLanguages = contest?.programming_languages ?? new List<SACA_Common.Enums.eType_ContestProgrammingLanguage>() ;
            ViewBag.ContestEndTime = contest?.end_at.ToString("o");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SubmitCode(SubmitSolutionRequest form, [FromRoute] string ContestId)
        {
            try
            {
                await _submissionService.SubmitSolutionAsync(form);
                return RedirectToAction("Index", "SubmissionHistory", new { ContestId = ContestId });
            }
            catch(Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Submit", new { ContestId = ContestId });
            }
        }
    }
}
