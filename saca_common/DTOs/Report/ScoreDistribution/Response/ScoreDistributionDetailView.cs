using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Report.ScoreDistribution.Response
{
    public class ScoreDistributionDetailView
    {
        public double score { get; set; }
        public int count { get; set; }
        public List<ScoreDistributionParticipant> participants { get; set; } = new List<ScoreDistributionParticipant>();
    }
    public class ScoreDistributionParticipant
    {
        public string? full_name { get; set; }
        public string? email { get; set; }
        public string? roll_number { get; set; }
    }
}
