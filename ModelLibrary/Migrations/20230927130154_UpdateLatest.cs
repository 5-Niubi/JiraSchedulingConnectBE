using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModelLibrary.Migrations
{
    public partial class UpdateLatest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "unit_salary",
                table: "workforce",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "cost",
                table: "schedules",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "budget",
                table: "projects",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "budget",
                table: "parameter",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "optimizer",
                table: "parameter",
                type: "int",
                nullable: true,
                comment: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "optimizer",
                table: "parameter");

            migrationBuilder.AlterColumn<double>(
                name: "unit_salary",
                table: "workforce",
                type: "float",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "cost",
                table: "schedules",
                type: "int",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "budget",
                table: "projects",
                type: "float",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "budget",
                table: "parameter",
                type: "int",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
