using SACA_Common.DTOs.Contest.Request;
using SACA_Common.DTOs;
using SACA_Common.Routes;
using SACA_FE.Const;
using SACA_Common.DTOs.Account.Response;
using SACA_Common.DTOs.Account.Request;
using SACA_Common.DTOs.SysSetting.Response;
using SACA_Common.DTOs.Authenticate.Request;
using System.Net.Http.Headers;

namespace SACA_FE.Services
{
    public interface IAccountService
    {
        Task<Response<AccountView>> GetDetailAsync(string id);
        Task<PagedResponse<AccountView>> GetListMembersAsync(MemberTableFilter filter);
        Task<PagedResponse<AccountView>> SearchAsync(AccountTableFilter filter);
        Task<bool> UpdateAsync(AccountUpdating form);
        Task<bool> ResetPasswordAsync(ResetPasswordRequest form, string accountId);
        Task<bool> DeleteAsync(string accountId);
        Task<CreateResult> CreateAsync(AccountCreating form);
        Task<List<AccountCreating>> ImportExcel(IFormFile file);
    }
    public class AccountService : IAccountService
    {
        private readonly IHttpClientBase _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public AccountService
        (
            IHttpClientBase httpClient,
            IHttpContextAccessor contextAccessor
        )
        {
            _httpClient = httpClient;
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            _contextAccessor = contextAccessor;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest form, string accountId)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{AccountRoutes.INDEX}/reset-password/{accountId}";
            var result = await _httpClient.PostAsync<Response<bool>, ResetPasswordRequest>(path, form, header);
            return result.Result;
        }

        public async Task<CreateResult> CreateAsync(AccountCreating form)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{AccountRoutes.INDEX}/{AccountRoutes.ACTION.Create}";
            var result = await _httpClient.PostAsync<Response<CreateResult>, AccountCreating>(path, form, header);
            return result.Result!;
        }

        public async Task<bool> DeleteAsync(string accountId)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{AccountRoutes.INDEX}/{accountId}";
            var result = await _httpClient.DeleteAsync<Response<bool>, string>(path, accountId, header);
            return result.Result;
        }

        public async Task<Response<AccountView>> GetDetailAsync(string id)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{AccountRoutes.INDEX}/{id}";
            var result = await _httpClient.GetAsync<Response<AccountView>>(path, header);
            return result;
        }
        public async Task<PagedResponse<AccountView>> GetListMembersAsync(MemberTableFilter filter)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{AccountRoutes.INDEX}/{AccountRoutes.ACTION.GetListMembers}?contest_id={filter.contest_id}&page_index={filter.page_index}&page_size={filter.page_size}&keyword={filter.keyword}";
            var result = await _httpClient.GetAsync<PagedResponse<AccountView>>(path, header);
            return result;
        }
        public async Task<PagedResponse<AccountView>> SearchAsync(AccountTableFilter filter)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var path = @$"{AccountRoutes.INDEX}?page_index={filter.page_index}&page_size={filter.page_size}&keyword={filter.keyword}&status={filter.status}";
            var result = await _httpClient.GetAsync<PagedResponse<AccountView>>(path, header);
            return result;
        }
        public async Task<bool> UpdateAsync(AccountUpdating form)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{AccountRoutes.INDEX}/{AccountRoutes.ACTION.Update}";
            var result = await _httpClient.PutAsync<Response<bool>, AccountUpdating>(path, form, header);
            return result.Result;
        }

        public async Task<List<AccountCreating>> ImportExcel(IFormFile file)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);


            if (file == null || file.Length == 0)
            {
                Console.WriteLine("File is empty.");
                return new List<AccountCreating>();
            }
            using var stream = file.OpenReadStream();
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            using var form = new MultipartFormDataContent();
            form.Add(streamContent, "file", file.FileName);

            var path = @$"{AccountRoutes.INDEX}/{AccountRoutes.ACTION.ImportExcel}";
            var result = await _httpClient.PostAsync<Response<List<AccountCreating>>, MultipartFormDataContent>(path, form, header);
            return result.Result ?? new List<AccountCreating>();
        }
    }

}
