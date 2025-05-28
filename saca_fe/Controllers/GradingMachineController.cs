using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using Org.BouncyCastle.Asn1.Ocsp;
using SACA_Common.DTOs;
using SACA_Common.DTOs.Contest.Request;
using SACA_Common.DTOs.Notification.Request;
using SACA_Common.DTOs.Problem.Request;
using SACA_Common.DTOs.Problem.Response;
using SACA_FE.Services;

namespace SACA_FE.Controllers
{
    [Authorize(AuthenticationSchemes = "SACA", Roles = "Lecturer")]
    public class GradingMachineController : Controller
    {
        private readonly IGradingMachineService _gradingMachineService;

        public GradingMachineController
        (
            IGradingMachineService gradingMachineService
        )
        {
            _gradingMachineService = gradingMachineService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var info = await _gradingMachineService.GetGradingMachineInfo();
            return View(info);
        }
    }
}
