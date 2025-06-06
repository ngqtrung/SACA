﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SACA_Infra.Context;

#nullable disable

namespace SACA_Infra.Migrations
{
    [DbContext(typeof(SACA_Context))]
    [Migration("20250205155601_add-contest-table")]
    partial class addcontesttable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("SACA_Infra.Models.contest", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("code")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasComment("Mã cuộc thi");

                    b.Property<int>("contest_type")
                        .HasColumnType("int")
                        .HasComment("Loại cuộc thi");

                    b.Property<string>("created_by")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("created_on")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("deleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("deleted_by")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("deleted_on")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("description")
                        .HasColumnType("longtext")
                        .HasComment("Mô tả");

                    b.Property<double>("duration")
                        .HasColumnType("double")
                        .HasComment("Thời hạn");

                    b.Property<int>("duration_time_unit")
                        .HasColumnType("int")
                        .HasComment("Đơn vị tính thời hạn");

                    b.Property<DateTime>("end_at")
                        .HasColumnType("datetime(6)")
                        .HasComment("Thời gian kết thúc");

                    b.Property<int>("grading_type")
                        .HasColumnType("int")
                        .HasComment("Cách chấm điểm");

                    b.Property<bool>("leaderboard_enabled")
                        .HasColumnType("tinyint(1)")
                        .HasComment("Bảng xếp hạng");

                    b.Property<string>("modified_by")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("modified_on")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("plagiarism_detection_enabled")
                        .HasColumnType("tinyint(1)")
                        .HasComment("Kiểm tra đạo văn");

                    b.Property<DateTime>("start_at")
                        .HasColumnType("datetime(6)")
                        .HasComment("Thời gian bắt đầu");

                    b.Property<int>("status")
                        .HasColumnType("int")
                        .HasComment("Trạng thái");

                    b.Property<string>("subject_code")
                        .HasColumnType("longtext")
                        .HasComment("Mã môn học");

                    b.Property<string>("title")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasComment("Tiêu đề cuộc thi");

                    b.HasKey("id");

                    b.ToTable("contest", t =>
                        {
                            t.HasComment("Cuộc thi");
                        });
                });

            modelBuilder.Entity("SACA_Infra.Models.contest_participant", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("account_id")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasComment("Thí sinh");

                    b.Property<string>("contest_id")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasComment("Cuộc thi");

                    b.Property<string>("created_by")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("created_on")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("deleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("deleted_by")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("deleted_on")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("modified_by")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("modified_on")
                        .HasColumnType("datetime(6)");

                    b.HasKey("id");

                    b.HasIndex("account_id");

                    b.HasIndex("contest_id");

                    b.ToTable("contest_participant", t =>
                        {
                            t.HasComment("Thí sinh tham gia cuộc thi");
                        });
                });

            modelBuilder.Entity("SACA_Infra.Models.problem", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("code")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasComment("Mã câu hỏi");

                    b.Property<string>("contest_id")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasComment("Id cuộc thi");

                    b.Property<string>("created_by")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("created_on")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("deleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("deleted_by")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("deleted_on")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("description")
                        .HasColumnType("longtext")
                        .HasComment("Mô tả");

                    b.Property<string>("files")
                        .HasColumnType("longtext")
                        .HasComment("Tài nguyên");

                    b.Property<int?>("max_attempts")
                        .HasColumnType("int")
                        .HasComment("Giới hạn số lần nộp bài");

                    b.Property<string>("modified_by")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("modified_on")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("note")
                        .HasColumnType("longtext")
                        .HasComment("Ghi chú");

                    b.Property<double>("score")
                        .HasColumnType("double")
                        .HasComment("Số điểm của câu hỏi");

                    b.Property<string>("tags")
                        .HasColumnType("longtext")
                        .HasComment("Danh sách tag");

                    b.Property<string>("title")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasComment("Tiêu đề câu hỏi");

                    b.HasKey("id");

                    b.HasIndex("contest_id");

                    b.ToTable("problem", t =>
                        {
                            t.HasComment("Câu hỏi");
                        });
                });

            modelBuilder.Entity("SACA_Infra.Models.problem_submission", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("account_id")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasComment("Người nộp");

                    b.Property<string>("created_by")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("created_on")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("deleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("deleted_by")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("deleted_on")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("file_path")
                        .HasColumnType("longtext")
                        .HasComment("Đường dẫn file nộp");

                    b.Property<string>("modified_by")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("modified_on")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("problem_id")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasComment("Câu hỏi");

                    b.Property<int?>("programming_language")
                        .HasColumnType("int")
                        .HasComment("Loại ngôn ngữ lập trình");

                    b.Property<string>("source_code")
                        .HasColumnType("longtext")
                        .HasComment("Source code");

                    b.Property<DateTime>("submitted_at")
                        .HasColumnType("datetime(6)")
                        .HasComment("Thời gian nộp bài");

                    b.HasKey("id");

                    b.HasIndex("account_id");

                    b.HasIndex("problem_id");

                    b.ToTable("problem_submission", t =>
                        {
                            t.HasComment("Bài nộp");
                        });
                });

