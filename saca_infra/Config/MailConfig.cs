using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Infra.Config
{
    public class MailConfig
    {
        public string Email { get; set; } = null!;
        public string Password { get;set; } = null!;
        public int SmtpClient_Port { get; set; }
        public string SmtpClient_Host { get; set; } = null!;
        public string Fullname { get; set; } = null!;
    }
}
