using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Contest.Request
{
    public class ExportContestRequest
    {
        [Required, MinLength(1)]
        public List<string> contest_ids { get; set; } = new List<string>();
    }
}
