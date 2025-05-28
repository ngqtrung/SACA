using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SACA_Common.Models;
using System.ComponentModel.DataAnnotations.Schema;
using SACA_Common.Enums;

namespace SACA_Infra.Models
{
    [Table("account_notification")]
    [Comment("Thông báo theo tài khoản")]
    public class account_notification : account_notification_properties
    {
        public notification notification { get; set; } = null!;
        public sys_account account { get; set; } = null!;
    }
    public class account_notification_properties : ExtendModel
    {
        [Comment("Thí sinh")]
        public string account_id { get; set; } = null!;
        [Comment("Thông báo")]
        public string notification_id { get; set; } = null!;
        public int status { get; set; } = (int)eStatus_AccountNotification.Unread;
    }
    public class account_notification_configuration : IEntityTypeConfiguration<account_notification>
    {
        public void Configure(EntityTypeBuilder<account_notification> builder)
        {
            builder.HasOne(e => e.notification)
                   .WithMany()
                   .HasForeignKey(e => e.notification_id);
            builder.HasOne(e => e.account)
                   .WithMany(a => a.notifications)
                   .HasForeignKey(e => e.account_id);
        }
    }
}
