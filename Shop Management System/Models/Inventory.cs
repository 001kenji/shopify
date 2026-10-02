using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using Shop_Management_System.Data;
namespace Shop_Management_System.Models
{

    public enum CategoriesChoices
    {
        [Display(Name = "Grocery")]
        Grocery,

        [Display(Name = "Home & Kitchen")]
        HomeAndKitchen,

        [Display(Name = "Electronics & Appliances")]
        ElectronicsAndAppliances,

        [Display(Name = "Office Products")]
        OfficeProducts,

        [Display(Name = "Automotive")]
        Automotive,

        [Display(Name = "Sports & Outdoors")]
        SportsAndOutdoors,

        [Display(Name = "Toys & Games")]
        ToysAndGames,

        [Display(Name = "Kids & Baby Products")]
        KidsAndBabyProducts,

        [Display(Name = "Musical Instruments")]
        MusicalInstruments,

        [Display(Name = "Shoes")]
        Shoes,

        [Display(Name = "Phones & Accessories")]
        PhonesAndAccessories,

        [Display(Name = "Health & Beauty")]
        HealthAndBeauty,

        [Display(Name = "Computers & Accessories")]
        ComputersAndAccessories,

        [Display(Name = "Clothes")]
        Clothes,

        [Display(Name = "Fashion Accessories")]
        FashionAccessories,

        [Display(Name = "Bags")]
        Bags
    }
    public class Inventory
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Display(Name ="Category")]
        [Required(ErrorMessage = "Please select a category.")]
        public CategoriesChoices? Categories { get; set; }

        [Display(Name ="Best Before"),Required]
        public DateTime BestBefore { get; set; }

        [Display(Name = "Buying Price")]
        public decimal BuyingPrice { get; set; }

        [Display(Name = "Selling Price")]
        public decimal SellingPrice { get; set; }
        public int Quantity { get; set; }

        [Display(Name = "Can Be Delivered"), Required]
        public bool Is_Delivarable { get; set; }

        [Display(Name = "Delivery Radius"), Required]
        public string DeliveryRadius { get; set; }

        [Display(Name = "Unit")]
        [StringLength(20)]
        public string? Unit { get; set; }   // e.g. "kg", "g", "L", "ml", "pcs", "pack"

        [BindNever]
        public string ImagePath { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
    }
}
