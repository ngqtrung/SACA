using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.TestCase.Request
{
    public class TestCaseUpdating : TestCaseCreating
    {
        public string? id { get; set; }
    }
}
