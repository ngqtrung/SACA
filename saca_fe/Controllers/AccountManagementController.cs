using Microsoft.AspNetCore.Mvc;
using SACA_Common.DTOs.Account.Request;
using SACA_FE.Services;

namespace SACA_FE.Controllers
{
    [Route("AccountManagement")]
    public class AccountManagementController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountManagementController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] AccountTableFilter request)
        {
            var accounts = await _accountService.SearchAsync(request);
            return View(accounts);
        }
        [HttpGet("Detail/{id}")]
        public async Task<IActionResult> Detail([FromRoute] string id)
        {
            var account = await _accountService.GetDetailAsync(id);
            return View(account.Result);
        }
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit([FromRoute] string id)
        {
            var account = await _accountService.GetDetailAsync(id);
            return View(account.Result);
        }
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(AccountUpdating form)
        {
            try
            {
                await _accountService.UpdateAsync(form);
                TempData["APIMessage"] = "Account Updated Successfully.";
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
            }
            var account = await _accountService.GetDetailAsync(form.id!);
            return View(account.Result);
        }
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ResetPasswordRequest request, [FromForm] string id)
        {
            try
            {
                await _accountService.ResetPasswordAsync(request, id);
                TempData["APIMessage"] = "Password Updated Successfully.";
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
            }
            var account = (await _accountService.GetDetailAsync(id)).Result;
            return View("Edit", account);
        }
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            try
            {
                await _accountService.DeleteAsync(id);
                TempData["APIMessage"] = "Account Deleted Successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create(AccountCreating form)
        {
            try
            {
                await _accountService.CreateAsync(form);
                TempData["APIMessage"] = "Account Created Successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return View();
            }
        }
    }
}
