using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.Routes
{
    public class AccountRoutes
    {
        public const string INDEX = "account";

        public static class ACTION
        {
            public const string GetListMembers = "members";
            public const string GetDetail = "{id}";
            public const string Create = "add";
            public const string Update = "edit";
            public const string ResetPassword = "reset-password/{id}";
            public const string Delete = "{id}";
            public const string AddMany = "add-many";
            public const string Search = "";
            public const string ImportExcel = "import";
        }
    }
}
