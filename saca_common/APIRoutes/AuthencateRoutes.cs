using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.Routes
{
    public static class AuthenticateRoutes
    {
        public const string INDEX = "authen";
        public static class ACTION
        {
            public const string Login = "login";
            public const string ChangePassword = "change-password";
        }
    }
}
