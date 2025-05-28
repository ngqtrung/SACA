using SACA_Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Submission.Request
{
    public class SubmissionTableFilter : Paging
    {
        [Required]
        public string contest_id { get; set; } = null!;
        public string? problem_id { get; set; }
        public string? keyword { get; set; } = null!;
        [EnumDataType(typeof(eStatus_Submission))]
        public int? status { get; set; }
        [EnumDataType(typeof(eType_ContestProgrammingLanguage))]
        public int? programming_language { get; set; }
    }
}
