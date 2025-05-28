using Microsoft.EntityFrameworkCore;
using SACA_Common.DTOs.File.Response;
using SACA_Common.DTOs.TestCase.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Problem.Response
{
    public class ProblemTableView
    {
        public string id { get; set; } = null!;
        
        [Display(Name = "Code")]
        public string code { get; set; } = null!;
        [Display(Name = "Title")]
        public string title { get; set; } = null!;
        [Display(Name = "Description")]
        public string? description { get; set; }
        [Display(Name = "Tags")]
        public string? tags { get; set; }
        [Display(Name = "Score")]
        public double score { get; set; }
        [Display(Name = "Max attempts")]
        public int max_attempts { get; set; }

        //CAUTION: NOT DISPLAY ON THE TABLE
        public string? contest_id { get; set; } = null;
        public string? note { get; set; }
        public List<TestCaseTableView> test_cases { get; set; } = new List<TestCaseTableView>();
        public double? default_execution_time { get; set; }
        public double? default_memory_limit { get; set; }
        public string? file_id { get; set; }
        public SacaFileView? file { get; set; }
    }
}
