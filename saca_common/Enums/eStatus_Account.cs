using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.Enums
{
    public enum eStatus_Account
    {
        [Description("Ngưng sử dụng")]
        InActive = 0,
        [Description("Hoạt động")]
        Active = 1
    }
}
