using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Report.ScoreDistribution.Request
{
    public class GetScoreDistributionRequest
    {
        public string contest_id { get; set; } = null!;
        public List<string> problem_ids { get; set; } = new List<string>();
    }
}
