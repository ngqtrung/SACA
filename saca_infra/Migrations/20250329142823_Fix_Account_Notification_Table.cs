using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SACA_Infra.Migrations
{
    /// <inheritdoc />
    public partial class Fix_Account_Notification_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_account_notification_contest_notification_id",
                table: "account_notification");

            migrationBuilder.AddForeignKey(
                name: "FK_account_notification_notification_notification_id",
                table: "account_notification",
                column: "notification_id",
                principalTable: "notification",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_account_notification_notification_notification_id",
                table: "account_notification");

            migrationBuilder.AddForeignKey(
                name: "FK_account_notification_contest_notification_id",
                table: "account_notification",
                column: "notification_id",
                principalTable: "contest",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
