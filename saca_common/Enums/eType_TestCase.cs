using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.Enums
{
    public enum eType_TestCase
    {
        /// <summary>
        /// Sample
        /// </summary>
        [Description("Sample")]
        Sample = 1,

        /// <summary>
        /// Hidden
        /// </summary>
        [Description("Hidden")]
        Hidden = 0
    }
}
