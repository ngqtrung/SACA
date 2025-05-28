using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.TestCase.Response
{
    public class TestCaseFormView
    {
        public string? id { get; set; } = null;
        public string? problem_id { get; set; } = null;
        [Display(Name = "Description")]
        public string? description { get; set; } = null;
        [Display(Name = "Test Code"), Required]
        public string code { get; set; } = null!;
        [Display(Name = "Score"), Range(0, double.MaxValue)]
        public double score { get; set; } = 0;
        [Display(Name = "Input (M)"), Required]
        public string input { get; set; } = null!;
        [Display(Name = "Output (M)"), Required]
        public string output { get; set; } = null!;
        [Display(Name = "Testcase type"), Required]
        public int testcase_type { get; set; } = 1;
        [Display(Name = "Order")]
        public int order { get; set; } = 0;
        [Display(Name = "Time limit (ms)"), Required, Range(0, double.MaxValue)]
        public double execution_time { get; set; } = 0;
        [Display(Name = "Memory limit (KB)"), Required, Range(0, double.MaxValue)]
        public double memory_limit { get; set; } = 0;
    }
}
