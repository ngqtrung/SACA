using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SACA_Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Infra.Models
{
    public class best_submission : best_submission_properties
    {
        public problem problem { get; set; } = null!;
        public problem_submission problem_submission { get; set; } = null!;
        public sys_account sys_account { get; set; } = null!;
    }
    public class best_submission_properties : ExtendModel
    {
        public string user_id { get; set; } = null!;
        public string problem_id { get; set; } = null!;
        public string problem_submission_id { get; set; } = null!;
        public double score { get; set; }
        public double frozen_score { get; set; }
        public DateTime submited_at { get; set; }
        public double running_time { get; set; }
        public double running_memory { get; set; }
        public int number_of_attempts { get; set; }
        public string? plagiarism_submission_id { get; set; }
        public double? plagiarism_avg { get; set; }
        public double? plagiarism_max { get; set; }
    }
    public class best_submission_configuration : IEntityTypeConfiguration<best_submission>
    {
        public void Configure(EntityTypeBuilder<best_submission> builder)
        {
            builder.HasOne(e => e.problem)
                   .WithMany(e => e.best_submissions)
                   .HasForeignKey(e => e.problem_id);
            builder.HasOne(e => e.sys_account)
                   .WithMany(e => e.best_submissions)
                   .HasForeignKey(e => e.user_id);
            builder.HasOne(e => e.problem_submission)
                   .WithMany()
                   .HasForeignKey(e => e.problem_submission_id);
        }
    }
}
