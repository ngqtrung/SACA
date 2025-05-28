using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SACA_Common.Controllers;
using SACA_Common.DTOs;
using SACA_Service.Services;
using SACA_Common.DTOs.Submission.Request;
using SACA_Common.DTOs.Submission.Response;
using SACA_Common.Routes;
using Microsoft.AspNetCore.Authorization;

namespace SACA_API.Controllers
{
    [Route(SubmissionRoutes.INDEX)]
    [ApiController]
    public class SubmissionController : AuthorizeController
    {
        private ISubmissionService _submissionService;
        public SubmissionController
        (
            ISubmissionService SubmissionService,
            ILogger<ISubmissionService> logger
        ) : base(logger)
        {
            _submissionService = SubmissionService;
        }
        [HttpPost(SubmissionRoutes.ACTION.SubmitSolution)]
        public async Task<IActionResult> SubmitSolution([FromBody] SubmitSolutionRequest form)
        {
            try
            {
                return Ok(new Response<bool>(await _submissionService.SubmitSolutionAsync(form, UserHeader.user_id, UserHeader.role == "Lecturer")));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpPost(SubmissionRoutes.ACTION.ResubmitSolution)]
        public async Task<IActionResult> ResubmitSolution([FromBody] ResubmitSolutionRequest form)
        {
            try
            {
                return Ok(new Response<bool>(await _submissionService.ResubmitAsync(form, UserHeader.user_id, UserHeader.role == "Lecturer")));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpDelete(SubmissionRoutes.ACTION.Delete)]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            try
            {
                return Ok(new Response<bool>(await _submissionService.DeleteAsync(UserHeader.user_id, id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpDelete(SubmissionRoutes.ACTION.DeleteMany)]
        public async Task<IActionResult> DeleteMany([FromBody] DeleteManyRequest form)
        {
            try
            {
                return Ok(new Response<bool>(await _submissionService.DeleteAsync(UserHeader.user_id, form)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpGet(SubmissionRoutes.ACTION.GetDetail)]
        public async Task<IActionResult> GetDetail([FromRoute] string id)
        {
            try
            {
                return Ok(new Response<SubmissionView>(await _submissionService.GetDetailAsync(id, UserHeader.role == "Student" ? UserHeader.user_id : null)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpGet(SubmissionRoutes.ACTION.Search)]
        public async Task<IActionResult> Search([FromQuery] SubmissionTableFilter request)
        {
            try
            {
                return Ok(await _submissionService.SearchAsync(request, UserHeader.role == "Student" ? UserHeader.user_id : null));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [Authorize(Roles ="Lecturer")]
        [HttpGet(SubmissionRoutes.ACTION.GetGroupPlagiarism)]
        public async Task<IActionResult> GetGroupPlagiarism(string submission_id)
        {
            try
            {
                return Ok(await _submissionService.GetGroupPlagiarismSubmission(submission_id));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
