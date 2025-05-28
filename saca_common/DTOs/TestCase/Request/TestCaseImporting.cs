using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.TestCase.Request
{
    public class TestCaseImporting
    {
        [Required(ErrorMessage = "Please upload a file.")]
        [Display(Name = "Problem File")]
        public IFormFile import_file { get; set; }
        public double? score { get; set; }
        public double? default_execution_time { get; set; }
        public double? default_memory_limit { get; set; }

    }
}
