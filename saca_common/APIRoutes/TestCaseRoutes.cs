using Org.BouncyCastle.Pqc.Crypto.Bike;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.Routes
{
    public class TestCaseRoutes
    {
        public const string INDEX = "testcase";
        public static class ACTION
        {
            public const string Create = "";
            public const string Update = "";
            public const string GetDetail = "{id}";
            public const string Search = "";
            public const string Delete = "{id}";
            public const string DeleteMany = "delete-many";
            public const string ImportExcel = "import";
        }
    }
}
