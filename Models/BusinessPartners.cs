namespace webapi_backend.Models
{
    public class BusinessPartners
    {
        public int CardCode { get; set; }
        public string CardName { get; set; } = "";
        public string Address { get; set; } = "";
        public string Country { get; set; } = "";
        public string EmailAddress { get; set; } = "";

    }
}