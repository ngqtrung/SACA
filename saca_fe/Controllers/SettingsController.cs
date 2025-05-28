using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SACA_Common.DTOs.SysSetting.Request;
using SACA_FE.Services;

namespace SACA_FE.Controllers
{
    [Authorize(AuthenticationSchemes = "SACA", Roles = "Lecturer")]
    [Route("Settings")]
    public class SettingsController : Controller
    {
        private readonly ISettingService _settingService;

        public SettingsController(ISettingService settingService)
        {
            _settingService = settingService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var settings = await _settingService.GetAllAsync();
                return View(settings);
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Authen");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Update([FromForm] Dictionary<string, string> form)
        {
            try
            {
                // Tạo danh sách các setting cần cập nhật
                var settingsToUpdate = form.Select(x => new SysSettingUpdate
                {
                    key = x.Key,
                    value = x.Value
                }).ToList();

                // Gọi service để cập nhật từng setting
                await _settingService.UpdateAsync(settingsToUpdate);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Authen");
            }
        }
    }
}
