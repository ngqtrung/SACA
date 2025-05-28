using Microsoft.IdentityModel.Tokens;
using SACA_Common.Authentications;
using SACA_Infra.Const;
using SACA_Infra.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Service.Utils
{
    public static class JwtHelper
    {
        public static string RenderAccessToken(sys_account account)
        {
            var claims = new List<Claim>()
            {
                new Claim(CustomJwtClaimType.UserId, account.id),
                new Claim(CustomJwtClaimType.Username, account.username),
                new Claim(CustomJwtClaimType.Fullname, account.fullname),
                new Claim(CustomJwtClaimType.Email, account.email),
                new Claim(CustomJwtClaimType.Role, account.sys_role.name)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(StaticVariable.TokenAuthentication.SecretSercurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                StaticVariable.TokenAuthentication.Issuer,
                StaticVariable.TokenAuthentication.Issuer,
                claims,
                expires: DateTime.Now.AddDays(StaticVariable.TokenAuthentication.AccessTokenExpirationDay), //DateTime.Now.AddDays(30),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
