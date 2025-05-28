using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SACA_Infra.Migrations
{
    /// <inheritdoc />
    public partial class add_notification_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notification",
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
                    notification_id = table.Column<string>(type: "longtext", nullable: false, comment: "Mã thông báo"),
                    title = table.Column<string>(type: "longtext", nullable: false, comment: "Tiêu đề thông báo"),
                    description = table.Column<string>(type: "longtext", nullable: false, comment: "Nội dung"),
                    contest_id = table.Column<string>(type: "varchar(255)", nullable: false, comment: "Id cuộc thi")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification", x => x.id);
                    table.ForeignKey(
                        name: "FK_notification_contest_contest_id",
                        column: x => x.contest_id,
                        principalTable: "contest",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Thông báo")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_notification_contest_id",
                table: "notification",
                column: "contest_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notification");
        }
    }
}
