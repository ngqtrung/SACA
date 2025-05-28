using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SACA_Common.Authentications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common
{
    public static class BaseServiceInjection
    {
        public static void AddCustomAuthorization(this IServiceCollection services, IConfiguration Configuration)
        {
            /* config bearer JWT */
            string issuer = Configuration.GetValue<string>("TokenAuthentication:Issuer") ?? "";
            string secretSercurityKey = Configuration.GetValue<string>("TokenAuthentication:SecretSercurityKey") ?? "";
            byte[] signingKeyBytes = Encoding.UTF8.GetBytes(secretSercurityKey);
            services.AddScoped<IAuthorizationEvents, AuthorizationEvents>();
            services
                   .AddAuthentication(options =>
                   {
                       options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                       options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                       options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                   })
                   .AddPolicyScheme(DynamicAuth.AuthenticationScheme, DynamicAuth.AuthenticationScheme, options =>
                   {
                       options.ForwardDefaultSelector = DynamicAuth.OnForwardDefault;
                   })
                   .AddJwtBearer(options =>
                   {
                       options.Audience = issuer;
                       options.RequireHttpsMetadata = false;
                       options.SaveToken = true;
                       options.TokenValidationParameters = new TokenValidationParameters()
                       {
                           ValidateIssuerSigningKey = true,
                           IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes),
                           ValidateIssuer = true,
                           ValidIssuer = issuer,
                           ValidateAudience = true,
                           ValidAudience = issuer,
                           ValidateLifetime = true,
                           ClockSkew = TimeSpan.Zero
                       };
                       options.EventsType = typeof(IAuthorizationEvents);
                   });
        }
    }
}
