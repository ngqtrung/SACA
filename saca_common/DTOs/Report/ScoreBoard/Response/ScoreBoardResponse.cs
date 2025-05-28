using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Report.ScoreBoard.Response
{
    public class ScoreBoardResponse
    {
        public string fullname { get; set; } = null!;
        public string email { get; set; } = null!;
        public string? roll_number { get; set; }
        public double score { get; set; }
    }
}
