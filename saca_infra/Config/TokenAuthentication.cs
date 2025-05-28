using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Infra.Config
{
    public class TokenAuthentication
    {
        public string Issuer { get; set; } = null!;
        public string SecretSercurityKey { get; set; } = null!;
        public int AccessTokenExpirationDay { get; set; }
    }
}
