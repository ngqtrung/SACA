using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Report.ScoreBoard.Request
{
    public class GetScoreBoardRequest : Paged
    {
        public string? contest_id { get; set; }
        public string? class_code { get; set; }
        public string? subject_code { get; set; }
        public string? keyword { get; set; }
    }
}
