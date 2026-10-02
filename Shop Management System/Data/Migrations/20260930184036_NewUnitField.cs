using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop_Management_System.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewUnitField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "Inventory",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Inventory");
        }
    }
}
