namespace webapi_backend.Models
{
    public class SalesOrderHeader
    {
        //public int DocEntry { get; set; }
        //public int DocNum { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DocDueDate { get; set; }
        public int CardCode { get; set; }
        public string CardName { get; set; } = "";
        public decimal DocTotal { get; set; }
        public List<SalesOrderRow> SalesOrderRow { get; set; } = [];
    }
    public class SalesOrderRow
    {
        public string ItemCode { get; set; } = "";
        public string ItemName { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}