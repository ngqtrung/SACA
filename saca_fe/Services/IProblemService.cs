using SACA_Common.DTOs;
using SACA_FE.Const;
using SACA_Common.Routes;
using Microsoft.AspNetCore.Mvc;
using SACA_Common.DTOs.Problem.Request;
using SACA_Common.DTOs.Problem.Response;
using Microsoft.AspNetCore.WebUtilities;
using SACA_Common.DTOs.TestCase.Request;
using System.Net.Http.Headers;

namespace SACA_FE.Services
{
    public interface IProblemService
    {
        Task<PagedResponse<ProblemTableView>> Search(ProblemTableFilter filter);
        Task<ProblemView> GetDetailAsync(string id);
        Task<List<ProblemCreating>> ImportExcel(IFormFile file);
    }

    public class ProblemService : IProblemService
    {
        private readonly IHttpClientBase _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public ProblemService(IHttpClientBase httpClient, IHttpContextAccessor contextAccessor)
        {
            _httpClient = httpClient;
            _contextAccessor = contextAccessor;
        }
        public async Task<PagedResponse<ProblemTableView>> Search(ProblemTableFilter request)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";

            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);

            var queryParams = new Dictionary<string, string>();
            queryParams.Add("contest_id", request.contest_id ?? "");
            queryParams.Add("page_index", request.page_index.ToString());
            queryParams.Add("page_size", request.page_size.ToString());
            queryParams.Add("keyword", request.keyword ?? "");

            var path = QueryHelpers.AddQueryString(@$"{ProblemRoutes.INDEX}", queryParams);

            var result = await _httpClient.GetAsync<PagedResponse<ProblemTableView>>(path, header);
            return result;
        }


        public async Task<ProblemView?> GetDetailAsync(string id)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{ProblemRoutes.INDEX}/{id}";
            var result = await _httpClient.GetAsync<Response<ProblemView>>(path, header);
            var problem = result.Result;
            return problem;
        }


        public async Task<List<ProblemCreating>> ImportExcel(IFormFile file)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);


            if (file == null || file.Length == 0)
            {
                Console.WriteLine("File is empty.");
                return new List<ProblemCreating>();
            }
            using var stream = file.OpenReadStream();
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            using var form = new MultipartFormDataContent();
            form.Add(streamContent, "file", file.FileName);

            var path = @$"{ProblemRoutes.INDEX}/{ProblemRoutes.ACTION.ImportExcel}";
            var result = await _httpClient.PostAsync<Response<List<ProblemCreating>>, MultipartFormDataContent>(path, form, header);
            return result.Result ?? new List<ProblemCreating>();
        }
    }
}
