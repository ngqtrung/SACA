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
    public class NotifiController : Controller
    {
        private readonly INotifiService _notificationService;
        private readonly IContestService _contestService;
        private readonly IProblemService _problemService;

        public NotifiController(INotifiService notificationService, IContestService contestService, IProblemService problemService)
        {
            _notificationService = notificationService;
            _contestService = contestService;
            _problemService = problemService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var notificationResponse = await _notificationService.GetAllAsync();
            return View(notificationResponse);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var notifications = await _notificationService.GetAccountNotification();
            return Json(notifications);
        }
        [HttpGet]
        public async Task<IActionResult> Create(string? contest_id, string? title, string? description)
        {
            var contests = await _contestService.SearchAsync(new ContestTableFilter());

            var problems = contest_id != null
                ? (await _problemService.Search(new ProblemTableFilter { contest_id = contest_id })).Items
                : null;

            ViewBag.Contests = contests.Items;
            ViewBag.Problems = problems;
            ViewBag.SelectedContest = contest_id;

            var model = new NotificationCreating
            {
                title = title ?? "",
                description = description ?? ""
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(NotificationCreating notification)
        {
            try
            {
                await _notificationService.CreateAsync(notification);
                TempData["APIMessage"] = "Notification created Successfully.";
                return RedirectToAction("Index", "Notifi");
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Notifi");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _notificationService.DeleteByIdAsync(id);
                TempData["APIMessage"] = "Notification deleted Successfully.";
                return RedirectToAction("Index", "Notifi");
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Notifi");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            try
            {
                var result = await _notificationService.GetById(id);
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Notifi");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Send(string id, string contestId)
        {
            try
            {
                await _notificationService.SendAsync(new SendNotificationToContestRequest{
                    notification_id = id,
                    contest_id = contestId
                });
                TempData["APIMessage"] = "Send Notification Successfully.";
                return RedirectToAction("Index", "Notifi");
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Notifi");
            }
        }
    }
}
