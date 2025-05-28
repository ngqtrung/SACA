using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SACA_Infra.Migrations
{
    /// <inheritdoc />
    public partial class addfrozen_scoreis_frozentosometables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_frozen",
                table: "contest",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                comment: "Đóng băng");

            migrationBuilder.AddColumn<double>(
                name: "frozen_score",
                table: "best_submissions",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_frozen",
                table: "contest");

            migrationBuilder.DropColumn(
                name: "frozen_score",
                table: "best_submissions");
        }
    }
}
