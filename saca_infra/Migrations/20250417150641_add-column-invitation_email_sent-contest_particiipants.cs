using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SACA_Infra.Migrations
{
    /// <inheritdoc />
    public partial class addcolumninvitation_email_sentcontest_particiipants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "invitation_email_sent",
                table: "contest_participant",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "invitation_email_sent",
                table: "contest_participant");
        }
    }
}
