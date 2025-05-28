using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SACA_Infra.Migrations
{
    /// <inheritdoc />
    public partial class addplagiarismtobest_submission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "plagiarism_avg",
                table: "best_submissions",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "plagiarism_max",
                table: "best_submissions",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "plagiarism_submission_id",
                table: "best_submissions",
                type: "longtext",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "plagiarism_avg",
                table: "best_submissions");

            migrationBuilder.DropColumn(
                name: "plagiarism_max",
                table: "best_submissions");

            migrationBuilder.DropColumn(
                name: "plagiarism_submission_id",
                table: "best_submissions");
        }
    }
}
