using Microsoft.EntityFrameworkCore;
using SACA_Common.DTOs.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Account.Request
{
    public class AccountUpdating : AccountBase
    {
        public string? id { get; set; }
        //Override Required attribute from AccountBase
        [Comment("Tên người dùng"), MaxLength(255)]
        public new string? username { get; set; } = null;

        [Comment("Mật khẩu"), MaxLength(255)]
        public new string? password { get; set; } = null;
    }
}
