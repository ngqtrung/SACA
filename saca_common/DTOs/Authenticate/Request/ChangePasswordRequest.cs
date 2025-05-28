using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Authenticate.Request
{
    public class ChangePasswordRequest
    {
        public string old_password { get; set; } = null!;
        public string new_password { get; set; } = null!;
        public string re_password { get; set; } = null!;
    }
}
