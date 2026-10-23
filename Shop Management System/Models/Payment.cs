using Microsoft.AspNetCore.Identity;
using Shop_Management_System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop_Management_System.Models
{
    public class Payment
    {
        public int id { get; set; }

        [ Display(Name = "Booking Item")]
        public int? BookingID { get; set; }

        [ForeignKey(nameof(BookingID))]
        public Booking? Booking { get; set; }

        [ Display(Name = "Delivery Item")]
        public int? DeliveryId { get; set; }

        [ForeignKey(nameof(DeliveryId))]
        public Delivery? Delivery { get; set; }

        [Display(Name ="Item Name"),Required]
        public string ItemName { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; } 

        public string? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        [Display(Name ="Payment Method")]
        public MethodTypes PaymentMethod { get; set; }

        [Required,Display(Name ="Payment Date")]
        public DateTime PaymentDate { get; set; } = DateTime.Now ;

        public ServiceType? Service { get; set; } 

    }
    public enum ServiceType
    {
        [Display(Name ="Book service")]
        BOOK,
        [Display(Name = "Delivery service")]
        DELIVER
    }
    public enum MethodTypes
    {
        [Display(Name ="M-PESA")]
        MPESA,
        [Display(Name = "VISA")]
        VISA,
        [Display(Name = "CASH")]
        CASH
    }
}
