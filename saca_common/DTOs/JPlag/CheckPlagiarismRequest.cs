using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.JPlag
{
    public class CheckPlagiarismRequest
    {
        public string contest_id { get; set; } = null!;
        public string? problem_id { get; set; } = null!;
        public int? programing_language { get; set; }
    }
}
