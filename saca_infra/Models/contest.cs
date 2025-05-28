using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SACA_Common.Enums;
using SACA_Common.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Emit;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SACA_Infra.Models
{
    [Table("contest")]
    [Comment("Cuộc thi")]
    public class contest : contest_properties
    {
        public ICollection<problem> problems { get; set; } = new List<problem>();
        public ICollection<notification> notifications { get; set; } = new List<notification>();
        public ICollection<contest_participant> contest_participants { get; set; } = new List<contest_participant>();
    }
    public class contest_properties : ExtendModel
    {
        [Comment("Mã cuộc thi")]
        public string code { get; set; } = null!;
        [Comment("Tiêu đề cuộc thi")]
        public string title { get; set; } = null!;
        [Comment("Mô tả")]
        public string? description { get; set; }
        [Comment("Thời gian bắt đầu")]
        public DateTime start_at { get; set; }
        [Comment("Thời gian kết thúc")]
        public DateTime end_at { get; set; }
        [Comment("Trạng thái")]
        public int status { get; set; } = (int)eStatus_Contest.Scheduled;
        [Comment("Mã môn học")]
        public string? subject_code { get; set; }
        [Comment("Thời hạn")]
        public double duration { get; set; }
        [Comment("Đơn vị tính thời hạn")]
        public int duration_time_unit { get; set; }
        [Comment("Loại cuộc thi")]
        public int contest_type { get; set; }
        [Comment("Cách chấm điểm")]
        public int grading_type { get; set; }
        [Comment("Bảng xếp hạng")]
        public bool leaderboard_enabled { get; set; } = false;
        [Comment("Kiểm tra đạo văn")]
        public bool plagiarism_detection_enabled { get; set; } = false;
        [Comment("Lớp học")]
        public string? class_code { get; set; }
        [Comment("Ngôn ngữ lập trình")]
        public List<eType_ContestProgrammingLanguage> programming_languages { get; set; } = null!;
        [Comment("Thời gian phạt")]
        public double penalty_time { get; set; }
        [Comment("Đóng băng")]
        public bool is_frozen { get; set; }
    }
    public class contest_configuration : IEntityTypeConfiguration<contest>
    {
        public void Configure(EntityTypeBuilder<contest> builder)
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());

            builder
                .Property(c => c.programming_languages)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, options),
                    v => JsonSerializer.Deserialize<List<eType_ContestProgrammingLanguage>>(v ?? "[]", options)!)
                .HasColumnType("json")
                .Metadata.SetValueComparer(new ValueComparer<List<eType_ContestProgrammingLanguage>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList() // Clone
                ));
        }
    }
}
