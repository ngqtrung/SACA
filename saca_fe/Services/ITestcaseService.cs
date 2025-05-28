using SACA_Common.DTOs.Account.Request;
using SACA_Common.DTOs;
using SACA_Common.DTOs.TestCase.Request;
using SACA_Common.Routes;
using SACA_FE.Const;
using System.Net.Http.Headers;

namespace SACA_FE.Services
{
    public interface ITestcaseService
    {
        Task<List<TestCaseCreating>> ImportExcel(IFormFile file);
    }
    public class TestCaseService : ITestcaseService
    {

        private readonly IHttpClientBase _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;
        public TestCaseService
        (
            IHttpClientBase httpClient,
            IHttpContextAccessor contextAccessor
        )
        {
            _httpClient = httpClient;
            _contextAccessor = contextAccessor;
        }

        public async Task<List<TestCaseCreating>> ImportExcel(IFormFile file)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);


            if (file == null || file.Length == 0)
            {
                Console.WriteLine("File is empty.");
                return new List<TestCaseCreating>();
            }
            using var stream = file.OpenReadStream();
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            using var form = new MultipartFormDataContent();
            form.Add(streamContent, "file", file.FileName);

            var path = @$"{TestCaseRoutes.INDEX}/{TestCaseRoutes.ACTION.ImportExcel}";
            var result = await _httpClient.PostAsync<Response<List<TestCaseCreating>>, MultipartFormDataContent>(path, form, header);
            return result.Result ?? new List<TestCaseCreating>();
        }
    }
}
