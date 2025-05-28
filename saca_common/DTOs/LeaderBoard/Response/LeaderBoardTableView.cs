using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.LeaderBoard.Response
{
    public class LeaderBoardTableView : Paging
    {
        public string contest_id { get; set; } = null!;
        public bool is_fronzen { get; set; }
        public List<LeaderBoardTableView_Problem> problems { get; set; } = new List<LeaderBoardTableView_Problem>();
        public List<LeaderBoardTableView_Row> rows { get; set; } = new List<LeaderBoardTableView_Row>();
    }
    public class LeaderBoardTableView_Problem
    {
        public string id { get; set; } = null!;
        public string code { get; set; } = null!;
        public double score { get; set; }
    }
    public class LeaderBoardTableView_Row
    {
        public string user_id { get; set; } = null!;
        public string username { get; set; } = null!;
        public string fullname { get; set; } = null!;
        public double total_score { get; set; }
        public double total_penaty { get; set; }
        public List<LeaderBoardTableView_Row_ProblemDetail> details { get; set; } = new List<LeaderBoardTableView_Row_ProblemDetail>();
    }
    public class LeaderBoardTableView_Row_ProblemDetail
    {
        public string submission_id { get; set; } = null!;
        public string problem_id { get; set; } = null!;
        public double score { get; set; }
        public double frozen_score { get; set; }
        public TimeSpan complete_time { get; set; }
        public int number_of_attempts { get; set; }
        public string? plagiarism_submission_id { get; set; }
        public double? plagiarism_avg { get; set; }
        public double? plagiarism_max { get; set; }
    }
}
