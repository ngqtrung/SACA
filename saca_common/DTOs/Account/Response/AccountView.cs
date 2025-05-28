using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Account.Response
{
    public class AccountView
    {
        public string id { get; set; } = null!;
        public string fullname { get; set; } = null!;
        public string email { get; set; } = null!;
        public string username { get; set; } = null!;
        public string password { get; set; } = null!;
        public int status { get; set; }
        public DateTime? last_login { get; set; }
        public string role_name { get; set; } = null!;
        public string? roll_number { get; set; }
        public string? student_code { get; set; }
        public string code { get; set; } = null!;
        public bool invitation_email_sent { get; set; } = false;
    }
}
