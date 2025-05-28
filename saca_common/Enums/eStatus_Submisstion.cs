using System.ComponentModel;

namespace SACA_Common.Enums
{
    public enum eStatus_Submission
    {
        /// <summary>
        /// Chờ chấm điểm
        /// </summary>
        [Description("In Queue")]
        InQueue = 0,

        /// <summary>
        /// Đang chạy
        /// </summary>
        [Description("Running")]
        Running = 1,

        /// <summary>
        /// Chạy thành công, đúng kết quả
        /// </summary>
        [Description("Accepted")]
        Accepted = 2,

        /// <summary>
        /// Chạy sai kết quả
        /// </summary>
        [Description("Wrong Answer")]
        WrongAnswer = 3,

        /// <summary>
        /// Chạy bị lỗi biên dịch
        /// </summary>
        [Description("Compile Error")]
        CompileError = 4,

        /// <summary>
        /// Chạy bị lỗi runtime (Ví dụ: chia cho 0, truy cập mảng ngoài phạm vi, ...)
        /// </summary>
        [Description("Runtime Error")]
        RuntimeError = 5,

        /// <summary>
        /// Chạy vượt quá thời gian cho phép
        /// </summary>
        [Description("Time Limit Exceeded")]
        TimeLimitExceeded = 6,

        /// <summary>
        /// Chạy vượt quá bộ nhớ cho phép
        /// </summary>
        [Description("Memory Limit Exceeded")]
        MemoryLimitExceeded = 7,

        /// <summary>
        /// Chạy đúng một phần nhưng chưa đạt kết quả tối ưu
        /// </summary>
        [Description("Partial Accepted")]
        PartialAccepted = 8,

        /// <summary>
        /// Lỗi hệ thống khi chấm bài
        /// </summary>
        [Description("System Error")]
        SystemError = 9
    }
}
