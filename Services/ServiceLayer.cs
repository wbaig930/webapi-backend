using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using System.Text.Json;
using webapi_backend.Models;

namespace webapi_backend.Services
{
    public class ServiceLayer
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public ServiceLayer(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;

            var server = _config["SAP:Server"];
            _httpClient.BaseAddress = new Uri($"https://{server}:50000/b1s/v1/");
        }

        public async Task<bool> LoginAsync()
        {
            var payload = new
            {
                CompanyDB = _config["SAP:CompanyDB"],
                UserName = _config["SAP:UserName"],
                Password = _config["SAP:Password"]
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("Login", content);

            if (!response.IsSuccessStatusCode)
                return false;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var sessionId = doc.RootElement.GetProperty("SessionId").GetString();

            _httpClient.DefaultRequestHeaders.Add("Cookie", $"B1SESSION={sessionId}");
            return true;
        }



        // ---------------------------------------------------Items---------------------------------------------------

        //public async Task<string> GetItemsAsync()
        //{
        //    var response = await _httpClient.GetAsync("Items?$top=20&$orderby=ItemCode");
        //    return await response.Content.ReadAsStringAsync();
        //}

        public async Task<List<dynamic>> GetItemsAsync()
        {
            var items = new List<dynamic>();

            string connectionString = _config.GetConnectionString("SapCompanyDB");
            string query = @"
                            SELECT TOP 20 
                                T0.ItemCode, 
                                T0.ItemName, 
                                ISNULL(T1.Price, 0) AS Price
                            FROM OITM T0
                            LEFT JOIN ITM1 T1 ON T0.ItemCode = T1.ItemCode AND T1.PriceList = 1
                            ORDER BY T0.ItemCode";


            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        items.Add(new
                        {
                            ItemCode = reader["ItemCode"].ToString(),
                            ItemName = reader["ItemName"].ToString(),
                            Price = Convert.ToDecimal(reader["Price"])
                        });
                    }
                }
            }

            return items;
        }

        public async Task<string> GetItemByCodeAsync(string itemCode)
        {
            var response = await _httpClient.GetAsync($"Items('{itemCode}')");
            return await response.Content.ReadAsStringAsync();
        }



        // -------------------------------------------------Customers-------------------------------------------------

        public async Task<string> GetCustomersAsync()
        {
            var response = await _httpClient.GetAsync("BusinessPartners?$filter=CardType eq 'cCustomer'&$top=20&$orderby=CardCode");
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetCustomerByCodeAsync(string cardCode)
        {
            var response = await _httpClient.GetAsync($"BusinessPartners('{cardCode}')");
            return await response.Content.ReadAsStringAsync();
        }

        // --------------------------------------------------Orders---------------------------------------------------

        public async Task<string> GetSalesOrdersAsync()
        {
            var response = await _httpClient.GetAsync("Orders?$top=20&$orderby=DocEntry desc");
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> GetSalesOrderByDocEntryAsync(int docEntry)
        {
            var response = await _httpClient.GetAsync($"Orders({docEntry})");
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> CreateSalesOrderAsync(SalesOrderHeader order)
        {
            await LoginAsync();

            //var sapOrder = new
            //{
            //    CardCode = order.CardCode,
            //    DocDate = order.DocDate,
            //    DocDueDate = order.DocDueDate,
            //    DocumentLines = order.SalesOrderRow.Select(l => new
            //    {
            //        ItemCode = l.ItemCode,
            //        Quantity = l.Quantity,
            //        UnitPrice = l.Price
            //    })
            //};
            var sapOrder = new
            {
                CardCode = order.CardCode,
                DocDate = order.DocDate,
                DocDueDate = order.DocDueDate,
                DocumentLines = order.SalesOrderRow.Select(l => new
                {
                    ItemCode = l.ItemCode,
                    Quantity = l.Quantity > 0 ? l.Quantity : 1,
                    UnitPrice = l.Price > 0 ? l.Price : 1
                })
            };


            var json = JsonSerializer.Serialize(sapOrder);
            Console.WriteLine(json);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Orders", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }
    }
}
