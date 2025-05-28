using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Service.DTO.Judge0.Response
{
    public class Judge0_GetSubmissionBatchResponse
    {
        public List<Judge0_GetSubmissionResponse> submissions { get; set; } = new List<Judge0_GetSubmissionResponse>();
    }
}
