using SACA_Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Account.Request
{
    public class MemberTableFilter : Paging
    {
        public string? contest_id { get; set; }
        public string? keyword { get; set; }
    }
}
