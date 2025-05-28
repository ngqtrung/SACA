using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SACA_Common.Controllers;
using SACA_Common.DTOs;
using SACA_Common.DTOs.SysSetting.Request;
using SACA_Common.DTOs.SysSetting.Response;
using SACA_Common.Exceptions;
using SACA_Common.Routes;
using SACA_Service.Services;

namespace SACA_API.Controllers
{
    [Route(SysSettingRoutes.INDEX)]
    [ApiController]
    public class SysSettingController : AuthorizeController
    {
        private readonly ISysSettingService _sysSettingService;

        public SysSettingController(ISysSettingService sysSettingService, ILogger<SysSettingController> logger)
            : base(logger)
        {
            _sysSettingService = sysSettingService ?? throw new ArgumentNullException(nameof(sysSettingService));
            _logger = logger;
        }

        [HttpGet(SysSettingRoutes.Action.GetAll)]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(new Response<List<SysSettingView>>(_sysSettingService.GetAll()));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut(SysSettingRoutes.Action.Update)]
        public IActionResult Update([FromBody] List<SysSettingUpdate> form)
        {
            try
            {
                return Ok(new Response<bool>(_sysSettingService.Update(UserHeader.user_id, form)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
