using SACA_Common.DTOs;
using SACA_Common.DTOs.Authenticate.Request;
using SACA_Common.DTOs.Authenticate.Response;
using SACA_Common.DTOs.Contest.Request;
using SACA_Common.DTOs.Contest.Response;
using SACA_Common.DTOs.Problem.Request;
using SACA_Common.Exceptions;
using SACA_Common.Routes;
using SACA_FE.Const;
using System.Net.Http.Headers;

namespace SACA_FE.Services
{
    public interface IContestService
    {
        Task<PagedResponse<ContestTableView>> SearchAsync(ContestTableFilter filter);
        Task<List<string>> GetUserContestsAsync();
        Task<Response<CreateResult>> CreateAsync(ContestCreating form);
        Task<Response<bool>> UpdateAsync(ContestUpdating form);
        Task<ContestView?> GetDetailAsync(string id);
        Task<Response<bool>> FrozenContestAsync(string contestId);
        Task<DateTime> GetContestEndTime(string id);
        Task<Response<bool>> DeleteAsync(string id);
        Task<List<ContestTableView>> GetAllContestAsync();
        Task<ContestCreating> ImportExcel(IFormFile file);
    }
    public class ContestService : IContestService
    {
        private readonly IHttpClientBase _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public ContestService
        (
            IHttpClientBase httpClient,
            IHttpContextAccessor contextAccessor
        )
        {
            _httpClient = httpClient;
            _contextAccessor = contextAccessor;
        }
        public async Task<Response<CreateResult>> CreateAsync(ContestCreating form)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var path = @$"{ContestRoutes.INDEX}/{ContestRoutes.ACTION.Create}";
            var result = await _httpClient.PostAsync<Response<CreateResult>, ContestCreating>(path, form, header);
            return result;
        }

        public async Task<Response<bool>> UpdateAsync(ContestUpdating form)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var path = @$"{ContestRoutes.INDEX}/{ContestRoutes.ACTION.Update}";
            var result = await _httpClient.PutAsync<Response<bool>, ContestUpdating>(path, form, header);
            return result;
        }

        public async Task<Response<bool>> FrozenContestAsync(string contestId)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var path = @$"{ContestRoutes.INDEX}/{ContestRoutes.ACTION.FrozenContest}";
            var result = await _httpClient.PostAsync<Response<bool>, string>(path, contestId, header);
            return result;
        }

        public async Task<ContestView?> GetDetailAsync(string id)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{ContestRoutes.INDEX}/{id}";
            var result = await _httpClient.GetAsync<Response<ContestView>>(path, header);
            var contest = result.Result;
            return contest;
        }

        public async Task<PagedResponse<ContestTableView>> SearchAsync(ContestTableFilter filter)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{ContestRoutes.INDEX}/{ContestRoutes.ACTION.Search}?page_index={filter.page_index}&page_size={filter.page_size}&keyword={filter.keyword}&status={filter.status}";
            var result = await _httpClient.GetAsync<PagedResponse<ContestTableView>>(path, header);
            return result;
        }
        public async Task<List<string>> GetUserContestsAsync()
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{ContestRoutes.INDEX}/{ContestRoutes.ACTION.GetUserContests}";
            var result = await _httpClient.GetAsync<Response<List<string>>>(path, header);
            return result?.Result ?? new List<string>();
        }

        public async Task<DateTime> GetContestEndTime(string id)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{ContestRoutes.INDEX}/{ContestRoutes.ACTION.GetContestEndTime.Replace("{id}", id)}";
            var result = await _httpClient.GetAsync<Response<DateTime>>(path, header);
            var datetime = result.Result;
            return datetime;
        }

        public async Task<Response<bool>> DeleteAsync(string id)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var path = @$"{ContestRoutes.INDEX}/{id}";
            var result = await _httpClient.DeleteAsync<Response<bool>, string>(path, id, header);
            return result;
        }

        public async Task<List<ContestTableView>> GetAllContestAsync()
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{ContestRoutes.INDEX}/{ContestRoutes.ACTION.GetAll}";
            var result = await _httpClient.GetAsync<List<ContestTableView>>(path, header);
            return result;
        }

        public async Task<ContestCreating> ImportExcel(IFormFile file)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);


            if (file == null || file.Length == 0)
            {
                Console.WriteLine("File is empty.");
                return new ContestCreating();
            }
            using var stream = file.OpenReadStream();
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            using var form = new MultipartFormDataContent();
            form.Add(streamContent, "file", file.FileName);

            var path = @$"{ContestRoutes.INDEX}/{ContestRoutes.ACTION.ImportExcel}";
            var result = await _httpClient.PostAsync<Response<ContestCreating>, MultipartFormDataContent>(path, form, header);
            return result.Result ?? new ContestCreating();
        }
    }
}
