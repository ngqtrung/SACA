using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SACA_Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Infra.Models
{
    [Table("submission_grading")]
    public class submission_grading : submission_grading_properties
    {
        public problem_submission problem_submission { get; set; } = null!;
        public test_case test_case { get; set; } = null!;
    }
    public class submission_grading_properties : ExtendModel
    {
        public string problem_submission_id { get; set; } = null!;
        public string test_case_id { get; set; } = null!;
        public int status { get; set; }
        public double runinng_time { get; set; }
        public double running_memory { get; set; }
        public string? actual_output { get; set; }
        public string? judge0_token { get; set; }
    }
    public class submission_grading_configuration : IEntityTypeConfiguration<submission_grading>
    {
        public void Configure(EntityTypeBuilder<submission_grading> builder)
        {
            builder.HasOne(e => e.problem_submission)
                   .WithMany(e => e.submission_gradings)
                   .HasForeignKey(e => e.problem_submission_id);
            builder.HasOne(e => e.test_case)
                   .WithMany(e => e.submission_grading)
                   .HasForeignKey(e => e.test_case_id);
        }
    }
}
