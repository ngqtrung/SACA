using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.Routes
{
    public class FileRoutes
    {
        public const string INDEX = "file";
        public static class ACTION
        {
            public const string Create = "";
            public const string Update = "{id}";
            public const string Get = "{id}";
            public const string Delete = "{id}";
            public const string DownloadFile = "{id}/download";
        }
    }
}
