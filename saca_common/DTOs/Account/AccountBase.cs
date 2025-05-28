using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Account
{
    public class AccountBase
    {
        [Comment("Họ và tên"), Required, MaxLength(255)]
        public string fullname { get; set; } = null!;
        [Comment("Email"), Required, EmailAddress]
        public string email { get; set; } = null!;
        [Comment("Tên người dùng"), Required, MaxLength(255)]
        public string username { get; set; } = null!;
        [Comment("Mật khẩu"), Required, MaxLength(255)]
        public string password { get; set; } = null!;
        [Comment("Mã số sinh viên"), StringLength(255)]
        public string? roll_number { get; set; }
        public string? student_code { get; set; }
    }
}
