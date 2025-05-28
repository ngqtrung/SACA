using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.Routes
{
    public class SubmissionRoutes
    {
        public const string INDEX = "submission";
        public static class ACTION
        {
            public const string SubmitSolution = "";
            public const string ResubmitSolution = "resubmit-solution";
            public const string GetDetail = "{id}";
            public const string Search = "search";
            public const string Delete = "{id}";
            public const string DeleteMany = "delete-many";
        }
    }
}
