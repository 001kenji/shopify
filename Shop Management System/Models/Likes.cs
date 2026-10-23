using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shop_Management_System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop_Management_System.Models
{
    public class Likes
    {
        public int id { get; set; }
        [Required]
        [ForeignKey(nameof(UserId))]
        public string UserId { get; set; }

        
        public ApplicationUser User { get; set; }
        [Required]
        public int ReviewId { get; set; }

        [ForeignKey(nameof(ReviewId))]
        public Review Review { get; set; }
    }
}
