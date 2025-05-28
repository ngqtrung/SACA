using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.Routes
{
    public static class MailRoutes
    {
        public const string INDEX = "mail";

        public static class ACTION
        {
            public const string SendMaiInvite = "send-mail-invite";
        }
    }
}
