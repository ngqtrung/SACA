using Org.BouncyCastle.Asn1.Mozilla;
using SACA_Common.DTOs;
using SACA_Common.DTOs.Contest.Response;
using SACA_Common.DTOs.Submission.Request;
using SACA_Common.DTOs.Submission.Response;
using SACA_Common.Routes;
using SACA_FE.Const;

namespace SACA_FE.Services
{
    public interface ISubmissionService
    {
        Task<bool> SubmitSolutionAsync(SubmitSolutionRequest form);
        Task<PagedResponse<SubmissionTableView>> SearchAsync(SubmissionTableFilter filter);
        Task<SubmissionView> GetDetail(string id);
        Task<bool> ResubmitSolutionAsync(ResubmitSolutionRequest request);
    }
    public class SubmissionService : ISubmissionService
    {
        private readonly IHttpClientBase _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;
        public SubmissionService
        (
            IHttpClientBase httpClient,
            IHttpContextAccessor contextAccessor
        )
        {
            _httpClient = httpClient;
            _contextAccessor = contextAccessor;
        }

        public async Task<SubmissionView> GetDetail(string id)
        {
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{SubmissionRoutes.INDEX}/{id}";
            var result = await _httpClient.GetAsync<Response<SubmissionView>>(path, header);
            return result.Result ?? new SubmissionView();
        }

        public async Task<PagedResponse<SubmissionTableView>> SearchAsync(SubmissionTableFilter filter)
        {
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var queryParams = $"?page_index={filter.page_index}&page_size={filter.page_size}&keyword={Uri.EscapeDataString(filter.keyword ?? "")}&contest_id={filter.contest_id}&status={filter.status}&programming_language={filter.programming_language}";
            var path = @$"{SubmissionRoutes.INDEX}/{SubmissionRoutes.ACTION.Search}{queryParams}";
            var result = await _httpClient.GetAsync<PagedResponse<SubmissionTableView>>(path, header);
            return result;
        }

        public async Task<bool> SubmitSolutionAsync(SubmitSolutionRequest form)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";

            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{SubmissionRoutes.INDEX}";
            var result = await _httpClient.PostAsync<Response<bool>, SubmitSolutionRequest>(path, form, header);
            return result.Result;
        }
        public async Task<bool> ResubmitSolutionAsync(ResubmitSolutionRequest request)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";

            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{SubmissionRoutes.INDEX}/{SubmissionRoutes.ACTION.ResubmitSolution}";
            var result = await _httpClient.PostAsync<Response<bool>, ResubmitSolutionRequest>(path, request, header);
            return result.Result;
        }
    }
}
