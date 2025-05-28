using SACA_Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Submission.Response
{
    public class SubmissionTableView
    {
        public string id { get; set; } = null!;
        public string problem_title { get; set; } = null!;
        public DateTime submitted_at { get; set; }
        public string? file_path { get; set; }
        public string submitted_by { get; set; } = null!;
        public string submitted_by_name { get; set; } = null!;

        public int testcase_passed { get; set; }
        public int programming_language { get; set; }
        [EnumDataType(typeof(eStatus_Submission))]
        public int status { get; set; }
        public double runinng_time { get; set; }
        public double running_memory { get; set; }
        public bool is_frozen { get; set; }
        public double score { get; set; }
    }
}
