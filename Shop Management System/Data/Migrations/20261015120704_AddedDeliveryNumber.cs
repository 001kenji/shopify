using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop_Management_System.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedDeliveryNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveryPersonNo",
                table: "Delivery",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryPersonNo",
                table: "Delivery");
        }
    }
}
