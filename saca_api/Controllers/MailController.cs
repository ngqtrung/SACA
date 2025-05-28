using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SACA_Common.Controllers;
using SACA_Common.DTOs;
using SACA_Service.Services;
using SACA_Common.DTOs.TestCase.Request;
using SACA_Common.DTOs.TestCase.Response;
using SACA_Common.Routes;
using SACA_Infra.Models;
using SACA_Common.DTOs.File.Response;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using SACA_Common.DTOs.Mail.Request;

namespace SACA_API.Controllers
{
    [Route(MailRoutes.INDEX)]
    [Authorize(Roles = "Lecturer")]
    public class MailController : AuthorizeController
    {
        private IMailService _mailService;
        public MailController
        (
            IMailService mailService,
            ILogger<MailController> logger
        ) : base(logger)
        {
            _mailService = mailService;
        }
        [HttpPost(MailRoutes.ACTION.SendMaiInvite)]
        public async Task<IActionResult> SendMaiInvite([FromBody] SendMailInvite request)
        {
            try
            {
                return Ok(new Response<bool>(await _mailService.SendMailInvite(request)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
