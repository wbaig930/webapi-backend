using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webapi_backend.Models;
using webapi_backend.Services;

namespace webapi_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesOrdersController : ControllerBase
    {
        private readonly ServiceLayer _serviceLayer;

        public SalesOrdersController(ServiceLayer serviceLayer)
        {
            _serviceLayer = serviceLayer;
        }


        [HttpGet("orders")]
        public async Task<IActionResult> GetSalesOrdersAsync()
        {
            await _serviceLayer.LoginAsync();
            var orders = await _serviceLayer.GetSalesOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("orders/{DocEntry}")]
        public async Task<IActionResult> GetSalesOrderByDocEntryAsync(int DocEntry)
        {
            await _serviceLayer.LoginAsync();
            var order = await _serviceLayer.GetSalesOrderByDocEntryAsync(DocEntry);
            return Ok(order);
        }
        [HttpPost]
        public async Task<IActionResult> CreateSalesOrder([FromBody] SalesOrderHeader order)
        {
            var result = await _serviceLayer.CreateSalesOrderAsync(order);
            return Ok(result);
        }
    }
}
