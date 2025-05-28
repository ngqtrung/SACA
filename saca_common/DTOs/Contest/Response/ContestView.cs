using SACA_Common.DTOs.Account.Response;
using SACA_Common.DTOs.Contest;
using SACA_Common.DTOs.Problem.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Contest.Response
{
    public class ContestView : ContestBase
    {
        public string id { get; set; } = null!;
        public List<ProblemView> problems { get; set; } = new List<ProblemView>();
        public List<AccountView> participants { get; set; } = new List<AccountView>();
    }
}
