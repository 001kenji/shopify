using Microsoft.AspNetCore.Identity;
using Shop_Management_System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop_Management_System.Models
{
    public class Payment
    {
        public int id { get; set; }
        [Display(Name ="Item Name"),Required]
        public string ItemName { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        [Display(Name ="Payment Method")]
        public MethodTypes PaymentMethod { get; set; }

        [Required,Display(Name ="Payment Date")]
        public DateTime PaymentDate { get; set; }

    }
    
    public enum MethodTypes
    {
        MPESA,VISA,CASH
    }
}
