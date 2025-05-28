using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SACA_Infra.Migrations
{
    /// <inheritdoc />
    public partial class initdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sys_role",
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
                    name = table.Column<string>(type: "longtext", nullable: false),
                    description = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_role", x => x.id);
                },
                comment: "phan quyen")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "sys_account",
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
                    status = table.Column<int>(type: "int", nullable: false, comment: "Trạng thái account"),
                    username = table.Column<string>(type: "longtext", nullable: false, comment: "Tên đăng nhập"),
                    email = table.Column<string>(type: "longtext", nullable: false, comment: "Email"),
                    fullname = table.Column<string>(type: "longtext", nullable: false, comment: "Tên đầy đủ"),
                    password = table.Column<string>(type: "longtext", nullable: false, comment: "Mật khẩu mã hóa"),
                    password_salt = table.Column<string>(type: "longtext", nullable: false, comment: "Mã mật khẩu"),
                    failed_count = table.Column<int>(type: "int", nullable: false, comment: "Số lần đăng nhập thất bại"),
                    last_login = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "Lần đăng nhập gần nhất"),
                    role_id = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sys_account", x => x.id);
                    table.ForeignKey(
                        name: "FK_sys_account_sys_role_role_id",
                        column: x => x.role_id,
                        principalTable: "sys_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Tài khoản")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_sys_account_role_id",
                table: "sys_account",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sys_account");

            migrationBuilder.DropTable(
                name: "sys_role");
        }
    }
}
