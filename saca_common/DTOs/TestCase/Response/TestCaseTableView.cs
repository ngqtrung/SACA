using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.TestCase.Response
{
    public class TestCaseTableView
    {
        public string? id { get; set; } = null!;
        public string? problem_id { get; set; } = null!;
        [Display(Name = "Description")]
        public string? description { get; set; } = null;
        [Display(Name = "Test Code")]
        public string code { get; set; } = null!;
        [Display(Name = "Score")]
        public double score { get; set; }
        [Display(Name = "Input (M)")]
        public string input { get; set; } = null!;
        [Display(Name = "Output (M)")]
        public string output { get; set; } = null!;
        [Display(Name = "Testcase type")]
        public int testcase_type { get; set; } = 0;
        [Display(Name = "Order")]
        public int order { get; set; } = 0;
        [Display(Name = "Time limit (ms)")]
        public double? execution_time { get; set; }
        [Display(Name = "Memory limit (KB)")]
        public double? memory_limit { get; set; }
    }
}
