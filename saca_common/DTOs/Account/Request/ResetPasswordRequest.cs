using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Account.Request
{
    public class ResetPasswordRequest
    {
        [Required]
        public string new_password { get; set; } = null!;
        [Required]
        public string re_password { get; set; } = null!;
    }
}
