using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SACA_Infra.Migrations
{
    /// <inheritdoc />
    public partial class updatetestcasetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "execution_time_unit",
                table: "testcase");

            migrationBuilder.DropColumn(
                name: "memory_limit_unit",
                table: "testcase");

            migrationBuilder.AlterColumn<double>(
                name: "memory_limit",
                table: "testcase",
                type: "double",
                nullable: true,
                comment: "Giới hạn bộ nhớ (Kb)",
                oldClrType: typeof(double),
                oldType: "double",
                oldNullable: true,
                oldComment: "Giới hạn bộ nhớ");

            migrationBuilder.AlterColumn<double>(
                name: "execution_time",
                table: "testcase",
                type: "double",
                nullable: true,
                comment: "Giới hạn thời gian chạy (ms)",
                oldClrType: typeof(double),
                oldType: "double",
                oldNullable: true,
                oldComment: "Giới hạn thời gian chạy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "memory_limit",
                table: "testcase",
                type: "double",
                nullable: true,
                comment: "Giới hạn bộ nhớ",
                oldClrType: typeof(double),
                oldType: "double",
                oldNullable: true,
                oldComment: "Giới hạn bộ nhớ (Kb)");

            migrationBuilder.AlterColumn<double>(
                name: "execution_time",
                table: "testcase",
                type: "double",
                nullable: true,
                comment: "Giới hạn thời gian chạy",
                oldClrType: typeof(double),
                oldType: "double",
                oldNullable: true,
                oldComment: "Giới hạn thời gian chạy (ms)");

            migrationBuilder.AddColumn<int>(
                name: "execution_time_unit",
                table: "testcase",
                type: "int",
                nullable: true,
                comment: "Đơn vị tính thời gian");

            migrationBuilder.AddColumn<int>(
                name: "memory_limit_unit",
                table: "testcase",
                type: "int",
                nullable: true,
                comment: "Đơn vị tính bộ nhớ");
        }
    }
}
