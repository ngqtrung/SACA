using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SACA_Common.DTOs;
using SACA_Common.DTOs.Account.Request;
using SACA_Common.DTOs.Authenticate.Request;
using SACA_FE.Services;
using SACA_FE.Utils;

namespace SACA_FE.Controllers
{
    public class AuthenController : Controller
    {
        private readonly IAuthenService _authenService;
        private readonly IHttpContextAccessor _contextAccessor;
        public AuthenController
        (
            IAuthenService authenService,
            IHttpContextAccessor contextAccessor)
        {
            _authenService = authenService;
            _contextAccessor = contextAccessor;
        }
        public IActionResult Index()
        {
            var userInfo = TokenUtils.GetUserInfo(_contextAccessor.HttpContext!);
            if(userInfo != null)
            {
                if(userInfo.role == "Student")
                {
                    return RedirectToAction("Index", "Contests");
                }
                if (userInfo.role == "Lecturer")
                {
                    return RedirectToAction("Index", "ContestManagement");
                }

            }
            Response.Cookies.Delete("AuthToken");
            ViewData["APIErrorMessage"] = TempData["APIErrorMessage"];
            return View();
        }
        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {

                var response = await _authenService.Login(request); 
                Response.Cookies.Append("AuthToken", response.token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    Expires = DateTime.UtcNow.AddDays(1)
                });
                await Task.Delay(1000);
                if(response.role == "Lecturer") return RedirectToAction("Index", "ContestManagement");
                else
                {
                    return RedirectToAction("Index", "Contests");
                }
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordRequest model)
        {
            try
            {
                await _authenService.ChangePasswordAsync(model);
                return Ok(new { result = true, message = "Password changed successfully." });
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Contests");
            }
        }
    }
}
