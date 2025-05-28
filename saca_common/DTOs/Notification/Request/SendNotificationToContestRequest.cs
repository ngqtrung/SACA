using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Notification.Request
{
    public class SendNotificationToContestRequest
    {
        public string contest_id { get; set; } = null!;
        public string notification_id { get; set; } = null!;
    }
}
