using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SACA_Infra.Migrations
{
    /// <inheritdoc />
    public partial class addbets_submissiontable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "best_submissions",
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
                    user_id = table.Column<string>(type: "varchar(255)", nullable: false),
                    problem_id = table.Column<string>(type: "varchar(255)", nullable: false),
                    problem_submission_id = table.Column<string>(type: "varchar(255)", nullable: false),
                    score = table.Column<double>(type: "double", nullable: false),
                    submited_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    running_time = table.Column<double>(type: "double", nullable: false),
                    running_memory = table.Column<double>(type: "double", nullable: false),
                    number_of_attempts = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_best_submissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_best_submissions_problem_problem_id",
                        column: x => x.problem_id,
                        principalTable: "problem",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_best_submissions_problem_submission_problem_submission_id",
                        column: x => x.problem_submission_id,
                        principalTable: "problem_submission",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_best_submissions_sys_account_user_id",
                        column: x => x.user_id,
                        principalTable: "sys_account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_best_submissions_problem_id",
                table: "best_submissions",
                column: "problem_id");

            migrationBuilder.CreateIndex(
                name: "IX_best_submissions_problem_submission_id",
                table: "best_submissions",
                column: "problem_submission_id");

            migrationBuilder.CreateIndex(
                name: "IX_best_submissions_user_id",
                table: "best_submissions",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "best_submissions");
        }
    }
}
