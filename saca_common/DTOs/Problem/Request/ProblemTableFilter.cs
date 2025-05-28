
using System.ComponentModel.DataAnnotations;

namespace SACA_Common.DTOs.Problem.Request
{
    public class ProblemTableFilter : Paging
    {
        public string? keyword { get; set; }
        public string? contest_id { get; set; }
    }
}
