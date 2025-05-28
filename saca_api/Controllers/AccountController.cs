using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SACA_Common.Controllers;
using SACA_Common.DTOs;
using SACA_Common.DTOs.Account.Request;
using SACA_Common.DTOs.Account.Response;
using SACA_Common.DTOs.Authenticate.Request;
using SACA_Common.Routes;
using SACA_Service.Services;

namespace SACA_API.Controllers
{
    [Route(AccountRoutes.INDEX)]
    [ApiController]
    public class AccountController : AuthorizeController
    {
        private IAccountService _accountService;


        public AccountController
        (
            IAccountService accountService,
            ILogger<IAccountService> logger
        ) : base(logger)
        {
            _accountService = accountService;
        }

        [HttpGet(AccountRoutes.INDEX)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(new Response<List<AccountView>>(await _accountService.GetAllAsync()));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet(AccountRoutes.ACTION.GetDetail)]
        public async Task<IActionResult> GetDetail([FromRoute] string id)
        {
            try
            {
                return Ok(new Response<AccountView>(await _accountService.GetDetailAsync(id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }


        [HttpPost(AccountRoutes.ACTION.Create)]
        public async Task<IActionResult> Create([FromBody] AccountCreating form)
        {
            try
            {
                return Ok(new Response<CreateResult>(await _accountService.CreateAsync(UserHeader.user_id, form)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpPut(AccountRoutes.ACTION.Update)]
        public async Task<IActionResult> Update([FromBody] AccountUpdating form)
        {
            try
            {
                return Ok(new Response<bool>(await _accountService.UpdateAsync(UserHeader.user_id, form)));
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
                return Ok(new Response<bool>(await _accountService.DeleteAsync(UserHeader.user_id, id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost(AccountRoutes.ACTION.AddMany)]
        public async Task<IActionResult> AddMany([FromBody] AccountAddMany form)
        {
            try
            {
                return Ok(new Response<AddManyResponse>(await _accountService.AddManyAsync(UserHeader.user_id, form)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpGet(AccountRoutes.ACTION.GetListMembers)]
        public async Task<IActionResult> GetListMembers([FromQuery] MemberTableFilter filter)
        {
            try
            {
                return Ok(await _accountService.SearchContestMemberAsync(filter));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpPost(AccountRoutes.ACTION.ResetPassword)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest form, [FromRoute] string id)
        {
            try
            {
                return Ok(new Response<bool>(await _accountService.ResetPasswordAsync(form, id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpGet(AccountRoutes.ACTION.Search)]
        public async Task<IActionResult> Search([FromQuery] AccountTableFilter filter)
        {
            try
            {
                return Ok(await _accountService.SearchAsync(filter));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpPost(AccountRoutes.ACTION.ImportExcel)]
        public async Task<IActionResult> ImportExcel([FromForm] IFormFile file)
        {
            try
            {
                return Ok(new Response<List<AccountCreating>>(await _accountService.ImportExcel(file)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
