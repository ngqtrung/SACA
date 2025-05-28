using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SACA_Common.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace SACA_Infra.Models
{
    [Table("testcase")]
    [Comment("Test case của câu hỏi")]
    public class test_case : test_case_properties
    {
        public problem problem { get; set; } = null!;
        public ICollection<submission_grading> submission_grading { get; set; } = null!;
    }
    public class test_case_properties : ExtendModel
    {
        [Comment("Câu hỏi")]
        public string problem_id { get; set; } = null!;
        [Comment("Mô tả")]
        public string? description { get; set; }
        [Comment("Mã test case")]
        public string code { get; set; } = null!;
        [Comment("Điểm")]
        public double score { get; set; }
        [Comment("Dữ liệu nhận vào")]
        public string input { get; set; } = null!;
        [Comment("Kết quả trả ra")]
        public string output { get; set; } = null!;
        [Comment("Loại test case")]
        public int testcase_type { get; set; }
        [Comment("Thứ tự chạy test case")]
        public int order { get; set; }
        [Comment("Giới hạn thời gian chạy (ms)")]
        public double? execution_time { get; set; }
        [Comment("Giới hạn bộ nhớ (KB)")]
        public double? memory_limit { get; set; }
    }
    public class test_case_configuration : IEntityTypeConfiguration<test_case>
    {
        public void Configure(EntityTypeBuilder<test_case> builder)
        {
            builder.HasOne(e => e.problem)
                   .WithMany(p => p.test_cases)
                   .HasForeignKey(e => e.problem_id);
        }
    }
}
