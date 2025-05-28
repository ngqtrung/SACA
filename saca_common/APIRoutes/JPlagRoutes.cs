using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.APIRoutes
{
    public class JPlagRoutes
    {
        public const string INDEX = "jplag";
        public static class Action
        {
            public const string CheckPlagiarism = "check-plagiarism";
            public const string CheckPlagiarismByContestId = "check-plagiarism-by-contest-id";
        }
    }
}
