using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SACA_Infra.Migrations
{
    /// <inheritdoc />
    public partial class updatetableproblem_submisstionvs2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "programming_language",
                table: "problem_submission",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Loại ngôn ngữ lập trình",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Loại ngôn ngữ lập trình");

            migrationBuilder.AddColumn<double>(
                name: "runinng_time",
                table: "problem_submission",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "running_memory",
                table: "problem_submission",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "problem_submission",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "saca_file",
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
                    parent_id = table.Column<string>(type: "longtext", nullable: false),
                    name = table.Column<string>(type: "longtext", nullable: false),
                    extension = table.Column<string>(type: "longtext", nullable: false),
                    path = table.Column<string>(type: "longtext", nullable: false),
                    length = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_saca_file", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "saca_file");

            migrationBuilder.DropColumn(
                name: "runinng_time",
                table: "problem_submission");

            migrationBuilder.DropColumn(
                name: "running_memory",
                table: "problem_submission");

            migrationBuilder.DropColumn(
                name: "status",
                table: "problem_submission");

            migrationBuilder.AlterColumn<int>(
                name: "programming_language",
                table: "problem_submission",
                type: "int",
                nullable: true,
                comment: "Loại ngôn ngữ lập trình",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Loại ngôn ngữ lập trình");
        }
    }
}
