using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SACA_Infra.Migrations
{
    /// <inheritdoc />
    public partial class removefile_idincontest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contest_saca_file_fileid",
                table: "contest");

            migrationBuilder.DropIndex(
                name: "IX_contest_fileid",
                table: "contest");

            migrationBuilder.DropColumn(
                name: "fileid",
                table: "contest");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
