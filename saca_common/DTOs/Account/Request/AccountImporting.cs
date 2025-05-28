using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Account.Request
{
    public class AccountImporting
    {
        [Required(ErrorMessage = "Please upload a file.")]
        [Display(Name = "Attendance File")]
        public IFormFile import_file { get; set; }
    }
}
