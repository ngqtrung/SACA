using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SACA_Infra.Migrations
{
    /// <inheritdoc />
    public partial class updatefileproperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "files",
                table: "problem");

            migrationBuilder.AlterColumn<string>(
                name: "parent_id",
                table: "saca_file",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AddColumn<string>(
                name: "file_id",
                table: "problem",
                type: "varchar(255)",
                nullable: true,
                comment: "Tài nguyên");

            migrationBuilder.AddColumn<string>(
                name: "file_id",
                table: "contest",
                type: "varchar(255)",
                nullable: true,
                comment: "Tài nguyên");

            migrationBuilder.CreateIndex(
                name: "IX_problem_file_id",
                table: "problem",
                column: "file_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_contest_file_id",
                table: "contest",
                column: "file_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_contest_saca_file_file_id",
                table: "contest",
                column: "file_id",
                principalTable: "saca_file",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_problem_saca_file_file_id",
                table: "problem",
                column: "file_id",
                principalTable: "saca_file",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contest_saca_file_file_id",
                table: "contest");

            migrationBuilder.DropForeignKey(
                name: "FK_problem_saca_file_file_id",
                table: "problem");

            migrationBuilder.DropIndex(
                name: "IX_problem_file_id",
                table: "problem");

            migrationBuilder.DropIndex(
                name: "IX_contest_file_id",
                table: "contest");

            migrationBuilder.DropColumn(
                name: "file_id",
                table: "problem");

            migrationBuilder.DropColumn(
                name: "file_id",
                table: "contest");

            migrationBuilder.AlterColumn<string>(
                name: "parent_id",
                table: "saca_file",
                type: "longtext",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "files",
                table: "problem",
                type: "longtext",
                nullable: true,
                comment: "Tài nguyên");
        }
    }
}
