using Microsoft.EntityFrameworkCore;
using SACA_Common.DTOs.TestCase.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Problem.Response
{
    public class ProblemFormView
    {
        public string? id { get; set; } = null;
        public string? contest_id { get; set; } = null;
        [Display(Name = "Problem Code"), StringLength(25), Required]
        public string code { get; set; } = null!;
        [Display(Name = "Problem Name"), StringLength(100), Required]
        public string title { get; set; } = null!;
        [Display(Name = "Description")]
        public string? description { get; set; } = null;
        [Display(Name = "Tags"), StringLength(100)]
        public string? tags { get; set; } = null;
        [Display(Name = "Note"), StringLength(4096)]
        public string? note { get; set; } = null;
        [Display(Name = "Max attempts"), Range(1, int.MaxValue, ErrorMessage = "Max attempts must be at least 1.")]
        public int? max_attempts { get; set; } = 1;
        [Display(Name = "Score"), Required, Range(0, double.MaxValue, ErrorMessage = "Score must be at least 0.")]
        public double score { get; set; } = 0;
        [Display(Name = "Upload files")]
        public string? file_id { get; set; } = null;
        [Display(Name = "Test Set")]
        public List<TestCaseTableView> test_cases { get; set; } = new List<TestCaseTableView>();
        [Display(Name = "Time limit (ms)")]
        public double? default_execution_time { get; set; }
        [Display(Name = "Memory limit (KB)")]
        public double? default_memory_limit { get; set; }
    }
}
