using SACA_Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Submission.Response
{
    public class GradingView
    {
        public string? testcase_code { get; set; }
        public string testcase_id { get; set; } = null!;
        public string input { get; set; } = null!;
        public string expected_output { get; set; } = null!;
        public string? actual_output { get; set; } = null!;
        public double running_time { get; set; }
        public double running_memory { get; set; }
        public eStatus_Judge0_Submission status { get; set; }
    }
}
