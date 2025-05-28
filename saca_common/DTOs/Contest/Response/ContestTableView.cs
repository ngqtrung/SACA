using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Contest.Response
{
    public class ContestTableView
    {
        public string id { get; set; } = null!;
        public string code { get; set; } = null!;
        public string title { get; set; } = null!;
        public int status { get; set; }
        public string? description { get; set; }
        public string? class_code { get; set; }
        public string? subject_code { get; set; }
        public DateTime start_at { get; set; }
        public DateTime end_at { get; set; }
    }
}
