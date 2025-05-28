using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SACA_Common.DTOs.Notification.Request;
using SACA_Common.Exceptions;
using SACA_Infra.Const;
using SACA_Infra.Context;
using SACA_Infra.Models;
using SACA_Common.DTOs.Notification.Response;
using SACA_Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SACA_Infra.SignalR;
using SACA_Common.Enums;
using SACA_Common.Models;
using Microsoft.AspNetCore.Mvc;
namespace SACA_Service.Services
{
    public interface INotificationService
    {
        Task<List<NotificationResponse>> NotificateAsync();
        Task<bool> AddNotificationAsync(NotificationCreating model);
        Task<bool> UpdateNotificationAsync(int id, NotificationCreating model);
        Task<bool> DeleteNotificationAsync(string id);
        Task<NotificationResponse> GetNotificationByIdAsync(string id);
        Task<bool> SendNotificationToContest(string contestId, string notificationId, string userId);
        Task<NotificationResponse> GetNotificationAsync(string id);
        Task<bool> MarkAsReadAsync(string notificationId, string userId);
        Task<bool> DeleteAccountNotificationAsync(string id, string userId);
        Task<List<AccountNotificationModel>> GetAccountNotificationAsync(string userId);
    }
    public class NotificationService : INotificationService
    {
        private readonly SACA_Context _context;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationService
        (
            SACA_Context context,
            IMapper mapper,
            IHubContext<NotificationHub> hubContext
        )
        {
            _context = context;
            _mapper = mapper;
            _hubContext = hubContext;
        }
        public NotificationService(SACA_Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<NotificationResponse>> NotificateAsync()
        {
            var notifications = await _context.notifications
                .Include(n => n.contest)
                .Include(n => n.problem)
                .ToListAsync();

            var notificationDtos = notifications.Select(n => new NotificationResponse
            {
                id = n.id,
                title = n.title,
                description = n.description,
                contest_code = n.contest.code,
                problem_name = n.problem.title,
                contest_id = n.contest_id,
                problem_id = n.problem_id
            }).ToList();

            return notificationDtos;
        }
        public async Task<NotificationResponse> GetNotificationByIdAsync(string id)
        {
            var notification = await _context.notifications
                .Include(n => n.contest)
                .Include(n => n.problem)
                .FirstOrDefaultAsync(n => n.id == id);

            if (notification == null)
            {
                throw new BadException(ErrorMessage.NotificationNotFound);
            }

            return new NotificationResponse
            {
                id = notification.id,
                title = notification.title,
                description = notification.description,
                contest_code = notification.contest.code,
                problem_name = notification.problem.title
            };
        }
        public async Task<bool> AddNotificationAsync(NotificationCreating model)
        {
            if (string.IsNullOrEmpty(model.title))
            {
                throw new BadException(ErrorMessage.TitleIsRequired);
            }

            if (string.IsNullOrEmpty(model.description))
            {
                throw new BadException(ErrorMessage.DescriptionIsRequired);
            }

            if (string.IsNullOrEmpty(model.contest_id))
            {
                throw new BadException(ErrorMessage.ContestIdIsRequired);
            }
            if (string.IsNullOrEmpty(model.problem_id))
            {
                throw new BadException(ErrorMessage.ProblemIsRequired);
            }
            var notification = _mapper.Map<NotificationCreating, notification>(model);
            await _context.notifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> UpdateNotificationAsync(int id, NotificationCreating model)
        {
            var notification = await _context.notifications.FindAsync(id);
            if (notification == null)
            {
                throw new BadException(ErrorMessage.NotificationNotFound);
            }

            try
            {
                model.Validate();
            }
            catch (BadException)
            {
                return false;
            }

            _mapper.Map(model, notification);
            _context.notifications.Update(notification);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteNotificationAsync(string id)
        {
            var notification = await _context.notifications.FindAsync(id);
            if (notification == null)
            {
                throw new BadException(ErrorMessage.NotificationNotFound);
            }

            _context.notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> SendNotificationToContest(string contestId, string notificationId, string userId)
        {
            var notification = await GetNotificationAsync(notificationId);
            var contestParticipantIds = await _context.contest_participants.AsNoTracking()
                .Where(e => e.contest_id == contestId)
                .Select(e => e.account_id)
                .ToListAsync();
            var accountNotifications = new List<account_notification>();
            foreach (var participantId in contestParticipantIds)
            {
                var accountNotification = new account_notification
                {
                    account_id = participantId,
                    notification_id = notification.id,
                    status = (int)eStatus_AccountNotification.Unread
                };
                accountNotification.Created(userId);
                accountNotifications.Add(accountNotification);
            }
            _context.account_notifications.AddRange(accountNotifications);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.Group(contestId).SendAsync("ReceiveNotification", notification);
            return true;
        }

        public async Task<NotificationResponse> GetNotificationAsync(string id)
        {
            var result = await _context.notifications.AsNoTracking()
                .Include(e => e.contest)
                .Include(e => e.problem)
                .Select(e => new NotificationResponse
                {
                    id = e.id,
                    title = e.title,
                    description = e.description,
                    contest_code = e.contest.code,
                    problem_name = e.problem.title,
                    contest_id = e.contest_id,
                    problem_id = e.problem_id
                })
                .FirstOrDefaultAsync();
            if (result == null)
            {
                throw new NotFoundException(ErrorMessage.NotFound);
            }
            return result;
        }

        public async Task<bool> MarkAsReadAsync(string notificationId, string userId)
        {
            var accountNotification = await _context.account_notifications
                .FirstOrDefaultAsync(e => e.notification_id == notificationId && e.account_id == userId);
            if (accountNotification == null)
            {
                throw new NotFoundException(ErrorMessage.NotFound);
            }
            accountNotification.status = (int)eStatus_AccountNotification.Read;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAccountNotificationAsync(string id, string userId)
        {
            var accountNotification = await _context.account_notifications
                .FirstOrDefaultAsync(e => e.id == id);
            if (accountNotification == null)
            {
                throw new BadException(ErrorMessage.NotFound);
            }
            accountNotification.Deleted(userId);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<AccountNotificationModel>> GetAccountNotificationAsync(string userId)
        {
            var accountNotification = await _context.account_notifications.Include(a => a.notification)
                                        .Where(a => a.account_id == userId).Select(a => new AccountNotificationModel
                                        {
                                            title = a.notification.contest.title,
                                            description = a.notification.contest.description
                                        }).ToListAsync();
            return accountNotification;
        }
    }
    public static class NotificationSerivceExtension
    {
        public static void Validate(this NotificationCreating form)
        {
            if (string.IsNullOrEmpty(form.title))
            {
                throw new BadException(ErrorMessage.TitleIsRequired);
            }

            if (string.IsNullOrEmpty(form.description))
            {
                throw new BadException(ErrorMessage.DescriptionIsRequired);
            }

            if (string.IsNullOrEmpty(form.contest_id))
            {
                throw new BadException(ErrorMessage.ContestIdIsRequired);
            }
            if (string.IsNullOrEmpty(form.problem_id))
            {
                throw new BadException(ErrorMessage.ProblemIsRequired);
            }
        }
    }
}
