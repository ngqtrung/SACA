using Microsoft.AspNetCore.Mvc;
using SACA_Common.Controllers;
using SACA_Common.DTOs;
using SACA_Common.DTOs.Problem.Response;
using SACA_Common.DTOs.Problem.Request;
using SACA_Service.Services;
using SACA_Common.Routes;

namespace SACA_API.Controllers
{
    [Route(ProblemRoutes.INDEX)]
    [ApiController]
    public class ProblemController : AuthorizeController
    {
        private IProblemService _problemService;
        public ProblemController
        (
            IProblemService problemService,
            ILogger<IProblemService> logger
        ) : base(logger)
        {
            _problemService = problemService;
        }
        [HttpPost(ProblemRoutes.ACTION.Create)]
        public async Task<IActionResult> Create([FromBody] ProblemCreating form)
        {
            try
            {
                return Ok(new Response<CreateResult>(await _problemService.CreateAsync(form, UserHeader.user_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpPut(ProblemRoutes.ACTION.Update)]
        public async Task<IActionResult> Update([FromBody] ProblemUpdating form)
        {
            try
            {
                return Ok(new Response<bool>(await _problemService.UpdateAsync(form, UserHeader.user_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpDelete(ProblemRoutes.ACTION.Delete)]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            try
            {
                return Ok(new Response<bool>(await _problemService.DeleteAsync(UserHeader.user_id, id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpDelete(ProblemRoutes.ACTION.DeleteMany)]
        public async Task<IActionResult> DeleteMany([FromBody] DeleteManyRequest form)
        {
            try
            {
                return Ok(new Response<bool>(await _problemService.DeleteAsync(form, UserHeader.user_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpGet(ProblemRoutes.ACTION.GetDetail)]
        public async Task<IActionResult> GetDetail([FromRoute] string id)
        {
            try
            {
                return Ok(new Response<ProblemView>(await _problemService.GetDetailAsync(id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpGet(ProblemRoutes.ACTION.Search)]
        public async Task<IActionResult> Search([FromQuery] ProblemTableFilter request)
        {
            try
            {
                return Ok(await _problemService.SearchAsync(request));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpPost(ProblemRoutes.ACTION.ImportExcel)]
        public async Task<IActionResult> ImportExcel([FromForm] IFormFile file)
        {
            try
            {
                return Ok(new Response<List<ProblemCreating>>( await _problemService.ImportExcel(file)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
