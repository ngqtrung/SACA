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
using SACA_Common.DTOs.GradingMachine.Response;

namespace SACA_API.Controllers
{
    [Route(GradingMachineRoutes.INDEX)]
    [Authorize(Roles = "Lecturer")]
    public class GradingMachineController : AuthorizeController
    {
        private IJudge0Service _judge0Service;
        public GradingMachineController
        (
            IJudge0Service judge0Service,
            ILogger<GradingMachineController> logger
        ) : base(logger)
        {
            _judge0Service = judge0Service;
        }
        [HttpGet(GradingMachineRoutes.ACTION.GetInfo)]
        public async Task<IActionResult> GetInfo()
        {
            try
            {
                return Ok(new Response<GradingMachineInfo>(await _judge0Service.GetGradingMachineInfo()));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
