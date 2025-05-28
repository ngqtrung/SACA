using SACA_Common.DTOs;
using SACA_FE.Const;
using SACA_Common.DTOs.Notification.Request;
using SACA_Common.Routes;
using SACA_Common.DTOs.Notification.Response;
using SACA_Common.DTOs.Notification;
using Newtonsoft.Json;

namespace SACA_FE.Services
{
    public interface INotifiService
    {
        Task<CreateResult> CreateAsync(NotificationCreating form);
        Task<List<NotificationResponse>> GetAllAsync();
        Task<bool> DeleteByIdAsync(string id);
        Task<NotificationResponse> GetById(string id);
        Task<List<AccountNotificationModel>> GetAccountNotification();
        Task<bool> SendAsync(SendNotificationToContestRequest request);
    }
    public class NotifiService : INotifiService
    {
        private readonly IHttpClientBase _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public NotifiService(IHttpClientBase httpClient, IHttpContextAccessor contextAccessor)
        {
            _httpClient = httpClient;
            _contextAccessor = contextAccessor;
        }
        public async Task<CreateResult> CreateAsync(NotificationCreating form)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{NotificationRoutes.INDEX}/{NotificationRoutes.ACTION.Create}";
            var result = await _httpClient.PostAsync<CreateResult, NotificationCreating>(path, form, header);
            return result;
        }
        public async Task<List<NotificationResponse>> GetAllAsync()
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{NotificationRoutes.INDEX}/{NotificationRoutes.ACTION.GetAll}";
            var result = await _httpClient.GetAsync<List<NotificationResponse>>(path, header);
            return result;
        }
        public async Task<NotificationResponse> GetById(string id)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{NotificationRoutes.INDEX}/{NotificationRoutes.ACTION.GetById}?id={id}";
            var result = await _httpClient.GetAsync<NotificationResponse>(path, header);
            return result;
        }
        public async Task<bool> DeleteByIdAsync(string id)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{NotificationRoutes.INDEX}/{NotificationRoutes.ACTION.Delete}?id={id}";
            var result = await _httpClient.DeleteAsync<bool, string?>(path, null, header);
            return true;
        }
        public async Task<List<AccountNotificationModel>> GetAccountNotification()
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{NotificationRoutes.INDEX}/{NotificationRoutes.ACTION.GetAccountNotification}";
            var result = await _httpClient.GetAsync<List<AccountNotificationModel>>(path, header);
            return result;
        }
        public async Task<bool> SendAsync(SendNotificationToContestRequest request)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{NotificationRoutes.INDEX}/{NotificationRoutes.ACTION.SendNotificationToContest}";
            var result = await _httpClient.PostAsync<bool, SendNotificationToContestRequest>(path, request, header);
            return result;
        }
    }
}
