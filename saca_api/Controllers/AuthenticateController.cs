using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SACA_Common.Controllers;
using SACA_Common.DTOs;
using SACA_Common.DTOs.Authenticate.Request;
using SACA_Common.DTOs.Authenticate.Response;
using SACA_Common.Routes;
using SACA_Service.Services;

namespace SACA_API.Controllers
{
    [Route(AuthenticateRoutes.INDEX)]
    [ApiController]
    public class AuthenticateController : AuthorizeController
    {
        private IAuthenticateService _authenService;
        public AuthenticateController
        (
            IAuthenticateService authenService,
            ILogger<IAuthenticateService> logger
        ) : base(logger)
        {
            _authenService = authenService;
        }
        [AllowAnonymous]
        [HttpPost(AuthenticateRoutes.ACTION.Login)]
        public async Task<IActionResult> Login(LoginRequest form)
        {
            try
            {
                return Ok(new Response<LoginResponse>(await _authenService.AuthencateAsync(form)));
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpPost(AuthenticateRoutes.ACTION.ChangePassword)]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest form)
        {
            try
            {
                return Ok(new Response<bool>(await _authenService.ChangePasswordAsync(form, UserHeader.user_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
