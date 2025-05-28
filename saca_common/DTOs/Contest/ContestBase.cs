using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SACA_Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Contest
{
    public class ContestBase
    {
        [Comment("Mã cuộc thi"), Required, StringLength(25)]
        public string code { get; set; } = null!;
        [Comment("Tiêu đề cuộc thi"), Required, StringLength(100)]
        public string title { get; set; } = null!;
        [Comment("Mô tả")]
        public string? description { get; set; } = null;
        [Comment("Thời gian bắt đầu"), Required]
        public DateTime start_at { get; set; }
        [Comment("Thời gian kết thúc"), Required]
        public DateTime end_at { get; set; }
        [Comment("Mã môn học"), StringLength(50)]
        public string? subject_code { get; set; } = null;
        [Comment("Thời hạn (phút)"), Required, Range(0, double.MaxValue)]
        public double duration { get; set; }
        [Comment("Loại cuộc thi"), Required, EnumDataType(typeof(eType_Contest))]
        public int contest_type { get; set; }
        [Comment("Cách chấm điểm"), Required, EnumDataType(typeof(eType_Grading))]
        public int grading_type { get; set; }
        [Comment("Bảng xếp hạng")]
        public bool leaderboard_enabled { get; set; } = false;
        [Comment("Kiểm tra đạo văn")]
        public bool plagiarism_detection_enabled { get; set; } = false;
        [Comment("Ngôn ngữ lập trình")]
        public List<eType_ContestProgrammingLanguage> programming_languages { get; set; } = null!;
        [Comment("Thời gian phạt")]
        public double penalty_time { get; set; }
        [Comment("Trạng thái")]
        public int status { get; set; }
        [Comment("Lớp học")]
        public string? class_code { get; set; }
    }
}
