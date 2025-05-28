using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SACA_Common.Authentications;

namespace SACA_FE.Utils
{
    public static class TokenUtils
    {
        private const string CookieName = "AuthToken"; // Tên cookie chứa JWT

        public static UserHeader? GetUserInfo(HttpContext context)
        {
            var token = context.Request.Cookies[CookieName];
            return ParseToken(token);
        }

        public static string? GetClaim(HttpContext context, string claimType)
        {
            var user = GetUserInfo(context);
            return user?.GetType().GetProperty(claimType)?.GetValue(user)?.ToString();
        }

        private static UserHeader? ParseToken(string? token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var claims = jwtToken.Claims.ToList();

                return new UserHeader
                {
                    user_id = claims.FirstOrDefault(c => c.Type == "userid")?.Value ?? "",
                    username = claims.FirstOrDefault(c => c.Type == "username")?.Value ?? "",
                    fullname = claims.FirstOrDefault(c => c.Type == "fullname")?.Value ?? "",
                    role = claims.FirstOrDefault(c => c.Type == "role")?.Value ?? "",
                    email = claims.FirstOrDefault(c => c.Type == "email")?.Value ?? ""
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
