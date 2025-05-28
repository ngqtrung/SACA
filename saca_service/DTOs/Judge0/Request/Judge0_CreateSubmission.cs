using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Service.DTO.Judge0.Request
{
    public class Judge0_CreateSubmission
    {
        public string source_code { get; set; } = null!;
        public int language_id { get; set; }
        public string stdin { get; set; } = null!;
        public string expected_output { get; set; } = null!;
        public double? memory_limit { get; set; }
        public double? cpu_time_limit { get; set; }
        // Extend
    }
}
