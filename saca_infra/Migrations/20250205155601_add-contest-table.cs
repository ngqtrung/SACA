using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SACA_Infra.Migrations
{
    /// <inheritdoc />
    public partial class addcontesttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contest",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(255)", nullable: false),
                    created_on = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true),
                    deleted_on = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    modified_by = table.Column<string>(type: "longtext", nullable: true),
                    modified_on = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    code = table.Column<string>(type: "longtext", nullable: false, comment: "Mã cuộc thi"),
                    title = table.Column<string>(type: "longtext", nullable: false, comment: "Tiêu đề cuộc thi"),
                    description = table.Column<string>(type: "longtext", nullable: true, comment: "Mô tả"),
                    start_at = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "Thời gian bắt đầu"),
                    end_at = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "Thời gian kết thúc"),
                    status = table.Column<int>(type: "int", nullable: false, comment: "Trạng thái"),
                    subject_code = table.Column<string>(type: "longtext", nullable: true, comment: "Mã môn học"),
                    duration = table.Column<double>(type: "double", nullable: false, comment: "Thời hạn"),
                    duration_time_unit = table.Column<int>(type: "int", nullable: false, comment: "Đơn vị tính thời hạn"),
                    contest_type = table.Column<int>(type: "int", nullable: false, comment: "Loại cuộc thi"),
                    grading_type = table.Column<int>(type: "int", nullable: false, comment: "Cách chấm điểm"),
                    leaderboard_enabled = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "Bảng xếp hạng"),
                    plagiarism_detection_enabled = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "Kiểm tra đạo văn")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contest", x => x.id);
                },
                comment: "Cuộc thi")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "contest_participant",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(255)", nullable: false),
                    created_on = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true),
                    deleted_on = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    modified_by = table.Column<string>(type: "longtext", nullable: true),
                    modified_on = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    account_id = table.Column<string>(type: "varchar(255)", nullable: false, comment: "Thí sinh"),
                    contest_id = table.Column<string>(type: "varchar(255)", nullable: false, comment: "Cuộc thi")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contest_participant", x => x.id);
                    table.ForeignKey(
                        name: "FK_contest_participant_contest_contest_id",
                        column: x => x.contest_id,
                        principalTable: "contest",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_contest_participant_sys_account_account_id",
                        column: x => x.account_id,
                        principalTable: "sys_account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Thí sinh tham gia cuộc thi")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "problem",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(255)", nullable: false),
                    created_on = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true),
                    deleted_on = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    modified_by = table.Column<string>(type: "longtext", nullable: true),
                    modified_on = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    contest_id = table.Column<string>(type: "varchar(255)", nullable: false, comment: "Id cuộc thi"),
                    code = table.Column<string>(type: "longtext", nullable: false, comment: "Mã câu hỏi"),
                    title = table.Column<string>(type: "longtext", nullable: false, comment: "Tiêu đề câu hỏi"),
                    description = table.Column<string>(type: "longtext", nullable: true, comment: "Mô tả"),
                    tags = table.Column<string>(type: "longtext", nullable: true, comment: "Danh sách tag"),
                    note = table.Column<string>(type: "longtext", nullable: true, comment: "Ghi chú"),
                    max_attempts = table.Column<int>(type: "int", nullable: true, comment: "Giới hạn số lần nộp bài"),
                    score = table.Column<double>(type: "double", nullable: false, comment: "Số điểm của câu hỏi"),
                    files = table.Column<string>(type: "longtext", nullable: true, comment: "Tài nguyên")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_problem", x => x.id);
                    table.ForeignKey(
                        name: "FK_problem_contest_contest_id",
                        column: x => x.contest_id,
                        principalTable: "contest",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Câu hỏi")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "problem_submission",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(255)", nullable: false),
                    created_on = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true),
                    deleted_on = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    modified_by = table.Column<string>(type: "longtext", nullable: true),
                    modified_on = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    problem_id = table.Column<string>(type: "varchar(255)", nullable: false, comment: "Câu hỏi"),
                    account_id = table.Column<string>(type: "varchar(255)", nullable: false, comment: "Người nộp"),
                    source_code = table.Column<string>(type: "longtext", nullable: true, comment: "Source code"),
                    programming_language = table.Column<int>(type: "int", nullable: true, comment: "Loại ngôn ngữ lập trình"),
                    submitted_at = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "Thời gian nộp bài"),
                    file_path = table.Column<string>(type: "longtext", nullable: true, comment: "Đường dẫn file nộp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_problem_submission", x => x.id);
                    table.ForeignKey(
                        name: "FK_problem_submission_problem_problem_id",
                        column: x => x.problem_id,
                        principalTable: "problem",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_problem_submission_sys_account_account_id",
                        column: x => x.account_id,
                        principalTable: "sys_account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Bài nộp")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "testcase",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(255)", nullable: false),
                    created_on = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_by = table.Column<string>(type: "longtext", nullable: true),
                    deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted_by = table.Column<string>(type: "longtext", nullable: true),
                    deleted_on = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    modified_by = table.Column<string>(type: "longtext", nullable: true),
                    modified_on = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    problem_id = table.Column<string>(type: "varchar(255)", nullable: false, comment: "Câu hỏi"),
                    description = table.Column<string>(type: "longtext", nullable: true, comment: "Mô tả"),
                    code = table.Column<string>(type: "longtext", nullable: false, comment: "Mã test case"),
                    score = table.Column<double>(type: "double", nullable: false, comment: "Điểm"),
                    input = table.Column<string>(type: "longtext", nullable: false, comment: "Dữ liệu nhận vào"),
                    output = table.Column<string>(type: "longtext", nullable: false, comment: "Kết quả trả ra"),
                    testcase_type = table.Column<int>(type: "int", nullable: false, comment: "Loại test case"),
                    order = table.Column<int>(type: "int", nullable: false, comment: "Thứ tự chạy test case"),
                    execution_time = table.Column<double>(type: "double", nullable: true, comment: "Giới hạn thời gian chạy"),
                    execution_time_unit = table.Column<int>(type: "int", nullable: true, comment: "Đơn vị tính thời gian"),
                    memory_limit = table.Column<double>(type: "double", nullable: true, comment: "Giới hạn bộ nhớ"),
                    memory_limit_unit = table.Column<int>(type: "int", nullable: true, comment: "Đơn vị tính bộ nhớ")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_testcase", x => x.id);
                    table.ForeignKey(
                        name: "FK_testcase_problem_problem_id",
                        column: x => x.problem_id,
                        principalTable: "problem",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Test case của câu hỏi")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_contest_participant_account_id",
                table: "contest_participant",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_contest_participant_contest_id",
                table: "contest_participant",
                column: "contest_id");

            migrationBuilder.CreateIndex(
                name: "IX_problem_contest_id",
                table: "problem",
                column: "contest_id");

            migrationBuilder.CreateIndex(
                name: "IX_problem_submission_account_id",
                table: "problem_submission",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_problem_submission_problem_id",
                table: "problem_submission",
                column: "problem_id");

            migrationBuilder.CreateIndex(
                name: "IX_testcase_problem_id",
                table: "testcase",
                column: "problem_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contest_participant");

            migrationBuilder.DropTable(
                name: "problem_submission");

            migrationBuilder.DropTable(
                name: "testcase");

            migrationBuilder.DropTable(
                name: "problem");

            migrationBuilder.DropTable(
                name: "contest");
        }
    }
}
