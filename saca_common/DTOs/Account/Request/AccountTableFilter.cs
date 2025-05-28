using SACA_Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Account.Request
{
    public class AccountTableFilter : Paging
    {
        public string? keyword { get; set; }
        [EnumDataType(typeof(eStatus_Account))]
        public int? status { get; set; }
    }
}
