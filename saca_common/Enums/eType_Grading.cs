using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.Enums
{
    /// <summary>
    /// Loại chấm điểm
    /// </summary>
    public enum eType_Grading
    {
        /// <summary>
        /// Ngay khi nộp bài
        /// </summary>
        [Description("Ngay khi nộp bài")]
        Immediately = 0,
        /// <summary>
        /// Khi hết giờ
        /// </summary>
        [Description("Khi hết giờ")]
        AfterConclusion = 1
    }
}
