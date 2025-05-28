using Org.BouncyCastle.Asn1.Ocsp;
using SACA_Common.DTOs;
using SACA_Common.DTOs.Authenticate.Request;
using SACA_Common.DTOs.Authenticate.Response;
using SACA_Common.Routes;
using SACA_FE.Const;

namespace SACA_FE.Services
{
    public interface IAuthenService
    {
        Task<LoginResponse> Login(LoginRequest request);
        Task<Response<bool>> ChangePasswordAsync(ChangePasswordRequest form);
    }
    public class AuthenService : IAuthenService
    {
        private readonly IHttpClientBase _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;
        public AuthenService(IHttpClientBase httpClient, IHttpContextAccessor contextAccessor)
        {
            _httpClient = httpClient;
            _contextAccessor = contextAccessor;
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var path = @$"{AuthenticateRoutes.INDEX}/{AuthenticateRoutes.ACTION.Login}";
            var result = await _httpClient.PostAsync<Response<LoginResponse>, LoginRequest>(path, request);
            return result.Result!;
        }
        public async Task<Response<bool>> ChangePasswordAsync(ChangePasswordRequest form)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{AuthenticateRoutes.INDEX}/{AuthenticateRoutes.ACTION.ChangePassword}";
            var result = await _httpClient.PostAsync<Response<bool>, ChangePasswordRequest>(path, form, header);
            return result;
        }
    }
}
