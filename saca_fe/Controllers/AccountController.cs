using Microsoft.AspNetCore.Mvc;
using SACA_Common.DTOs.Account.Request;
using SACA_Common.DTOs.Authenticate.Request;
using SACA_FE.Services;

namespace SACA_FE.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        [BindProperty]
        public ProfileViewModel ProfileData { get; set; } = new();
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            try
            {
                var accountDetail = await _accountService.GetDetailAsync(id);

                var viewModel = new ProfileViewModel
                {
                    Account = accountDetail,
                    ChangePassword = new ChangePasswordRequest()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Contests");
            }
        }
    }
}
