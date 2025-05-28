using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs
{
    public class DeleteManyRequest
    {
        [Required, MinLength(1)]
        public List<string> ids { get; set; } = new List<string>();
    }
}
