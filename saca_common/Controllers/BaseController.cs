using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SACA_Common.Authentications;
using SACA_Common.DTOs;
using SACA_Common.Exceptions;
using System.Security.Claims;

namespace SACA_Common.Controllers
{
    [ApiController]
    public class BaseController : Controller
    {
        protected ILogger _logger;
        protected ActionResult HandleException(Exception ex)
        {
            _logger.LogError(HttpContext.Request.Path + ": " + ex.Message + "\nInnerExeption:" + ex.InnerException + "\nStackTrace: " + ex.StackTrace);
            if (ex is ForbiddenException)
            {
                return Forbid(ex.Message);
            }
            if (ex is NotFoundException)
            {
                return NotFound(((NotFoundException)ex).Message);
            }
            if (ex is BadException)
            {
                return BadRequest(((BadException)ex).Message);
            }
            return Problem(detail: ex.Message, statusCode: 500);
        }
        public BaseController(ILogger logger)
        {
            _logger = logger;
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthorizeController : BaseController
    {
        protected UserHeader UserHeader
        {
            get
            {
                try
                {
                    return new UserHeader
                    {
                        username = User.FindFirst(CustomJwtClaimType.Username)?.Value ?? "",
                        user_id = User.FindFirst(CustomJwtClaimType.UserId)?.Value ?? "",
                        role = User.FindFirst(ClaimTypes.Role)?.Value ?? "",
                        fullname = User.FindFirst(CustomJwtClaimType.Fullname)?.Value ?? "",
                        email = User.FindFirst(ClaimTypes.Email)?.Value ?? ""
                    };
                }
                catch
                {
                    return new UserHeader();
                }
            }
        }
        public AuthorizeController(ILogger logger) : base(logger)
        {

        }
    }
}