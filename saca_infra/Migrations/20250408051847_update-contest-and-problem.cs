using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SACA_Infra.Migrations
{
    /// <inheritdoc />
    public partial class updatecontestandproblem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "default_execution_time",
                table: "problem",
                type: "double",
                nullable: true,
                comment: "Giới hạn thời gian chạy (ms)");

            migrationBuilder.AddColumn<double>(
                name: "default_memory_limit",
                table: "problem",
                type: "double",
                nullable: true,
                comment: "Giới hạn bộ nhớ (Kb)");

            migrationBuilder.AddColumn<double>(
                name: "penalty_time",
                table: "contest",
                type: "double",
                nullable: false,
                defaultValue: 0.0,
                comment: "Thời gian phạt");

            migrationBuilder.AddColumn<string>(
                name: "programming_languages",
                table: "contest",
                type: "json",
                nullable: false,
                comment: "Ngôn ngữ lập trình");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "default_execution_time",
                table: "problem");

            migrationBuilder.DropColumn(
                name: "default_memory_limit",
                table: "problem");

            migrationBuilder.DropColumn(
                name: "penalty_time",
                table: "contest");

            migrationBuilder.DropColumn(
                name: "programming_languages",
                table: "contest");
        }
    }
}
