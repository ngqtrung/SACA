using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SACA_Common.APIRoutes;
using SACA_Common.Controllers;
using SACA_Common.Routes;
using SACA_Service.Services;

namespace SACA_API.Controllers
{
    [Route(JPlagRoutes.INDEX)]
    [ApiController]
    [Authorize(Roles = "Lecturer")]
    public class JPlagController : AuthorizeController
    {
        public readonly IJPlagService _jPlagService;

        public JPlagController(IJPlagService jPlagService, ILogger<JPlagController> logger) : base(logger)
        {
            _jPlagService = jPlagService;
        }

        [HttpGet(JPlagRoutes.Action.CheckPlagiarism)]
        public async Task<IActionResult> CheckPlagiarism([FromQuery] string contestId, [FromQuery] string problemId, [FromQuery] int languageCode)
        {
            try
            {
                return Ok(await _jPlagService.CheckPlagiarismAsync(contestId, problemId, languageCode));
            }
            catch (Exception ex)
            {
                return HandleException(ex);

            }
        }

        [HttpGet(JPlagRoutes.Action.CheckPlagiarismByContestId)]
        public async Task<IActionResult> CheckPlagiarismByContestId([FromQuery] string contestId)
        {
            try
            {
                return Ok(await _jPlagService.CheckPlagiarismByContestIdAsync(contestId, UserHeader.user_id));
            }
            catch (Exception ex)
            {
                return HandleException(ex);

            }
        }
    }
}
