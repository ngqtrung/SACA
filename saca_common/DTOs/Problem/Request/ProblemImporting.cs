using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Problem.Request
{
    public class ProblemImporting
    {
        [Required(ErrorMessage = "Please upload a file.")]
        [Display(Name = "Problem File")]
        public IFormFile import_file { get; set; }
    }
}
