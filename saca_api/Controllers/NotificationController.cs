using Microsoft.AspNetCore.Mvc;
using SACA_Common.Controllers;
using SACA_Common.DTOs.Notification.Request;
using SACA_Common.Routes;
using SACA_Common.DTOs.Notification.Response;
using SACA_Service.Services;
using System.Threading.Tasks;
using SACA_Common.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace SACA_API.Controllers
{
    [Route(NotificationRoutes.INDEX)]
    [ApiController]
    public class NotificationController : AuthorizeController
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService, ILogger<INotificationService> logger) : base(logger)
        {
            _notificationService = notificationService;
        }
        [HttpGet(NotificationRoutes.ACTION.GetAll)]
        public async Task<ActionResult<NotificationResponse>> GetNotifications()
        {
            try
            {
                var response = await _notificationService.NotificateAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpGet(NotificationRoutes.ACTION.GetById)]
        public async Task<ActionResult<NotificationResponse>> Detail(string id)
        {
            try
            {
                var response = await _notificationService.GetNotificationByIdAsync(id);
                return response;
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpPost(NotificationRoutes.ACTION.Create)]
        public async Task<IActionResult> CreateNotification(NotificationCreating model)
        {
            if (model == null)
            {
                return BadRequest("Invalid notification data.");
            }

            var success = await _notificationService.AddNotificationAsync(model);
            if (!success)
            {
                return BadRequest("Failed to create notification.");
            }

            return CreatedAtAction(nameof(GetNotifications), new { }, "Notification created successfully.");
        }

        [HttpPut(NotificationRoutes.ACTION.Update)]
        public async Task<IActionResult> UpdateNotification(int id, [FromBody] NotificationCreating model)
        {
            if (model == null)
            {
                return BadRequest("Invalid notification data.");
            }

            var success = await _notificationService.UpdateNotificationAsync(id, model);
            if (!success)
            {
                return NotFound("Notification not found or update failed.");
            }

            return Ok("Notification updated successfully.");
        }

        [HttpDelete(NotificationRoutes.ACTION.Delete)]
        public async Task<IActionResult> DeleteNotification(string id)
        {
            var success = await _notificationService.DeleteNotificationAsync(id);
            if (!success)
            {
                return NotFound("Notification not found.");
            }

            return Ok("Notification deleted successfully.");
        }
        [HttpPost(NotificationRoutes.ACTION.SendNotificationToContest)]
        public async Task<IActionResult> SendNotificationToContest([FromBody] SendNotificationToContestRequest request)
        {
            try
            {
                return Ok(new Response<bool>(await _notificationService.SendNotificationToContest(request.contest_id, request.notification_id, UserHeader.user_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpPost(NotificationRoutes.ACTION.MarkAsRead)]
        public async Task<IActionResult> MarkAsRead([FromRoute] string notification_id)
        {
            try
            {
                return Ok(new Response<bool>(await _notificationService.MarkAsReadAsync(notification_id, UserHeader.user_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpDelete(NotificationRoutes.ACTION.DeleteAccountNotification)]
        public async Task<IActionResult> DeleteAccountNotification([FromRoute] string id)
        {
            try
            {
                return Ok(new Response<bool>(await _notificationService.DeleteAccountNotificationAsync(id, UserHeader.user_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpGet(NotificationRoutes.ACTION.GetAccountNotification)]
        public async Task<IActionResult> GetAccountNotification()
        {
            try
            {
                return Ok(new List<AccountNotificationModel>(await _notificationService.GetAccountNotificationAsync(UserHeader.user_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
