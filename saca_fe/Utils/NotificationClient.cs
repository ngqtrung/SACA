using Microsoft.AspNetCore.SignalR.Client;

namespace SACA_FE.Utils
{
    public class NotificationClient
    {
        private readonly HubConnection _connection;
        public NotificationClient(string hubUrl)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .Build();
        }
        public async Task StartConnectionAsync()
        {
            _connection.On<string>("ReceiveNotification", (message) =>
            {
                // update UI
            });
            await _connection.StartAsync();
        }
        public async Task SendNotificationAsync(string userId, string message)
        {
            await _connection.InvokeAsync("SendNotification", userId, message);
        }
    }
}
