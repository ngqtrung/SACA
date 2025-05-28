using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.LeaderBoard.Request
{
    public class LeaderBoardTableFilter : Paging
    {
        public string contest_id { get; set; } = null!;
        public bool get_all { get; set; } = false;
    }
}
