using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Submission.Response
{
    public class SubmissionView
    {
        public string id { get; set; } = null!;

        public int programming_language { get; set; }
        public DateTime submitted_at { get; set; }
        public string? file_path { get; set; } = null!;
        public string problem_title { get; set; } = null!;
        public string? source_code { get; set; }
        public string contest_code { get; set; } = null!;
        public string? userid { get; set; } 
        public string? username { get; set; }
        public double total_testcase { get; set; }
        public double passed_testcase { get; set; }
        public List<GradingView> gradings { get; set; } = new List<GradingView>();
    }
}
