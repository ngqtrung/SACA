using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Notification.Request
{
    public class NotificationCreating
    {
        public string title { get; set; } = null!;
        public string description { get; set; } = null!;
        public string contest_id { get; set; } = null!;
        public string problem_id { get; set; } = null!;
    }
}
