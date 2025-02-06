using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tracker.Migrations
{
    /// <inheritdoc />
    public partial class exp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Recurrin",
                table: "Expenses",
                newName: "Recurring");

            migrationBuilder.RenameColumn(
                name: "ExpenseDes",
                table: "Expenses",
                newName: "ExpenseDescription");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Recurring",
                table: "Expenses",
                newName: "Recurrin");

            migrationBuilder.RenameColumn(
                name: "ExpenseDescription",
                table: "Expenses",
                newName: "ExpenseDes");
        }
    }
}
