using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SACA_Common.Controllers;
using SACA_Common.DTOs;
using SACA_Service.Services;
using SACA_Common.DTOs.TestCase.Request;
using SACA_Common.DTOs.TestCase.Response;
using SACA_Common.Routes;

namespace SACA_API.Controllers
{
    [Route(TestCaseRoutes.INDEX)]
    [ApiController]
    public class TestCaseController : AuthorizeController
    {
        private ITestCaseService _testcaseService;
        public TestCaseController
        (
            ITestCaseService testcaseService,
            ILogger<ITestCaseService> logger
        ) : base(logger)
        {
            _testcaseService = testcaseService;
        }
        [HttpPost(TestCaseRoutes.ACTION.Create)]
        public async Task<IActionResult> Create([FromBody] TestCaseCreating form)
        {
            try
            {
                return Ok(new Response<CreateResult>(await _testcaseService.CreateAsync(UserHeader.user_id, form)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpPut(TestCaseRoutes.ACTION.Update)]
        public async Task<IActionResult> Update([FromBody] TestCaseUpdating form)
        {
            try
            {
                return Ok(new Response<bool>(await _testcaseService.UpdateAsync(UserHeader.user_id, form)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpDelete(TestCaseRoutes.ACTION.Delete)]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            try
            {
                return Ok(new Response<bool>(await _testcaseService.DeleteAsync(UserHeader.user_id, id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpDelete(TestCaseRoutes.ACTION.DeleteMany)]
        public async Task<IActionResult> DeleteMany([FromBody] DeleteManyRequest form)
        {
            try
            {
                return Ok(new Response<bool>(await _testcaseService.DeleteAsync(UserHeader.user_id, form)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpGet(TestCaseRoutes.ACTION.GetDetail)]
        public async Task<IActionResult> GetDetail([FromRoute] string id)
        {
            try
            {
                return Ok(new Response<TestCaseView>(await _testcaseService.GetDetailAsync(id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpGet(TestCaseRoutes.ACTION.Search)]
        public async Task<IActionResult> Search([FromQuery] TestCaseTableFilter request)
        {
            try
            {
                return Ok(await _testcaseService.SearchAsync(request));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpPost(TestCaseRoutes.ACTION.ImportExcel)]
        public async Task<IActionResult> ImportExcel([FromForm] IFormFile file)
        {
            try
            {
                return Ok(new Response<List<TestCaseCreating>>(await _testcaseService.ImportExcel(file)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
