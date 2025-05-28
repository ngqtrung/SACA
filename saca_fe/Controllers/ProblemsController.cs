using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SACA_FE.Services;
using SACA_Common.DTOs.Problem.Response;
using SACA_Common.DTOs.Submission.Request;

namespace SACA_FE.Controllers
{
    [Route("Contest/{contestId}/Problem")]
    [Authorize(AuthenticationSchemes = "SACA")]
    public class ProblemsController : Controller
    {
        private readonly IProblemService _problemService;
        private readonly ISubmissionService _submissionService;
        private readonly IContestService _contestService;

        public ProblemsController(IProblemService problemService, ISubmissionService submissionService, IContestService contestService)
        {
            _problemService = problemService;
            _submissionService = submissionService;
            _contestService = contestService;
        }

        [HttpGet("{problemId}")]
        public async Task<IActionResult> Detail([FromRoute] string contestId, [FromRoute] string problemId)
        {
            try
            {
                var problem = await _problemService.GetDetailAsync(problemId);
                ViewBag.ProblemId = problemId;
                ViewBag.ContestId = contestId;
                var contest = await _contestService.GetDetailAsync(contestId);
                ViewBag.AvailLanguages = contest?.programming_languages ?? new List<SACA_Common.Enums.eType_ContestProgrammingLanguage>();
                ViewBag.ContestEndTime = contest?.end_at.ToString("o");
                return View(problem);
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Authen");
            }
        }
        [HttpPost("{problemId}")]
        public async Task<IActionResult> SubmitSolution(SubmitSolutionRequest form, [FromRoute] string problemId, [FromForm] string contestId)
        {
            try
            {
                await _submissionService.SubmitSolutionAsync(form);
                TempData["APIMessage"] = "Submitted Successfully.";
                return RedirectToAction("Detail", new { contestId, problemId });
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Detail", new { contestId, problemId });
            }
        }
    }
}
