using SACA_Common.DTOs.Account.Request;
using SACA_Common.DTOs.Problem.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Contest.Request
{
    public class ContestUpdating : ContestBase
    {
        [Required]
        public string id { get; set; } = null!;
        public List<ProblemUpdating> problems { get; set; } = new List<ProblemUpdating>();
        public List<AccountUpdating> participants { get; set; } = new List<AccountUpdating>();
    }
}
