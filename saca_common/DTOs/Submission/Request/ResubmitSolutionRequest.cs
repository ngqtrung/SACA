using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Submission.Request
{
    public class ResubmitSolutionRequest
    {
        public List<string> submissionIds { get; set; } = new List<string>();
        public string contestId { get; set; } = null!;
    }
}
