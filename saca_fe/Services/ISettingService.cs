using SACA_Common.DTOs;
using SACA_FE.Const;
using SACA_Common.Routes;
using SACA_Common.DTOs.SysSetting.Response;
using SACA_Common.DTOs.SysSetting.Request;

namespace SACA_FE.Services
{
    public interface ISettingService
    {
        Task<List<SysSettingView>> GetAllAsync();
        Task<bool> UpdateAsync(List<SysSettingUpdate> form);
    }

    public class SettingService : ISettingService
    {
        private readonly IHttpClientBase _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public SettingService(IHttpClientBase httpClient, IHttpContextAccessor contextAccessor)
        {
            _httpClient = httpClient;
            _contextAccessor = contextAccessor;
        }

        public async Task<List<SysSettingView>> GetAllAsync()
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var path = @$"{SysSettingRoutes.INDEX}";
            var result = await _httpClient.GetAsync<Response<List<SysSettingView>>>(path, header);
            return result.Result!;
        }

        public async Task<bool> UpdateAsync(List<SysSettingUpdate> form)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{SysSettingRoutes.INDEX}/{SysSettingRoutes.Action.Update}";
            var result = await _httpClient.PutAsync<Response<bool>, List<SysSettingUpdate>>(path, form, header);
            return result.Result;
        }
    }
}
