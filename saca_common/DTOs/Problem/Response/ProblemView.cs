using SACA_Common.DTOs.File.Response;
using SACA_Common.DTOs.Problem;
using SACA_Common.DTOs.TestCase.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Problem.Response
{
    public class ProblemView : ProblemBase
    {
        public string id { get; set; } = null!;
        public List<TestCaseTableView> test_cases { get; set; } = new List<TestCaseTableView>();
        public SacaFileView? file { get; set; }
    }
}
