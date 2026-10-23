using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shop_Management_System.Models;

namespace Shop_Management_System.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Shop_Management_System.Models.Review> Review { get; set; } = default!;
        public DbSet<Shop_Management_System.Models.Payment> Payment { get; set; } = default!;
        public DbSet<Shop_Management_System.Models.Delivery> Delivery { get; set; } = default!;
        public DbSet<Shop_Management_System.Models.Booking> Booking { get; set; } = default!;
        public DbSet<Shop_Management_System.Models.Inventory> Inventory { get; set; } = default!;
        public DbSet<Shop_Management_System.Models.Likes> Likes { get; set; } = default!;
        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);

        //    builder.Entity<ApplicationUser>(entity =>
        //    {
        //        entity.HasIndex(u => u.NormalizedEmail)
        //              .IsUnique()
        //              .HasDatabaseName("EmailIndex_Unique");
        //    });

        //    builder.Entity<Payment>(entity =>
        //    {
        //        entity.HasOne(p => p.Booking)
        //              .WithMany()
        //              .HasForeignKey(p => p.BookingID)
        //              .OnDelete(DeleteBehavior.Restrict);

        //        entity.HasOne(p => p.Delivery)
        //              .WithMany()
        //              .HasForeignKey(p => p.DeliveryId)
        //              .OnDelete(DeleteBehavior.Restrict);

        //        entity.HasOne(p => p.User)
        //              .WithMany()
        //              .HasForeignKey(p => p.UserId)
        //              .OnDelete(DeleteBehavior.Restrict);
        //    });

        //    // Configure Likes -> User relationship (NoAction to break the cycle)
        //    builder.Entity<Likes>()
        //        .HasOne(l => l.User)
        //        .WithMany()
        //        .HasForeignKey(l => l.UserId)
        //        .OnDelete(DeleteBehavior.NoAction);

        //    // Configure Likes -> Review relationship (Cascade)
        //    builder.Entity<Likes>()
        //        .HasOne(l => l.Review)
        //        .WithMany(r => r.likes)
        //        .HasForeignKey(l => l.ReviewId)
        //        .OnDelete(DeleteBehavior.Cascade);


        //}


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.HasIndex(u => u.NormalizedEmail)
                      .IsUnique()
                      .HasDatabaseName("EmailIndex_Unique");
            });

            builder.Entity<Payment>(entity =>
            {
                entity.HasOne(p => p.Booking)
                      .WithMany()
                      .HasForeignKey(p => p.BookingID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Delivery)
                      .WithMany()
                      .HasForeignKey(p => p.DeliveryId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.User)
                      .WithMany()
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Likes -> User relationship (NoAction to break the cycle)
            builder.Entity<Likes>()
                .HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure Likes -> Review relationship (Cascade)
            builder.Entity<Likes>()
                .HasOne(l => l.Review)
                .WithMany(r => r.likes)
                .HasForeignKey(l => l.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed 10 diverse inventory items
            builder.Entity<Inventory>().HasData(
                // 1. Electronics
                new Inventory
                {
                    ID = 1,
                    Name = "Wireless Noise-Cancelling Headphones",
                    Description = "Over-ear Bluetooth headphones with active noise cancellation, 30-hour battery life, and built-in microphone.",
                    Categories = CategoriesChoices.ElectronicsAndAppliances,
                    BestBefore = new DateTime(2028, 12, 31),
                    BuyingPrice = 85.00m,
                    SellingPrice = 149.99m,
                    Quantity = 45,
                    Is_Delivarable = true,
                    DeliveryRadius = "50 km",
                    Unit = "pcs",
                    ImagePath = "/images/headphones.jpg"
                },
                // 2. Grocery
                new Inventory
                {
                    ID = 2,
                    Name = "Organic Arabica Coffee Beans",
                    Description = "Premium medium-roast whole coffee beans sourced from sustainable farms. Rich chocolate and caramel notes.",
                    Categories = CategoriesChoices.Grocery,
                    BestBefore = new DateTime(2025, 6, 15),
                    BuyingPrice = 8.50m,
                    SellingPrice = 18.99m,
                    Quantity = 120,
                    Is_Delivarable = true,
                    DeliveryRadius = "15 km",
                    Unit = "kg",
                    ImagePath = "/images/coffee.jpg"
                },
                // 3. Home & Kitchen
                new Inventory
                {
                    ID = 3,
                    Name = "Stainless Steel Cookware Set",
                    Description = "10-piece induction-ready cookware set with heat-resistant handles and tempered glass lids.",
                    Categories = CategoriesChoices.HomeAndKitchen,
                    BestBefore = new DateTime(2030, 1, 1),
                    BuyingPrice = 45.00m,
                    SellingPrice = 89.99m,
                    Quantity = 20,
                    Is_Delivarable = true,
                    DeliveryRadius = "30 km",
                    Unit = "set",
                    ImagePath = "/images/cookware.jpg"
                },
                // 4. Clothes
                new Inventory
                {
                    ID = 4,
                    Name = "Men's Classic Denim Jacket",
                    Description = "100% cotton medium-wash denim jacket. Features button closure, chest pockets, and adjustable waist tabs.",
                    Categories = CategoriesChoices.Clothes,
                    BestBefore = new DateTime(2035, 1, 1),
                    BuyingPrice = 25.00m,
                    SellingPrice = 59.99m,
                    Quantity = 35,
                    Is_Delivarable = true,
                    DeliveryRadius = "25 km",
                    Unit = "pcs",
                    ImagePath = "/images/denim-jacket.jpg"
                },
                // 5. Health & Beauty
                new Inventory
                {
                    ID = 5,
                    Name = "Vitamin C Facial Serum",
                    Description = "Brightening facial serum with 15% Vitamin C, Hyaluronic Acid, and Vitamin E. 30ml bottle.",
                    Categories = CategoriesChoices.HealthAndBeauty,
                    BestBefore = new DateTime(2025, 10, 20),
                    BuyingPrice = 12.00m,
                    SellingPrice = 24.50m,
                    Quantity = 80,
                    Is_Delivarable = false,
                    DeliveryRadius = "0 km",
                    Unit = "ml",
                    ImagePath = "/images/serum.jpg"
                },
                // 6. Computers
                new Inventory
                {
                    ID = 6,
                    Name = "Ergonomic Wireless Mouse",
                    Description = "Rechargeable wireless mouse with adjustable DPI (800-2400), silent clicks, and ergonomic thumb rest.",
                    Categories = CategoriesChoices.ComputersAndAccessories,
                    BestBefore = new DateTime(2032, 1, 1),
                    BuyingPrice = 15.00m,
                    SellingPrice = 34.99m,
                    Quantity = 150,
                    Is_Delivarable = true,
                    DeliveryRadius = "100 km",
                    Unit = "pcs",
                    ImagePath = "/images/mouse.jpg"
                },
                // 7. Sports & Outdoors
                new Inventory
                {
                    ID = 7,
                    Name = "Insulated Water Bottle (1L)",
                    Description = "Double-wall vacuum insulated stainless steel bottle. Keeps drinks cold for 24 hours or hot for 12 hours.",
                    Categories = CategoriesChoices.SportsAndOutdoors,
                    BestBefore = new DateTime(2040, 1, 1),
                    BuyingPrice = 10.00m,
                    SellingPrice = 22.00m,
                    Quantity = 200,
                    Is_Delivarable = true,
                    DeliveryRadius = "40 km",
                    Unit = "L",
                    ImagePath = "/images/water-bottle.jpg"
                },
                // 8. Toys & Games
                new Inventory
                {
                    ID = 8,
                    Name = "1000-Piece Jigsaw Puzzle",
                    Description = "Landscape themed high-quality cardboard puzzle. Recommended for ages 12 and up.",
                    Categories = CategoriesChoices.ToysAndGames,
                    BestBefore = new DateTime(2030, 1, 1),
                    BuyingPrice = 5.00m,
                    SellingPrice = 15.99m,
                    Quantity = 60,
                    Is_Delivarable = true,
                    DeliveryRadius = "20 km",
                    Unit = "pack",
                    ImagePath = "/images/puzzle.jpg"
                },
                // 9. Office Products
                new Inventory
                {
                    ID = 9,
                    Name = "Executive Leather Notebook",
                    Description = "A5 hardcover notebook with PU leather finish, 120gsm dotted paper, and an elastic closure strap.",
                    Categories = CategoriesChoices.OfficeProducts,
                    BestBefore = new DateTime(2035, 1, 1),
                    BuyingPrice = 4.50m,
                    SellingPrice = 12.00m,
                    Quantity = 95,
                    Is_Delivarable = true,
                    DeliveryRadius = "25 km",
                    Unit = "pcs",
                    ImagePath = "/images/notebook.jpg"
                },
                // 10. Bags
                new Inventory
                {
                    ID = 10,
                    Name = "Water-Resistant Laptop Backpack",
                    Description = "Fits up to 15.6-inch laptops. Features USB charging port, anti-theft pocket, and water-resistant fabric.",
                    Categories = CategoriesChoices.Bags,
                    BestBefore = new DateTime(2035, 1, 1),
                    BuyingPrice = 20.00m,
                    SellingPrice = 49.99m,
                    Quantity = 40,
                    Is_Delivarable = true,
                    DeliveryRadius = "30 km",
                    Unit = "pcs",
                    ImagePath = "/images/backpack.jpg"
                }
            );
        }


    }

    
}