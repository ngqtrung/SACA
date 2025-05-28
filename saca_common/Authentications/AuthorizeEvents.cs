using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.Authentications
{
    public interface IAuthorizationEvents
    {
    }

    public class AuthorizationEvents : JwtBearerEvents, IAuthorizationEvents
    {
        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            return base.AuthenticationFailed(context);
        }

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            context.HttpContext.User = context.Principal!;
            await base.TokenValidated(context);
        }
    }
}
