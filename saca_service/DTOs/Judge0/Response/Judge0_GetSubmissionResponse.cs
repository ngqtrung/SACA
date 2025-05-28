using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Service.DTO.Judge0.Response
{
    public class Judge0_GetSubmissionResponse
    {
        public string? stdout { get; set; } = null!;
        public string? time { get; set; } = null!;
        public double? memory { get; set; }
        public string? token { get; set; } = null!;
        public string? message { get; set; }
        public Judge0_GetSubmissionResponse_Status status { get; set; } = null!;
    }
    public class Judge0_GetSubmissionResponse_Status
    {
        public int id { get; set; } 
        public string description { get; set; } = null!;
    }
}
