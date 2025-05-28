using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Org.BouncyCastle.Asn1.Mozilla;
using SACA_Common.Enums;
using SACA_Common.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SACA_Infra.Models
{
    [Table("problem_submission")]
    [Comment("Bài nộp")]
    public class problem_submission : problem_submission_properties
    {
        public problem problem { get; set; } = null!;
        public sys_account submitter { get; set; } = null!;
        public ICollection<submission_grading> submission_gradings { get; set; } = new List<submission_grading>();
    }
    public class problem_submission_properties : ExtendModel
    {
        [Comment("Câu hỏi")]
        public string problem_id { get; set; } = null!;
        [Comment("Người nộp")]
        public string account_id { get; set; } = null!;
        [Comment("Source code")]
        public string? source_code { get; set; }
        [Comment("Loại ngôn ngữ lập trình")]
        public int programming_language { get; set; }
        [Comment("Thời gian nộp bài")]
        public DateTime submitted_at { get; set; }
        [Comment("Đường dẫn file nộp")]
        public string? file_path { get; set; }
        [EnumDataType(typeof(eStatus_Submission))]
        public int status { get; set; }
        public double runinng_time { get; set; }
        public double running_memory { get; set; }
        public double scrore { get; set; }
    }
    public class problem_submission_configuration : IEntityTypeConfiguration<problem_submission>
    {
        public void Configure(EntityTypeBuilder<problem_submission> builder)
        {
            builder.HasOne(e => e.problem)
                   .WithMany(p => p.submissions)
                   .HasForeignKey(e => e.problem_id);
            builder.HasOne(e => e.submitter)
                   .WithMany(a => a.submissions)
                   .HasForeignKey(e => e.account_id);
        }
    }
}
