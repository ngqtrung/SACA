using SACA_Common.DTOs.Contest.Request;
using SACA_Common.DTOs;
using SACA_Common.Routes;
using SACA_FE.Const;
using SACA_Common.DTOs.Account.Response;
using SACA_Common.DTOs.Account.Request;
using SACA_Common.DTOs.GradingMachine.Response;

namespace SACA_FE.Services
{
    public interface IGradingMachineService
    {
        Task<GradingMachineInfo> GetGradingMachineInfo();
    }
    public class GradingMachineService : IGradingMachineService
    {
        private readonly IHttpClientBase _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public GradingMachineService
        (
            IHttpClientBase httpClient,
            IHttpContextAccessor contextAccessor
        )
        {
            _httpClient = httpClient;
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            _contextAccessor = contextAccessor;
        }

        public async Task<GradingMachineInfo> GetGradingMachineInfo()
        {
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{GradingMachineRoutes.INDEX}/{GradingMachineRoutes.ACTION.GetInfo}";
            var result = await _httpClient.GetAsync<Response<GradingMachineInfo>>(path,header);
            return result.Result ?? new GradingMachineInfo();
        }
    }

}
