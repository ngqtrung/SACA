using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.Enums
{
    public enum eStatus_AccountNotification
    {
        [Description("Chưa đọc")]
        Unread = 0,
        [Description("Đã đọc")]
        Read = 1
    }
}
