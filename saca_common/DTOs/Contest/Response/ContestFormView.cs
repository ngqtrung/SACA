using Microsoft.EntityFrameworkCore;
using SACA_Common.DTOs.Account.Request;
using SACA_Common.DTOs.Account.Response;
using SACA_Common.DTOs.Problem.Request;
using SACA_Common.DTOs.Problem.Response;
using SACA_Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Contest.Response
{
    public class ContestFormView
    {
        public string id { get; set; } = null!;
        
        [Display(Name = "Contest Code")]
        [Comment("Mã cuộc thi"), Required, StringLength(25)]
        public string code { get; set; } = null!;

        [Display(Name = "Contest Name")]
        [Comment("Tiêu đề cuộc thi"), Required, StringLength(100)]
        public string title { get; set; } = null!;

        [Display(Name = "Description")]
        [Comment("Mô tả")]
        public string? description { get; set; } = null;

        [Display(Name = "Start Time")]
        [Comment("Thời gian bắt đầu"), Required]
        public DateTime start_at { get; set; } = default;

        [Display(Name = "End Time")]
        [Comment("Thời gian kết thúc"), Required]
        public DateTime end_at { get; set; } = default;

        [Display(Name = "Subject Code")]
        [Comment("Mã môn học"), StringLength(10)]
        public string? subject_code { get; set; } = null;
        [Comment("Thời hạn (phút)"), Required, Range(0, double.MaxValue, ErrorMessage = "Duration must be at least 0.")]

        [Display(Name = "Duration (minute)")]
        public double duration { get; set; }
        [Comment("Loại cuộc thi"), Required, EnumDataType(typeof(eType_Contest))]

        [Display(Name = "Contest Type")]
        public int contest_type { get; set; } = (int)eType_Contest.Normal;

        [Display(Name = "Grading Type")]
        [Comment("Cách chấm điểm"), Required, EnumDataType(typeof(eType_Grading))]
        public int grading_type { get; set; } = (int)eType_Grading.Immediately;

        [Display(Name = "Leaderboard Enabled")]
        [Comment("Bảng xếp hạng")]
        public bool leaderboard_enabled { get; set; } = true;

        [Display(Name = "Plagiarism Detection Enabled")]
        [Comment("Kiểm tra đạo văn")]
        public bool plagiarism_detection_enabled { get; set; } = true;

        [Display(Name = "Programming Language")]
        [Comment("Ngôn ngữ lập trình"), Required]
        public List<eType_ContestProgrammingLanguage> programming_languages { get; set; } = new();

        [Display(Name = "Penalty Time (minute)")]
        [Comment("Thời gian phạt")]
        public double penalty_time { get; set; } = 0;

        [Display(Name = "Status")]
        [Comment("Trạng thái")]
        public int status { get; set; } = (int)eStatus_Contest.Scheduled;
        
        [Display(Name = "Class Code")]
        [Comment("Mã lớp học")]
        public string? class_code { get; set; }

        //Câu hỏi
        public PagedResponse<ProblemTableView> problems { get; set; } = new PagedResponse<ProblemTableView>()
        {
            Items = new List<ProblemTableView>(),
            page_index = 1,
            page_size = 5,
            total_items = 0
        };
        //Danh sách thành viên tham gia
        public PagedResponse<AccountTableView> participants { get; set; } = new PagedResponse<AccountTableView>() 
        {
            Items = new List<AccountTableView>(),
            page_index = 1,
            page_size = 5,
            total_items = 0
        };
    }
}
