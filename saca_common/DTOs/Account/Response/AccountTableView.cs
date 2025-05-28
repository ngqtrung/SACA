using Microsoft.EntityFrameworkCore;
using SACA_Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Account.Response
{
    public class AccountTableView
    {
        [Display(Name = "Member Code")]
        public string? id { get; set; } = null;
        [Display(Name = "Status")]
        [EnumDataType(typeof(eStatus_Account))]
        public int? status { get; set; }
        [Display(Name = "Username")]
        public string username { get; set; } = null!;
        [Display(Name = "Email")]
        public string email { get; set; } = null!;
        [Display(Name = "Fullname")]
        public string fullname { get; set; } = null!;
        [Display(Name = "Last Login")]
        public DateTime? last_login { get; set; }
        public string role_id { get; set; } = null!;
        [Display(Name = "Roll Number")]
        public string? roll_number { get; set; } = null!;
        [Display(Name = "Invitation Email Sent")]
        public bool invitation_email_sent { get; set; }
        //CAUTION: NOT DISPLAY ON THE TABLE
        public string? student_code { get; set; } = null!;
        public string password { get; set; } = null!;
        
    }
}
