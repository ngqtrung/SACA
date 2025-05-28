using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.DTOs.Report.Contest
{
    public class ProblemInfo
    {
        [Description("Mã câu hỏi")]
        public string code { get; set; } = null!;
        [Description("Tiêu đề câu hỏi")]
        public string title { get; set; } = null!;
        [Description("Mô tả")]
        public string? description { get; set; } = null;
        [Description("Danh sách tag")]
        public string? tags { get; set; } = null;
        [Description("Ghi chú")]
        public string? note { get; set; } = null;
        [Description("Giới hạn số lần nộp bài")]
        public int? max_attempts { get; set; } = null;
        [Description("Số điểm của câu hỏi")]
        public double score { get; set; }
        public double? default_execution_time { get; set; }
        public double? default_memory_limit { get; set; }
    }
}
