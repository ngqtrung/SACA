using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SACA_Common.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace SACA_Infra.Models
{
    [Table("contest_participant")]
    [Comment("Thí sinh tham gia cuộc thi")]
    public class contest_participant : contest_participant_properties
    {
        public contest contest { get; set; } = null!;
        public sys_account account { get; set; } = null!;
    }
    public class contest_participant_properties : ExtendModel
    {
        [Comment("Thí sinh")]
        public string account_id { get; set; } = null!;
        [Comment("Cuộc thi")]
        public string contest_id { get; set; } = null!;
        public bool invitation_email_sent { get; set; }
    }
    public class contest_participant_configuration : IEntityTypeConfiguration<contest_participant>
    {
        public void Configure(EntityTypeBuilder<contest_participant> builder)
        {
            builder.HasOne(e => e.contest)
                   .WithMany(c => c.contest_participants)
                   .HasForeignKey(e => e.contest_id);
            builder.HasOne(e => e.account)
                   .WithMany(a => a.contests)
                   .HasForeignKey(e => e.account_id);

        }
    }
}
