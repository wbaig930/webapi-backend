using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using webapi_backend.Models;
namespace webapi_backend.Services
{
    public class ServiceLayer
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://DESKTOP-CC4CPJ3:50000/b1s/v1/";
        private readonly string _companyDB = "DemoCo";
        private readonly string _username = "manager";
        private readonly string _password = "admin";
        private string _sessionId = "";

        public string SessionId => _sessionId;

        public ServiceLayer()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback =
                (req, cert, chain, errors) => true;

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(_baseUrl)
            };
        }

        public async Task<bool> LoginAsync()
        {
            var loginData = new
            {
                CompanyDB = _companyDB,
                UserName = _username,
                Password = _password
            };
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(loginData), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("Login", content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonDoc = System.Text.Json.JsonDocument.Parse(responseContent);
                _sessionId = jsonDoc.RootElement.GetProperty("SessionId").GetString() ?? "";
                return true;
            }
            else
            {
                return false;
            }
        }

        // ---------------------------------------------------Items---------------------------------------------------

        public async Task<string> GetItemsAsync()
        {
            var response = await _httpClient.GetAsync("Items?$top=20&$orderby=ItemCode");
            return await response.Content.ReadAsStringAsync();
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

            var sapOrder = new
            {
                CardCode = order.CardCode,
                DocDate = order.DocDate,
                DocDueDate = order.DocDueDate,
                DocumentLines = order.SalesOrderRow.Select(l => new
                {
                    ItemCode = l.ItemCode,
                    Quantity = l.Quantity,
                    UnitPrice = l.Price
                })
            };

            var json = JsonSerializer.Serialize(sapOrder);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Orders", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }
    }
}
