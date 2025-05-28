using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SACA_Common.Enums;
using SACA_Common.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace SACA_Infra.Models
{
    [Table("sys_account")]
    [Comment("Tài khoản")]
    public class sys_account : sys_account_properties
    {
        public sys_role sys_role { get; set; } = null!;
        public ICollection<contest_participant> contests { get; set; } = new List<contest_participant>();
        public ICollection<problem_submission> submissions { get; set; } = new List<problem_submission>();
        public ICollection<account_notification> notifications { get; set; } = new List<account_notification>();
        public ICollection<best_submission> best_submissions { get; set; } = new List<best_submission>();
    }
    public class sys_account_properties : ExtendModel
    {
        [Comment("Trạng thái account")]
        [EnumDataType(typeof(eStatus_Account))]
        public int status { get; set; }
        [Comment("Tên đăng nhập")]
        public string username { get; set; } = null!;
        [Comment("Email")]
        public string email { get; set; } = null!;
        [Comment("Tên đầy đủ")]
        public string fullname { get; set; } = null!;
        [Comment("Mật khẩu mã hóa")]
        public string password { get; set; } = null!;
        [Comment("Mã mật khẩu")]
        public string password_salt { get; set; } = null!;
        [Comment("Số lần đăng nhập thất bại")]
        public int failed_count { get; set; }
        [Comment("Lần đăng nhập gần nhất")]
        public DateTime? last_login { get; set; }
        public string role_id { get; set; } = null!;
        public string? sys_generated_password { get; set; }
        [Comment("Mã số sinh viên")]
        public string? roll_number { get; set; }
        [Comment("Định danh sinh viên (tên + hai ký tự đầu của họ và tên đệm + mã số sinh viên)")]
        public string? student_code { get; set; }
    }
    public class sys_account_configuration : IEntityTypeConfiguration<sys_account>
    {
        public void Configure(EntityTypeBuilder<sys_account> builder)
        {
            builder.HasOne(e => e.sys_role)
                   .WithMany(e => e.sys_accounts)
                   .HasForeignKey(e => e.role_id);
        }
    }
}
