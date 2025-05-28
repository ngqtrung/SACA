using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using SACA_Common.Controllers;
using SACA_Common.DTOs;
using SACA_Common.DTOs.Contest.Request;
using SACA_Common.DTOs.Contest.Response;
using SACA_Common.DTOs.Submission.Request;
using SACA_Common.Routes;
using SACA_Service.Services;

namespace SACA_API.Controllers
{
    [Route(ContestRoutes.INDEX)]
    [ApiController]
    public class ContestController : AuthorizeController
    {
        private IContestService _contestService;
        private ISubmissionService _submissionService;
        public ContestController
        (
            IContestService contestService,
            ISubmissionService submissionService,
            ILogger<IContestService> logger
        ) : base(logger)
        {
            _contestService = contestService;
            _submissionService = submissionService;
        }
        [HttpPost(ContestRoutes.ACTION.Create)]
        public async Task<IActionResult> Create([FromBody] ContestCreating form)
        {
            try
            {
                return Ok(new Response<CreateResult>(await _contestService.CreateAsync(UserHeader.user_id, form)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpPut(ContestRoutes.ACTION.Update)]
        public async Task<IActionResult> Update([FromBody] ContestUpdating form)
        {
            try
            {
                return Ok(new Response<bool>(await _contestService.UpdateAsync(UserHeader.user_id, form)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpPost(ContestRoutes.ACTION.FrozenContest)]
        public async Task<IActionResult> FrozenContest([FromBody] string contestId)
        {
            try
            {
                return Ok(new Response<bool>(await _contestService.FronzenContestAsync(contestId, UserHeader.user_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpDelete(ContestRoutes.ACTION.Delete)]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            try
            {
                return Ok(new Response<bool>(await _contestService.DeleteAsync(UserHeader.user_id, id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpDelete(ContestRoutes.ACTION.DeleteMany)]
        public async Task<IActionResult> DeleteMany([FromBody] DeleteManyRequest form)
        {
            try
            {
                return Ok(new Response<bool>(await _contestService.DeleteAsync(UserHeader.user_id, form)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpGet(ContestRoutes.ACTION.GetDetail)]
        public async Task<IActionResult> GetDetail([FromRoute] string id)
        {
            try
            {
                return Ok(new Response<ContestView>(await _contestService.GetDetailAsync(id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpGet(ContestRoutes.ACTION.Search)]
        public async Task<IActionResult> Search([FromQuery] ContestTableFilter request)
        {
            try
            {
                return Ok(await _contestService.SearchAsync(request, UserHeader.role == "Student" ? UserHeader.user_id : null));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpGet(ContestRoutes.ACTION.SubmitSolution)]
        public async Task<IActionResult> SubmitSolution([FromBody] SubmitSolutionRequest request)
        {
            try
            {
                return Ok(new Response<bool>(await _submissionService.SubmitSolutionAsync(request, UserHeader.user_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpGet(ContestRoutes.ACTION.GetUserContests)]
        public async Task<IActionResult> GetUserContests()
        {
            try
            {
                return Ok(new Response<List<string>>(await _contestService.GetUserContestsAsync(UserHeader.user_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpGet(ContestRoutes.ACTION.GetContestEndTime)]
        public async Task<IActionResult> GetContestEndTime([FromRoute] string id)
        {
            try
            {
                return Ok(new Response<DateTime>(await _contestService.GetContestEndTime(id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [Authorize(Roles = "Lecturer")]
        [HttpGet(ContestRoutes.ACTION.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await _contestService.GetAllAsync());
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpPost(ContestRoutes.ACTION.ImportExcel)]
        public async Task<IActionResult> ImportExcel([FromForm] IFormFile file)
        {
            try
            {
                return Ok(new Response<ContestCreating>(await _contestService.ImportExcel(file)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
