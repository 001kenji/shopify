using Microsoft.AspNetCore.Identity;
using Shop_Management_System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop_Management_System.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required, Display(Name ="Inventory Item")]
        public int InventoryID { get; set; }

        [ForeignKey(nameof(InventoryID))]
        public Inventory? Inventory { get; set; }

        [Required]
        public string UserId { get; set; } = default!;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        [Required,Display(Name ="Booked Quantity"),Range(1,int.MaxValue,ErrorMessage ="Quantity must be atleast 1.")]
        public int Quantity { get; set; }

        [Display(Name = "Picking Duration & Time"), Required]
        public DateTime PickingDuration { get; set; }

        [Display(Name = "Date Ordered"), Required]
        public DateTime DateOrdered { get; set; }
        [Display(Name = "Status")]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        [StringLength(500)]
        [Display(Name = "Admin Notes")]
        public string? AdminNotes { get; set; }
        // get item_name, Item_description, user_name, user_number with foreign key data retreval 
    }

    public enum BookingStatus
    {
        [Display(Name = "Pending")]
        Pending = 0,

        [Display(Name = "Confirmed")]
        Confirmed = 1,

        [Display(Name = "Completed")]
        Completed = 2,

        [Display(Name = "Cancelled")]
        Cancelled = 3
    }
}
