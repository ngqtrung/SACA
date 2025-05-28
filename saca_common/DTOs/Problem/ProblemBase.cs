using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace SACA_Common.DTOs.Problem
{
    public class ProblemBase
    {
        [Description("Id bài thi")]
        public string? contest_id { get; set; } = null;
        [Description("Mã câu hỏi"), StringLength(25), Required]
        public string code { get; set; } = null!;
        [Description("Tiêu đề câu hỏi"), StringLength(100), Required]
        public string title { get; set; } = null!;
        [Description("Mô tả")]
        public string? description { get; set; } = null;
        [Description("Danh sách tag")]
        public string? tags { get; set; } = null;
        [Description("Ghi chú")]
        public string? note { get; set; } = null;
        [Description("Giới hạn số lần nộp bài"), Range(1, int.MaxValue)]
        public int? max_attempts { get; set; } = null;
        [Description("Số điểm của câu hỏi"), Required, Range(0, double.MaxValue)]
        public double score { get; set; }
        [Description("Tài nguyên")]
        public string? file_id { get; set; } = null;
        public double? default_execution_time { get; set; }
        public double? default_memory_limit { get; set; }
    }
}
