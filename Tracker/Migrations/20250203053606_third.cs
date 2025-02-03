using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tracker.Migrations
{
    /// <inheritdoc />
    public partial class third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "Budget");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ExpenseDescription",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExpenseName",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Categories",
                table: "Budget",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpenseDescription",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "ExpenseName",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "Categories",
                table: "Budget");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Expenses",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "Budget",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
