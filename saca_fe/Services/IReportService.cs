using Microsoft.AspNetCore.WebUtilities;
using Org.BouncyCastle.Asn1.Ocsp;
using SACA_Common.DTOs;
using SACA_Common.DTOs.Contest.Request;
using SACA_Common.DTOs.Report.ScoreBoard.Request;
using SACA_Common.DTOs.Report.ScoreBoard.Response;
using SACA_Common.DTOs.Report.ScoreDistribution.Request;
using SACA_Common.DTOs.Report.ScoreDistribution.Response;
using SACA_Common.Routes;
using SACA_FE.Const;
using SACA_FE.DTOs.Report;

namespace SACA_FE.Services
{
    public interface IReportService
    {
        Task<IFormFile> ExportContests(ExportContestRequest form);
        Task<IFormFile> ExportContestParticipants(string contestId);
        Task<IFormFile> ExportContestProblems(string contestId);
        Task<PagedResponse<ScoreBoardResponse>> GetScoreBoard(GetScoreBoardRequest filter);
        Task<IFormFile> ExportScoreBoard(ExportScoreBoardRequest request);
        Task<List<ScoreDistributionDetailView>> GetScoreDistribution(GetScoreDistributionRequest filter);
    }
    public class ReportService : IReportService
    {
        private readonly IHttpClientBase _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;
        public ReportService
        (
            IHttpClientBase httpClient,
            IHttpContextAccessor contextAccessor
        )
        {
            _httpClient = httpClient;
            _contextAccessor = contextAccessor;
        }

        public async Task<IFormFile> ExportContestParticipants(string contestId)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{ReportRoute.INDEX}/{ReportRoute.ACTION.ContestParticipants}";
            var result = await _httpClient.GetFileAsIFormFile(path, contestId, header);
            return result;
        }


        public async Task<IFormFile> ExportContests(ExportContestRequest form)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{ReportRoute.INDEX}/{ReportRoute.ACTION.Contest}";
            var result = await _httpClient.GetFileAsIFormFile(path, form, header);
            return result;
        }
        public async Task<PagedResponse<ScoreBoardResponse>> GetScoreBoard(GetScoreBoardRequest filter)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var path = @$"{ReportRoute.INDEX}/{ReportRoute.ACTION.ScoreBoard}?page_index={filter.page_index}&page_size={filter.page_size}&keyword={filter.keyword}&contest_id={filter.contest_id}&class_code={filter.class_code}&subject_code={filter.subject_code}";
            var result = await _httpClient.GetAsync<PagedResponse<ScoreBoardResponse>>(path, header);
            return result;
        }
        public async Task<IFormFile> ExportContestProblems(string contestId)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{ReportRoute.INDEX}/{ReportRoute.ACTION.ContestProblems}";
            var result = await _httpClient.GetFileAsIFormFile(path, contestId, header);
            return result;
        }

        public async Task<IFormFile> ExportScoreBoard(ExportScoreBoardRequest request)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{ReportRoute.INDEX}/{ReportRoute.ACTION.ExportScoreBoard}";
            var result = await _httpClient.GetFileAsIFormFile(path, request, header);
            return result;
        }

        public async Task<List<ScoreDistributionDetailView>> GetScoreDistribution(GetScoreDistributionRequest filter)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var queryParams = new Dictionary<string, string?>
            {
                { "contest_id", filter.contest_id },
                { "problem_ids", string.Join(",", filter.problem_ids) }
            };
            var path = @$"{ReportRoute.INDEX}/{ReportRoute.ACTION.ScoreDistribution}";
            path = QueryHelpers.AddQueryString(path, queryParams);
            var result = await _httpClient.GetAsync<List<ScoreDistributionDetailView>>(path, header);
            return result;
        }
    }
}
