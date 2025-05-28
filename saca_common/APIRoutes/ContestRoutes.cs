using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.Routes
{
    public static class ContestRoutes
    {
        public const string INDEX = "contest";
        public static class ACTION
        {
            public const string Create = "";
            public const string Update = "";
            public const string GetDetail = "{id}";
            public const string Search = "";
            public const string Delete = "{id}";
            public const string DeleteMany = "delete-many";
            public const string SubmitSolution = "submit-solution";
            public const string GetUserContests = "user-contests";
            public const string FrozenContest = "frozen-contest";
            public const string GetContestEndTime = "{id}/end-at";
            public const string GetAll = "get-all";
            public const string ImportExcel = "import";
        }
    }
}
