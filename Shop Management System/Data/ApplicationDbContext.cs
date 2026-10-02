using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Shop_Management_System.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Shop_Management_System.Models.Delivery> Delivery { get; set; } = default!;
        public DbSet<Shop_Management_System.Models.Booking> Booking { get; set; } = default!;
        public DbSet<Shop_Management_System.Models.Inventory> Inventory { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.HasIndex(u => u.NormalizedEmail)
                      .IsUnique()
                      .HasDatabaseName("EmailIndex_Unique");
            });
        }
    }

    
}