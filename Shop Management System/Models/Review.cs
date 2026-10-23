using Microsoft.AspNetCore.Identity;
using Shop_Management_System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop_Management_System.Models
{
    public class Review
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }

        [Required]
        public int InventoryID { get; set; }
        [ForeignKey(nameof(InventoryID))]
        public Inventory Inventory { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
        [Required(ErrorMessage = "Please write some review details.")]
        public string Details { get; set; }
        [Required]
        public RatingsGrade Ratings { get; set; }

        [Required, Display(Name = "Posted On")]
        public DateTime PostedOn { get; set; } = DateTime.Now;

        public ServiceReviewed? Service { get; set; }
        public ICollection<Likes>? likes { get; set;  } = new List<Likes>();
    }
    public enum ServiceReviewed
    {
        [Display(Name = "Book service")]
        BOOK,
        [Display(Name = "Delivery service")]
        DELIVER
    }
    public enum RatingsGrade
    {
        [Display(Name = "1 Star")]
        OneStar = 1,   // Explicitly sets it to 1

        [Display(Name = "2 Star")]
        TwoStar = 2,

        [Display(Name = "3 Star")]
        ThreeStar = 3,

        [Display(Name = "4 Star")]
        FourStar = 4,

        [Display(Name = "5 Star")]
        FiveStar = 5
    }


}
