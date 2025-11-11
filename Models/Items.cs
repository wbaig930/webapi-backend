namespace webapi_backend.Models
{
    public class Items
    {
        public string ItemCode { get; set; } = "";
        public string ItemName { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}