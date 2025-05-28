using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Service.DTO.Judge0.Request
{
    public class Judge0_CreateSubmissionBatch
    {
        public List<Judge0_CreateSubmission> submissions { get; set; } = new List<Judge0_CreateSubmission>();
    }
}
