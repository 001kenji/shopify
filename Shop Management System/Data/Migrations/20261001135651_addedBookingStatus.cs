using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop_Management_System.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedBookingStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminNotes",
                table: "Booking",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Booking",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminNotes",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Booking");
        }
    }
}
