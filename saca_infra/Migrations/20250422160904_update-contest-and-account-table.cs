using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SACA_Infra.Migrations
{
    /// <inheritdoc />
    public partial class updatecontestandaccounttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contest_saca_file_file_id",
                table: "contest");

            migrationBuilder.DropIndex(
                name: "IX_contest_file_id",
                table: "contest");

            migrationBuilder.DropColumn(
                name: "file_id",
                table: "contest");

            migrationBuilder.AddColumn<string>(
                name: "roll_number",
                table: "sys_account",
                type: "longtext",
                nullable: true,
                comment: "Mã số sinh viên");

            migrationBuilder.AddColumn<string>(
                name: "class_code",
                table: "contest",
                type: "longtext",
                nullable: true,
                comment: "Lớp học");

            migrationBuilder.AddColumn<string>(
                name: "fileid",
                table: "contest",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_contest_fileid",
                table: "contest",
                column: "fileid");

            migrationBuilder.AddForeignKey(
                name: "FK_contest_saca_file_fileid",
                table: "contest",
                column: "fileid",
                principalTable: "saca_file",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contest_saca_file_fileid",
                table: "contest");

            migrationBuilder.DropIndex(
                name: "IX_contest_fileid",
                table: "contest");

            migrationBuilder.DropColumn(
                name: "roll_number",
                table: "sys_account");

            migrationBuilder.DropColumn(
                name: "class_code",
                table: "contest");

            migrationBuilder.DropColumn(
                name: "fileid",
                table: "contest");

            migrationBuilder.AddColumn<string>(
                name: "file_id",
                table: "contest",
                type: "varchar(255)",
                nullable: true,
                comment: "Tài nguyên");

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
        }
    }
}
