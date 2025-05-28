using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.Enums
{
    public enum eStatus_Judge0_Submission
    {
        /// <summary>
        /// Chờ chấm điểm
        /// </summary>
        [Description("In Queue")]
        InQueue = 1,

        /// <summary>
        /// Đang xử lý
        /// </summary>
        [Description("Processing")]
        Processing = 2,

        /// <summary>
        /// Chạy thành công, đúng kết quả
        /// </summary>
        [Description("Accepted")]
        Accepted = 3,

        /// <summary>
        /// Chạy sai kết quả
        /// </summary>
        [Description("Wrong Answer")]
        WrongAnswer = 4,

        /// <summary>
        /// Vượt quá thời gian cho phép
        /// </summary>
        [Description("Time Limit Exceeded")]
        TimeLimitExceeded = 5,

        /// <summary>
        /// Lỗi biên dịch
        /// </summary>
        [Description("Compilation Error")]
        CompilationError = 6,

        /// <summary>
        /// Runtime Error: Vi phạm bộ nhớ
        /// </summary>
        [Description("Runtime Error (SIGSEGV)")]
        RuntimeError_SIGSEGV = 7,

        /// <summary>
        /// Runtime Error: Vượt quá kích thước file
        /// </summary>
        [Description("Runtime Error (SIGXFSZ)")]
        RuntimeError_SIGXFSZ = 8,

        /// <summary>
        /// Runtime Error: Lỗi toán học (chia cho 0, ...)
        /// </summary>
        [Description("Runtime Error (SIGFPE)")]
        RuntimeError_SIGFPE = 9,

        /// <summary>
        /// Runtime Error: Lỗi huỷ chương trình
        /// </summary>
        [Description("Runtime Error (SIGABRT)")]
        RuntimeError_SIGABRT = 10,

        /// <summary>
        /// Runtime Error: Non-zero Exit Code
        /// </summary>
        [Description("Runtime Error (NZEC)")]
        RuntimeError_NZEC = 11,

        /// <summary>
        /// Runtime Error: Khác
        /// </summary>
        [Description("Runtime Error (Other)")]
        RuntimeError_Other = 12,

        /// <summary>
        /// Lỗi hệ thống nội bộ
        /// </summary>
        [Description("Internal Error")]
        InternalError = 13,

        /// <summary>
        /// File thực thi không hợp lệ
        /// </summary>
        [Description("Exec Format Error")]
        ExecFormatError = 14
    }

}
