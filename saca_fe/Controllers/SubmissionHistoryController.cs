using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using SACA_Common.DTOs;
using SACA_Common.DTOs.Submission.Request;
using SACA_Common.DTOs.Submission.Response;
using SACA_FE.Services;

namespace SACA_FE.Controllers
{
    [Authorize(AuthenticationSchemes = "SACA")]
    [Route("Contests/{ContestId}/SubmissionHistory")]
    public class SubmissionHistoryController : Controller
    {
        private readonly ISubmissionService _submissionService;
        private readonly IContestService _contestService;
        public SubmissionHistoryController
        (
             ISubmissionService submissionService,
             IContestService contestService
        )
        {
            _submissionService = submissionService;
            _contestService = contestService;
        }

        public async Task<IActionResult> Index([FromRoute] string ContestId, [FromQuery] SubmissionTableFilter request)
        {
            try
            {

                ViewBag.ContestId = ContestId;
                request.contest_id = ContestId;
                var endAt = await _contestService.GetContestEndTime(ContestId);
                ViewBag.ContestEndTime = endAt.ToString("o");
                var pageResponse = await _submissionService.SearchAsync(request);
                return View(pageResponse);
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return View(new PagedResponse<SubmissionTableView>()
                {

                });
            }
        }
        [HttpGet("Detail")]
        public async Task<IActionResult> Detail([FromRoute] string ContestId, [FromQuery] string id)
        {
            try
            {
                ViewBag.ContestId = ContestId;
                var view = await _submissionService.GetDetail(id);
                var endAt = await _contestService.GetContestEndTime(ContestId);
                ViewBag.ContestEndTime = endAt.ToString("o");
                return View(view);
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index", new
                {
                    ContestId = ContestId,
                    request = new SubmissionTableFilter
                    {
                        contest_id = ContestId
                    }
                });
            }
        }
        [HttpPost("Resubmit")]
        public async Task<IActionResult> ResubmitSolution(string contestId, string submissionIds)
        {
            var ids = submissionIds?.Split(',').ToList() ?? new List<string>();
            await _submissionService.ResubmitSolutionAsync(new ResubmitSolutionRequest
            {
                contestId = contestId,
                submissionIds = ids
            });
            TempData["APIMessage"] = "Successfully.";
            return RedirectToAction("Index", "SubmissionHistory", new { ContestId = contestId });
        }
    }
}
