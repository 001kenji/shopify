using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Shop_Management_System.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedInventoryData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Inventory",
                columns: new[] { "ID", "BestBefore", "BuyingPrice", "Categories", "DeliveryRadius", "Description", "ImagePath", "Is_Delivarable", "Name", "Quantity", "SellingPrice", "Unit" },
                values: new object[,]
                {
                    { 1, new DateTime(2028, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 85.00m, 2, "50 km", "Over-ear Bluetooth headphones with active noise cancellation, 30-hour battery life, and built-in microphone.", "/images/headphones.jpg", true, "Wireless Noise-Cancelling Headphones", 45, 149.99m, "pcs" },
                    { 2, new DateTime(2025, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 8.50m, 0, "15 km", "Premium medium-roast whole coffee beans sourced from sustainable farms. Rich chocolate and caramel notes.", "/images/coffee.jpg", true, "Organic Arabica Coffee Beans", 120, 18.99m, "kg" },
                    { 3, new DateTime(2030, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 45.00m, 1, "30 km", "10-piece induction-ready cookware set with heat-resistant handles and tempered glass lids.", "/images/cookware.jpg", true, "Stainless Steel Cookware Set", 20, 89.99m, "set" },
                    { 4, new DateTime(2035, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 25.00m, 13, "25 km", "100% cotton medium-wash denim jacket. Features button closure, chest pockets, and adjustable waist tabs.", "/images/denim-jacket.jpg", true, "Men's Classic Denim Jacket", 35, 59.99m, "pcs" },
                    { 5, new DateTime(2025, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 12.00m, 11, "0 km", "Brightening facial serum with 15% Vitamin C, Hyaluronic Acid, and Vitamin E. 30ml bottle.", "/images/serum.jpg", false, "Vitamin C Facial Serum", 80, 24.50m, "ml" },
                    { 6, new DateTime(2032, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 15.00m, 12, "100 km", "Rechargeable wireless mouse with adjustable DPI (800-2400), silent clicks, and ergonomic thumb rest.", "/images/mouse.jpg", true, "Ergonomic Wireless Mouse", 150, 34.99m, "pcs" },
                    { 7, new DateTime(2040, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 10.00m, 5, "40 km", "Double-wall vacuum insulated stainless steel bottle. Keeps drinks cold for 24 hours or hot for 12 hours.", "/images/water-bottle.jpg", true, "Insulated Water Bottle (1L)", 200, 22.00m, "L" },
                    { 8, new DateTime(2030, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5.00m, 6, "20 km", "Landscape themed high-quality cardboard puzzle. Recommended for ages 12 and up.", "/images/puzzle.jpg", true, "1000-Piece Jigsaw Puzzle", 60, 15.99m, "pack" },
                    { 9, new DateTime(2035, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4.50m, 3, "25 km", "A5 hardcover notebook with PU leather finish, 120gsm dotted paper, and an elastic closure strap.", "/images/notebook.jpg", true, "Executive Leather Notebook", 95, 12.00m, "pcs" },
                    { 10, new DateTime(2035, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 20.00m, 15, "30 km", "Fits up to 15.6-inch laptops. Features USB charging port, anti-theft pocket, and water-resistant fabric.", "/images/backpack.jpg", true, "Water-Resistant Laptop Backpack", 40, 49.99m, "pcs" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Inventory",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Inventory",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Inventory",
                keyColumn: "ID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Inventory",
                keyColumn: "ID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Inventory",
                keyColumn: "ID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Inventory",
                keyColumn: "ID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Inventory",
                keyColumn: "ID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Inventory",
                keyColumn: "ID",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Inventory",
                keyColumn: "ID",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Inventory",
                keyColumn: "ID",
                keyValue: 10);
        }
    }
}
