using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.Routes
{
    public static class GradingMachineRoutes
    {
        public const string INDEX = "grading-machine";

        public static class ACTION
        {
            public const string GetInfo = "info";
        }
    }
}
