using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Mail.Request
{
    public class SendMailInvite
    {
        public string contest_id { get; set; } = null!;
        public List<string> account_ids { get; set; } = new List<string>();
    }
}
