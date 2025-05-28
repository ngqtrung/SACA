using SACA_Common.Configurations;
using SACA_Infra.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Infra.Const
{
    public static class StaticVariable
    {
        public static TokenAuthentication TokenAuthentication { get; set; } = AppSettings.Get<TokenAuthentication>("TokenAuthentication");
        public static MailConfig EmailConfig { get; set; } = AppSettings.Get<MailConfig>("MailConfig");
    }
}
