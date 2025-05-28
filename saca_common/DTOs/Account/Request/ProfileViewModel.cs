using SACA_Common.DTOs.Account.Response;
using SACA_Common.DTOs.Authenticate.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Account.Request
{
    public class ProfileViewModel
    {
        public Response<AccountView> Account { get; set; } = new();
        public ChangePasswordRequest ChangePassword { get; set; } = new ChangePasswordRequest();
    }
}
