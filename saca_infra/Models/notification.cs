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
    [Table("notification")]
    [Comment("Thông báo")]
    public class notification : notification_properties
    {
        public contest contest { get; set; } = null!;
        public problem problem { get; set; } = null!;
    }
    public class notification_properties : ExtendModel
    {
        [Comment("Tiêu đề thông báo")]
        public string title { get; set; } = null!;
        [Comment("Nội dung")]
        public string description { get; set; } = null!;
        [Comment("Id cuộc thi")]
        public string contest_id { get; set; } = null!;
        public string problem_id { get; set; } = null!;
    }
    public class notification_configuration : IEntityTypeConfiguration<notification>
    {
        public void Configure(EntityTypeBuilder<notification> builder)
        {
            builder.HasOne(e => e.contest)
                   .WithMany(c => c.notifications)
                   .HasForeignKey(e => e.contest_id);
            builder.HasOne(e => e.problem)
                   .WithOne(c => c.notification)
                   .HasForeignKey<notification>(e => e.problem_id);
        }
    }
}
