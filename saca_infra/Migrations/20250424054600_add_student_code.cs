using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SACA_Infra.Migrations
{
    /// <inheritdoc />
    public partial class add_student_code : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "memory_limit",
                table: "testcase",
                type: "double",
                nullable: true,
                comment: "Giới hạn bộ nhớ (KB)",
                oldClrType: typeof(double),
                oldType: "double",
                oldNullable: true,
                oldComment: "Giới hạn bộ nhớ (Kb)");

            migrationBuilder.AddColumn<string>(
                name: "student_code",
                table: "sys_account",
                type: "longtext",
                nullable: true,
                comment: "Định danh sinh viên (tên + hai ký tự đầu của họ và tên đệm + mã số sinh viên)");

            migrationBuilder.AlterColumn<double>(
                name: "default_memory_limit",
                table: "problem",
                type: "double",
                nullable: true,
                comment: "Giới hạn bộ nhớ (KB)",
                oldClrType: typeof(double),
                oldType: "double",
                oldNullable: true,
                oldComment: "Giới hạn bộ nhớ (Kb)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "student_code",
                table: "sys_account");

            migrationBuilder.AlterColumn<double>(
                name: "memory_limit",
                table: "testcase",
                type: "double",
                nullable: true,
                comment: "Giới hạn bộ nhớ (Kb)",
                oldClrType: typeof(double),
                oldType: "double",
                oldNullable: true,
                oldComment: "Giới hạn bộ nhớ (KB)");

            migrationBuilder.AlterColumn<double>(
                name: "default_memory_limit",
                table: "problem",
                type: "double",
                nullable: true,
                comment: "Giới hạn bộ nhớ (Kb)",
                oldClrType: typeof(double),
                oldType: "double",
                oldNullable: true,
                oldComment: "Giới hạn bộ nhớ (KB)");
        }
    }
}
