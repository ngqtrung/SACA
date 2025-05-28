using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.Authentications
{
    public static class CustomJwtClaimType
    {
        public const string UserId = "userid";
        public const string Username = "username";
        public const string Fullname = "fullname";
        public const string Role = "role";
        public const string Email = "email";
        public const string Exp = "expire";
    }
}
