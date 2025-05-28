using Org.BouncyCastle.Asn1.Mozilla;
using SACA_Common.DTOs;
using SACA_Common.DTOs.Contest.Response;
using SACA_Common.DTOs.LeaderBoard.Request;
using SACA_Common.DTOs.LeaderBoard.Response;
using SACA_Common.DTOs.Submission.Request;
using SACA_Common.DTOs.Submission.Response;
using SACA_Common.Routes;
using SACA_FE.Const;

namespace SACA_FE.Services
{
    public interface ILeadboardService
    {
        Task<LeaderBoardTableView> Search(LeaderBoardTableFilter form);
    }
    public class LeadboardService : ILeadboardService
    {
        private readonly IHttpClientBase _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;
        public LeadboardService
        (
            IHttpClientBase httpClient,
            IHttpContextAccessor contextAccessor
        )
        {
            _httpClient = httpClient;
            _contextAccessor = contextAccessor;
        }
        public async Task<LeaderBoardTableView> Search(LeaderBoardTableFilter filter)
        {
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var queryParams = $"?page_index={filter.page_index}&page_size={filter.page_size}&contest_id={filter.contest_id}";
            var path = @$"{LeaderBoardRoute.INDEX}/{LeaderBoardRoute.ACTION.SEARCH}{queryParams}";
            var result = await _httpClient.GetAsync<Response<LeaderBoardTableView>>(path, header);
            return result.Result ?? new LeaderBoardTableView();
        }
    }
}
