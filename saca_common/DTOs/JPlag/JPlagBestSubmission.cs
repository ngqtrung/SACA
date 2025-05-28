using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.JPlag
{
    public class JPlagBestSubmission
    {
        public string src_code { get; set; } = null!;
        public string account_id { get; set; } = null!;
        public string username { get; set; } = null!;
        public string fullname { get; set; } = null!;
    }
}
