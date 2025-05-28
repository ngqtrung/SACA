using Microsoft.EntityFrameworkCore;
using SACA_Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace SACA_Common.DTOs.TestCase
{
    public class TestCaseBase
    {

        [Comment("Id Câu hỏi")]
        public string? problem_id { get; set; } = null;

        [Comment("Mô tả")]
        public string? description { get; set; } = null;
        [Comment("Mã test case"), Required, StringLength(25)]
        public string code { get; set; } = null!;
        [Comment("Điểm"), Required]
        public double score { get; set; } = 0;
        [Comment("Dữ liệu nhận vào"), Required]
        public string input { get; set; } = null!;
        [Comment("Kết quả trả ra"), Required]
        public string output { get; set; } = null!;
        [Comment("Loại test case"), Required, EnumDataType(typeof(eType_TestCase))]
        public int testcase_type { get; set; }
        [Comment("Thứ tự chạy test case"), Required, Range(0, int.MaxValue)]
        public int order { get; set; }
        [Comment("Giới hạn thời gian chạy (ms)"), Range(0, double.MaxValue)]
        public double? execution_time { get; set; } = null;
        [Comment("Giới hạn bộ nhớ (KB)"), Range(0, double.MaxValue)]
        public double? memory_limit { get; set; } = null;
    }
}
