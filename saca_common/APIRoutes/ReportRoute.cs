using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.Routes
{
    public static class ReportRoute
    {
        public const string INDEX = "report";
        public static class ACTION
        {
            public const string Contest = "contest";
            public const string ContestProblems = "contest-problems";
            public const string LeaderBoard = "leader-board";
            public const string ContestParticipants = "contest-participants";
            public const string ScoreBoard = "score-board";
            public const string ExportScoreBoard = "export-score-board";
            public const string ScoreDistribution = "score-distribution";
        }
    }
}
