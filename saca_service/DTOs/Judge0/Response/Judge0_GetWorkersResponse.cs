using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Service.DTO.Judge0.Response
{
    public class Judge0_GetWorkersResponse
    {
        public string queue { get; set; } = null!;
        public int size { get; set; }
        public int available { get; set; }
        public int idle { get; set; }
        public int working { get; set; }
        public int paused { get; set; }
        public int failed { get; set; }
    }
}
