using SACA_Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Account.Response
{
    public class AccountFormView
    {
        public string? id { get; set; } = null;
        [Display(Name = "Fullname"), Required, MaxLength(255)]
        public string fullname { get; set; } = null!;
        [Display(Name = "Email"), Required, EmailAddress]
        public string email { get; set; } = null!;
        [Display(Name = "Username"), MaxLength(255)]
        public string? username { get; set; } = null;
        [DataType(DataType.Password)]
        [Display(Name = "Password"), MaxLength(255)]
        public string? password { get; set; } = null;
        [Display(Name = "Roll Number"), MaxLength(255), Required]
        public string roll_number { get; set; } = "";
        [Display(Name = "Student Code"), MaxLength(255), Required]
        public string student_code { get; set; } = "";
    }
}
