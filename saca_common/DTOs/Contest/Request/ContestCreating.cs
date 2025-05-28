using SACA_Common.DTOs.Account.Request;
using SACA_Common.DTOs.Contest;
using SACA_Common.DTOs.Problem.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Contest.Request
{
    public class ContestCreating : ContestBase
    {
        //Câu hỏi
        public List<ProblemCreating> problems { get; set; } = new List<ProblemCreating>();
        //Danh sách thành viên tham gia
        public List<AccountCreating> participants { get; set; } = new List<AccountCreating>();
    }
}
