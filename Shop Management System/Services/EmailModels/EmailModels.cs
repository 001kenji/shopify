namespace Shop_Management_System.Services.EmailModels
{
    public class ConfirmAccountEmail
    {
        public string UserName { get; set; } = "";
        public string CallbackUrl { get; set; } = "";
    }

    public class ResetPasswordEmail
    {
        public string UserName { get; set; } = "";
        public string CallbackUrl { get; set; } = "";
    }

    public class BookingEmail
    {
        public string UserName { get; set; } = "";
        public string Reference { get; set; } = "";
        public string ItemName { get; set; } = "";
        public int Quantity { get; set; }
        public string Unit { get; set; } = "pcs";
        public string PickupDate { get; set; } = "";
        public string Total { get; set; } = "0.00";
    }

    public class DeliveryEmail
    {
        public string UserName { get; set; } = "";
        public string Reference { get; set; } = "";
        public string ItemName { get; set; } = "";
        public int Quantity { get; set; }
        public string Unit { get; set; } = "pcs";
        public string Location { get; set; } = "";
        public string DeliveryDate { get; set; } = "";
        public string Total { get; set; } = "0.00";
    }
}