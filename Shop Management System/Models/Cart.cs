using Shop_Management_System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop_Management_System.Models
{
    public class Cart
    {
        public int Id { get; set; }

        [Required]
        public int? InventoryID { get; set; }

        [ForeignKey(nameof(InventoryID))]
        public Inventory? Inventory { get; set; }

        public string? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        [Display(Name ="Cart Scope")]
        public CartScope Scope { get; set; }

    }

    public enum CartScope
    {
        Book,
        Delivery
    }
}
