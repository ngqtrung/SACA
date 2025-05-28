using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Authenticate.Response
{
    public class LoginResponse
    {
        public string user_id { get; set; } = null!;
        public string username { get; set; } = null!;
        public string fullname { get; set; } = null!;
        public string email { get; set; } = null!;
        public string token { get; set; } = null!;
        public string role { get; set; } = null!;
    }
}
