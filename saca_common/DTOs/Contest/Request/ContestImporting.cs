using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Contest.Request
{
    public class ContestImporting
    {
        [Required(ErrorMessage = "Please upload a file.")]
        public IFormFile import_file { get; set; }
    }
}
