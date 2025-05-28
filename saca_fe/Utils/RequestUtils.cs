using System.Net;
using System.Net.Http.Headers;

namespace SACA_FE.Utils
{
    public static class RequestUtils
    {
        public static HttpRequestHeaders GetAuthorizedRequestHeader(IHttpContextAccessor _contextAccessor)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(authToken))
            {
                throw new UnauthorizedAccessException("Authorization token is missing.");
            }

            var requestMessage = new HttpRequestMessage();
            requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

            return requestMessage.Headers;
        }
    }
}
