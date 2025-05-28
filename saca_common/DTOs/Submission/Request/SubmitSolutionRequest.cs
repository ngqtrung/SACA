using Microsoft.AspNetCore.Http;
using SACA_Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Submission.Request
{
    public class SubmitSolutionRequest
    {
        [Required]
        public string problem_id { get; set; } = null!;
        [EnumDataType(typeof(eType_ContestProgrammingLanguage)), Required]
        public int programming_language { get; set; }
        public string? source_code { get; set; }
        //public string? client_file_path { get; set; }
        public IFormFile? file { get; set; }
    }
}
