using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SACA_Common.DTOs;
using SACA_Common.DTOs.Account.Request;
using SACA_Common.DTOs.Account.Response;
using SACA_Common.DTOs.Contest.Request;
using SACA_Common.DTOs.Problem.Request;
using SACA_Common.Routes;
using SACA_FE.Services;

namespace SACA_FE.Controllers
{
    [Authorize(AuthenticationSchemes = "SACA")]
    [Route("Contests")]
    public class ContestsController : Controller
    {
        private readonly IContestService _contestService;
        private readonly IAccountService _accountService;
        private readonly IProblemService _problemService;

        public ContestsController(IContestService contestService, IAccountService accountService, IProblemService problemService)
        {
            _contestService = contestService;
            _accountService = accountService;
            _problemService = problemService;
        }
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ContestTableFilter request)
        {
            try
            {
                var contests = await _contestService.SearchAsync(request);
                return View(contests);
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Authen");
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Detail([FromRoute] string id, [FromQuery] ProblemTableFilter filter)
        {
            try
            {
                filter.contest_id = id;
                var problems = await _problemService.Search(filter);
                var contest = await _contestService.GetDetailAsync(id);
                ViewBag.ContestId = id; 
                if (contest?.end_at != null)
                {
                    ViewBag.ContestEndTime = contest.end_at.ToString("o"); // Sử dụng định dạng ISO 8601
                }

                return View(problems);
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Authen");
            }
        }
        [HttpGet("GetUserContests")]
        public async Task<IActionResult> GetUserContests()
        {
            try
            {
                var contests = await _contestService.GetUserContestsAsync();
                if (contests == null) return NotFound(new { message = "No contests found." });

                return Json(contests);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetUserContests: {ex.Message}");
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }
        [HttpGet("{id}/Members")]
        public async Task<IActionResult> Members([FromRoute] string id, [FromQuery] MemberTableFilter request)
        {
            try
            {
                request.contest_id = id;
                ViewBag.ContestId = id;
                var endAt = await _contestService.GetContestEndTime(id);
                ViewBag.ContestEndTime = endAt.ToString("o"); ;
                var memberList = await _accountService.GetListMembersAsync(request);
                return View(memberList);
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
