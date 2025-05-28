using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Infra.SignalR
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            string userId = Context.User?.Identity?.Name ?? "Unknown";
            Console.WriteLine($"📡 New connection: {Context.ConnectionId} (User: {userId})");
            await base.OnConnectedAsync();
        }
        public async Task SendNotification(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
        // Khi một thí sinh kết nối, thêm họ vào group của cuộc thi
        public async Task JoinContest(string contestId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, contestId);
        }

        // Khi cần gửi thông báo tới tất cả thí sinh trong cuộc thi
        public async Task SendNotificationToContest(string contestId, string message)
        {

            await Clients.Group(contestId).SendAsync("ReceiveNotification", message);
        }
    }
}
