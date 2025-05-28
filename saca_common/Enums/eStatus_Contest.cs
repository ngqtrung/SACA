using System.ComponentModel;

namespace SACA_Common.Enums
{
    public enum eStatus_Contest
    {

        /// <summary>
        /// Đã lên lịch
        /// </summary>
        [Description("Đã lên lịch")]
        Scheduled = 0,

        /// <summary>
        /// Đang diễn ra
        /// </summary>
        [Description("Đang diễn ra")]
        OnGoing = 1,

        /// <summary>
        /// Đã đóng nộp bài
        /// </summary>
        [Description("Đã đóng nộp bài")]
        ClosedForSubmission = 2,

        /// <summary>
        /// Đang chấm điểm
        /// </summary>
        [Description("Đang chấm điểm")]
        Grading = 3,

        /// <summary>
        /// Đã hoàn thân
        /// </summary>
        [Description("Đã hoàn thành")]
        Completed = 4,

        /// <summary>
        /// Đã hủy
        /// </summary>
        [Description("Đã hủy")]
        Canceled = 5,

        /// <summary>
        /// Đang chỉnh sửa
        /// </summary>
        [Description("Nháp")]
        Draft = 6
    }
}
