using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Authenticate.Request
{
    public class LoginRequest
    {
        public string username { get; set; } = null!;
        public string password { get; set; } = null!;
    }
}
