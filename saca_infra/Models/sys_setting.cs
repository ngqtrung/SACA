using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using SACA_Common.Models;

namespace SACA_Infra.Models
{
    [Table("sys_setting")]
    [Comment("Cấu hình hệ thống")]

    public class sys_setting
    {
        // Config File
        [Comment("Cấu hình thư mục lưu file")]
        public string? config_file_directory { get; set; }

        [Comment("Xóa file cứng hay mềm (true = xóa mềm, false = xóa cứng)")]
        public bool? config_file_delete_soft { get; set; }

        // Config Mail
        [Comment("Email gửi mail")]
        public string? config_mail_email { get; set; }

        [Comment("Mật khẩu email")]
        public string? config_mail_password { get; set; }

        [Comment("SMTP Client Port")]
        public int? config_mail_smtp_port { get; set; }

        [Comment("SMTP Client Host")]
        public string? config_mail_smtp_host { get; set; }

        [Comment("Tên hiển thị email")]
        public string? config_mail_fullname { get; set; }

        // Config Google Drive
        [Comment("Google Drive Client ID")]
        public string? config_google_drive_client_id { get; set; }

        [Comment("Google Drive Client Secret")]
        public string? config_google_drive_client_secret { get; set; }

        // Config Database
        [Comment("Chế độ chỉ đọc database (true = readonly)")]
        public bool? config_database_readonly { get; set; }

        // Config Debug
        [Comment("Bật/tắt chế độ debug")]
        public bool? config_debug_mode { get; set; }

        // Config Judge0
        [Comment("Judge0 API URL")]
        public string? config_judge0_api_url { get; set; }

        // Config User Management
        [Comment("Xóa người dùng sau khi kết thúc kỳ thi bao nhiêu giờ")]
        public int? config_user_delete_after_exam_hours { get; set; }

        [Comment("Cho phép reset tài khoản sinh viên (true = có)")]
        public bool? config_user_reset_student_enabled { get; set; }
    }
}
