using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SACA_Infra.Migrations
{
    /// <inheritdoc />
    public partial class addsubmission_gradingtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "scrore",
                table: "problem_submission",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "submission_grading",
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
                    problem_submission_id = table.Column<string>(type: "varchar(255)", nullable: false),
                    test_case_id = table.Column<string>(type: "varchar(255)", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    runinng_time = table.Column<double>(type: "double", nullable: false),
                    running_memory = table.Column<double>(type: "double", nullable: false),
                    actual_output = table.Column<string>(type: "longtext", nullable: true),
                    judge0_token = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_submission_grading", x => x.id);
                    table.ForeignKey(
                        name: "FK_submission_grading_problem_submission_problem_submission_id",
                        column: x => x.problem_submission_id,
                        principalTable: "problem_submission",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_submission_grading_testcase_test_case_id",
                        column: x => x.test_case_id,
                        principalTable: "testcase",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_submission_grading_problem_submission_id",
                table: "submission_grading",
                column: "problem_submission_id");

            migrationBuilder.CreateIndex(
                name: "IX_submission_grading_test_case_id",
                table: "submission_grading",
                column: "test_case_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "submission_grading");

            migrationBuilder.DropColumn(
                name: "scrore",
                table: "problem_submission");
        }
    }
}
