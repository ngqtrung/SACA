using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SACA_FE.Utils
{
    public class TokenAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                var claims = ValidateToken(token); 
                if (claims != null)
                {
                    var identity = new ClaimsIdentity(claims, "SACA");
                    var roleClaim = claims.FirstOrDefault(c => c.Type == "role");
                    if (roleClaim != null)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                    }

                    var principal = new ClaimsPrincipal(identity);
                    await context.SignInAsync("SACA", principal);
                }
            }

            await _next(context);
        }

        private List<Claim>? ValidateToken(string token)
        {
            // Giải mã token và trích xuất thông tin người dùng
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jwtToken = handler.ReadJwtToken(token);
                return jwtToken.Claims.ToList();
            }
            catch
            {
                return null;
            }
        }
    }

}
