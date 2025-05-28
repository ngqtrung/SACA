using Microsoft.EntityFrameworkCore;
using SACA_Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Report.Contest
{
    public class ContestInfo
    {
        [Comment("Mã cuộc thi")]
        public string code { get; set; } = null!;
        [Comment("Tiêu đề cuộc thi")]
        public string title { get; set; } = null!;
        [Comment("Mô tả")]
        public string? description { get; set; } = null;
        [Comment("Thời gian bắt đầu")]
        public DateTime start_at { get; set; }
        [Comment("Thời gian kết thúc")]
        public DateTime end_at { get; set; }
        [Comment("Mã môn học")]
        public string? subject_code { get; set; } = null;
        [Comment("Thời hạn (phút)")]
        public double duration { get; set; }
        [Comment("Loại cuộc thi")]
        public int contest_type { get; set; }
        public string contest_type_name { get; set; } = null!;
        [Comment("Cách chấm điểm")]
        public int grading_type { get; set; }
        public string grading_type_name { get; set; } = null!;
        [Comment("Bảng xếp hạng")]
        public bool leaderboard_enabled { get; set; } = false;
        [Comment("Kiểm tra đạo văn")]
        public bool plagiarism_detection_enabled { get; set; } = false;
        [Comment("Ngôn ngữ lập trình")]
        public List<int> programming_languages { get; set; } = null!;
        public List<string> programming_language_names { get; set; } = null!;
        [Comment("Thời gian phạt")]
        public double penalty_time { get; set; }
        [Comment("Trạng thái")]
        public int status { get; set; }
        public string status_name { get; set; } = null!;
    }
}
