using SACA_Common.DTOs.Contest.Request;
using SACA_Common.DTOs;
using SACA_Common.Routes;
using SACA_FE.Const;
using SACA_Common.DTOs.Account.Response;
using SACA_Common.DTOs.Account.Request;
using SACA_Common.DTOs.Mail.Request;
using Org.BouncyCastle.Asn1.Ocsp;

namespace SACA_FE.Services
{
    public interface IMailService
    {
        Task<bool> SendMailAsync(SendMailInvite request);
    }
    public class MailService : IMailService
    {
        private readonly IHttpClientBase _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public MailService
        (
            IHttpClientBase httpClient,
            IHttpContextAccessor contextAccessor
        )
        {
            _httpClient = httpClient;
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            _contextAccessor = contextAccessor;
        }

        public async Task<bool> SendMailAsync(SendMailInvite request)
        {
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            var path = @$"{MailRoutes.INDEX}/{MailRoutes.ACTION.SendMaiInvite}";
            var result = await _httpClient.PostAsync<Response<bool>, object>(path, request, header);
            return result.Result;
        }
    }

}
