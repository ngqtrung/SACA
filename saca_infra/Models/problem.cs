using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SACA_Common.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace SACA_Infra.Models
{
    [Table("problem")]
    [Comment("Câu hỏi")]
    public class problem : problem_properties
    {
        public contest contest { get; set; } = null!;
        public notification notification { get; set; } = null!;
        public saca_file file { get; set; } = null!;
        public ICollection<test_case> test_cases { get; set; } = new List<test_case>();
        public ICollection<problem_submission> submissions { get; set; } = new List<problem_submission>();
        public ICollection<best_submission> best_submissions { get; set; } = new List<best_submission>();
    }
    public class problem_properties : ExtendModel
    {
        [Comment("Id cuộc thi")]
        public string contest_id { get; set; } = null!;
        [Comment("Mã câu hỏi")]
        public string code { get; set; } = null!;
        [Comment("Tiêu đề câu hỏi")]
        public string title { get; set; } = null!;
        [Comment("Mô tả")]
        public string? description { get; set; }
        [Comment("Danh sách tag")]
        public string? tags { get; set; }
        [Comment("Ghi chú")]
        public string? note { get; set; }
        [Comment("Giới hạn số lần nộp bài")]
        public int? max_attempts { get; set; }
        [Comment("Số điểm của câu hỏi")]
        public double score { get; set; }
        [Comment("Tài nguyên")]
        public string? file_id { get; set; }
        [Comment("Giới hạn thời gian chạy (ms)")]
        public double? default_execution_time { get; set; }
        [Comment("Giới hạn bộ nhớ (KB)")]
        public double? default_memory_limit { get; set; }
    }
    public class problem_configuration : IEntityTypeConfiguration<problem>
    {
        public void Configure(EntityTypeBuilder<problem> builder)
        {
            builder.HasOne(e => e.contest)
                   .WithMany(c => c.problems)
                   .HasForeignKey(e => e.contest_id);
            builder.HasOne(e => e.file)
                .WithOne()
                .HasForeignKey<problem>(e => e.file_id)
                .IsRequired(false);
        }
    }
}
