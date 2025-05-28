using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.Enums
{
    public enum eType_TimeUnit
    {
        /// <summary>
        /// ms
        /// </summary>
        [Description("ms")]
        MS = 0,

        /// <summary>
        /// sec
        /// </summary>
        [Description("sec")]
        SEC = 1,

        /// <summary>
        /// min
        /// </summary>
        [Description("min")]
        MIN = 2,

        /// <summary>
        /// hour
        /// </summary>
        [Description("hour")]
        HOUR = 3,

        /// <summary>
        /// day
        /// </summary>
        [Description("day")]
        DAY = 4,
    }
}
