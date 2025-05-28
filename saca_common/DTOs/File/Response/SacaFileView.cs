
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.File.Response
{
    public class SacaFileView
    {
        public string id { get; set; } = null!;
        public string parent_id { get; set; } = null!;
        public string name { get; set; } = null!;
        public string extension { get; set; } = null!;
        public string path { get; set; } = null!;
        public long length { get; set; }
    }
}
