using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Notification
{
    public class NotificationBase
    {
        [Description("Tiêu đề thông báo"), StringLength(100), Required]
        public string title { get; set; } = null!;
        [Description("Nội dung thông báo"), StringLength(1000), Required]
        public string description { get; set; } = null!;
        public string contest_code { get; set; } = null!;
        public string problem_name { get; set; } = null!;
    }
}