            modelBuilder.Entity("SACA_Infra.Models.sys_account", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("created_by")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("created_on")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("deleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("deleted_by")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("deleted_on")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasComment("Email");

                    b.Property<int>("failed_count")
                        .HasColumnType("int")
                        .HasComment("Số lần đăng nhập thất bại");

                    b.Property<string>("fullname")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasComment("Tên đầy đủ");

                    b.Property<DateTime?>("last_login")
                        .HasColumnType("datetime(6)")
                        .HasComment("Lần đăng nhập gần nhất");

                    b.Property<string>("modified_by")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("modified_on")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasComment("Mật khẩu mã hóa");

                    b.Property<string>("password_salt")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasComment("Mã mật khẩu");

                    b.Property<string>("role_id")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<int>("status")
                        .HasColumnType("int")
                        .HasComment("Trạng thái account");

                    b.Property<string>("username")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasComment("Tên đăng nhập");

                    b.HasKey("id");

                    b.HasIndex("role_id");

                    b.ToTable("sys_account", t =>
                        {
                            t.HasComment("Tài khoản");
                        });
                });

            modelBuilder.Entity("SACA_Infra.Models.sys_role", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("created_by")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("created_on")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("deleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("deleted_by")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("deleted_on")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("description")
                        .HasColumnType("longtext");

                    b.Property<string>("modified_by")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("modified_on")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.ToTable("sys_role", t =>
                        {
                            t.HasComment("phan quyen");
                        });
                });

            modelBuilder.Entity("SACA_Infra.Models.test_case", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("code")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasComment("Mã test case");

                    b.Property<string>("created_by")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("created_on")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("deleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("deleted_by")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("deleted_on")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("description")
                        .HasColumnType("longtext")
                        .HasComment("Mô tả");

                    b.Property<double?>("execution_time")
                        .HasColumnType("double")
                        .HasComment("Giới hạn thời gian chạy");

                    b.Property<int?>("execution_time_unit")
                        .HasColumnType("int")
                        .HasComment("Đơn vị tính thời gian");

                    b.Property<string>("input")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasComment("Dữ liệu nhận vào");

                    b.Property<double?>("memory_limit")
                        .HasColumnType("double")
                        .HasComment("Giới hạn bộ nhớ");

                    b.Property<int?>("memory_limit_unit")
                        .HasColumnType("int")
                        .HasComment("Đơn vị tính bộ nhớ");

                    b.Property<string>("modified_by")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("modified_on")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("order")
                        .HasColumnType("int")
                        .HasComment("Thứ tự chạy test case");

                    b.Property<string>("output")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasComment("Kết quả trả ra");

                    b.Property<string>("problem_id")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasComment("Câu hỏi");

                    b.Property<double>("score")
                        .HasColumnType("double")
                        .HasComment("Điểm");

                    b.Property<int>("testcase_type")
                        .HasColumnType("int")
                        .HasComment("Loại test case");

                    b.HasKey("id");

                    b.HasIndex("problem_id");

                    b.ToTable("testcase", t =>
                        {
                            t.HasComment("Test case của câu hỏi");
                        });
                });

            modelBuilder.Entity("SACA_Infra.Models.contest_participant", b =>
                {
                    b.HasOne("SACA_Infra.Models.sys_account", "account")
                        .WithMany("contests")
                        .HasForeignKey("account_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SACA_Infra.Models.contest", "contest")
                        .WithMany("contest_participants")
                        .HasForeignKey("contest_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("account");

                    b.Navigation("contest");
                });

            modelBuilder.Entity("SACA_Infra.Models.problem", b =>
                {
                    b.HasOne("SACA_Infra.Models.contest", "contest")
                        .WithMany("problems")
                        .HasForeignKey("contest_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("contest");
                });

            modelBuilder.Entity("SACA_Infra.Models.problem_submission", b =>
                {
                    b.HasOne("SACA_Infra.Models.sys_account", "submitter")
                        .WithMany("submissions")
                        .HasForeignKey("account_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SACA_Infra.Models.problem", "problem")
                        .WithMany("submissions")
                        .HasForeignKey("problem_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("problem");

                    b.Navigation("submitter");
                });

            modelBuilder.Entity("SACA_Infra.Models.sys_account", b =>
                {
                    b.HasOne("SACA_Infra.Models.sys_role", "sys_role")
                        .WithMany("sys_accounts")
                        .HasForeignKey("role_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("sys_role");
                });

            modelBuilder.Entity("SACA_Infra.Models.test_case", b =>
                {
                    b.HasOne("SACA_Infra.Models.problem", "problem")
                        .WithMany("test_cases")
                        .HasForeignKey("problem_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("problem");
                });

            modelBuilder.Entity("SACA_Infra.Models.contest", b =>
                {
                    b.Navigation("contest_participants");

                    b.Navigation("problems");
                });

            modelBuilder.Entity("SACA_Infra.Models.problem", b =>
                {
                    b.Navigation("submissions");

                    b.Navigation("test_cases");
                });

            modelBuilder.Entity("SACA_Infra.Models.sys_account", b =>
                {
                    b.Navigation("contests");

                    b.Navigation("submissions");
                });

            modelBuilder.Entity("SACA_Infra.Models.sys_role", b =>
                {
                    b.Navigation("sys_accounts");
                });
#pragma warning restore 612, 618
        }
    }
}
