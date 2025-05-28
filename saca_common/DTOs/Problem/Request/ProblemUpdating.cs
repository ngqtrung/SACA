using SACA_Common.DTOs.TestCase.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Problem.Request
{
    public class ProblemUpdating : ProblemCreating
    {
        public string? id { get; set; }
        new public List<TestCaseUpdating> test_cases { get; set; } = new List<TestCaseUpdating>();
    }
}
