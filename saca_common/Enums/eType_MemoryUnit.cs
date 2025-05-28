using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.Enums
{
    public enum eType_MemoryUnit
    {
        /// <summary>
        /// KB
        /// </summary>
        [Description("KB")]
        KB = 0,

        /// <summary>
        /// MB
        /// </summary>
        [Description("MB")]
        MB = 1,

        /// <summary>
        /// GB
        /// </summary>
        [Description("GB")]
        GB = 2
    }
}
