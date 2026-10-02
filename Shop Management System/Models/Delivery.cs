using Microsoft.AspNetCore.Identity;
using Shop_Management_System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop_Management_System.Models
{
    public class Delivery
    {
        public int Id { get; set; }

        [Required, Display(Name = "Inventory Item")]
        public int InventoryID { get; set; }

        [ForeignKey(nameof(InventoryID))]
        public Inventory? Inventory { get; set; }

        [Required]
        public string UserId { get; set; } = default!;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        [Required(ErrorMessage = "Location of delivery is required.")]
        [StringLength(300)]
        [Display(Name = "Delivery Location")]
        public string Location { get; set; } = default!;

        [Display(Name = "Building Name")]
        [StringLength(100)]
        public string? BuildingName { get; set; }

        [Display(Name = "Unit Name")]
        [StringLength(100)]
        public string? UnitName { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required, Display(Name = "Delivery Quantity")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Display(Name = "Delivery Cost"), Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal DeliveryCost { get; set; } = 0;

        [Display(Name = "Date Delivered"), Required]
        public DateTime DateDelivered { get; set; }

        [Display(Name = "Date Requested")]
        public DateTime DateRequested { get; set; } = DateTime.UtcNow;

        [Display(Name = "Status")]
        public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;

        [StringLength(500)]
        [Display(Name = "Admin Notes")]
        public string? AdminNotes { get; set; }
    }

    public enum DeliveryStatus
    {
        [Display(Name = "Pending")] Pending = 0,
        [Display(Name = "Confirmed")] Confirmed = 1,
        [Display(Name = "Out for Delivery")] OutForDelivery = 2,
        [Display(Name = "Delivered")] Delivered = 3,
        [Display(Name = "Cancelled")] Cancelled = 4
    }
}