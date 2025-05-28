using SACA_Common.DTOs.Contest.Request;
using SACA_Common.DTOs;
using SACA_Common.Routes;
using SACA_FE.Const;
using SACA_Common.DTOs.Account.Response;
using SACA_Common.DTOs.Account.Request;
using SACA_Common.DTOs.GradingMachine.Response;
using SACA_Common.APIRoutes;

namespace SACA_FE.Services
{
    public interface IPlagiarismService
    {
        Task<bool> CheckPlagiarism(string contestId, string problemId, int languageCode);
        Task<bool> CheckPlagiarismByContestId(string contestId);
    }
    public class PlagiarismService : IPlagiarismService
    {
        private readonly IHttpClientBase _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public PlagiarismService
        (
            IHttpClientBase httpClient,
            IHttpContextAccessor contextAccessor
        )
        {
            _httpClient = httpClient;
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            _contextAccessor = contextAccessor;
        }

        public async Task<bool> CheckPlagiarism(string contestId, string problemId, int languageCode)
        {
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{JPlagRoutes.INDEX}/{JPlagRoutes.Action.CheckPlagiarism}?contestId={contestId}&problemId={problemId}&languageCode={languageCode}";
            var result = await _httpClient.GetAsync<bool>(path,header);
            return result;
        }

        public async Task<bool> CheckPlagiarismByContestId(string contestId)
        {
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{JPlagRoutes.INDEX}/{JPlagRoutes.Action.CheckPlagiarismByContestId}?contestId={contestId}";
            var result = await _httpClient.GetAsync<bool>(path, header);
            return result;
        }
    }

}
