using SACA_Common.DTOs.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Account.Request
{
    public class AccountAddMany
    {
        [Required]
        public List<AccountCreating> accounts { get; set; } = new List<AccountCreating>();
    }
}
