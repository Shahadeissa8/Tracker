using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tracker.Migrations
{
    /// <inheritdoc />
    public partial class recurinInModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Recurring",
                table: "Expenses");

            migrationBuilder.AddColumn<int>(
                name: "Recurrin",
                table: "Expenses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Recurrin",
                table: "Expenses");

            migrationBuilder.AddColumn<bool>(
                name: "Recurring",
                table: "Expenses",
                type: "bit",
                nullable: true);
        }
    }
}
