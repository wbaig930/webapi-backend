using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webapi_backend.Services;

namespace webapi_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ServiceLayer _serviceLayer;

        public CustomersController(ServiceLayer serviceLayer)
        {
            _serviceLayer = serviceLayer;
        }


        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomersAsync()
        {
            await _serviceLayer.LoginAsync();
            var customers = await _serviceLayer.GetCustomersAsync();
            return Ok(customers);
        }

        [HttpGet("customers/{CardCode}")]
        public async Task<IActionResult> GetCustomerByCodeAsync(string CardCode)
        {
            await _serviceLayer.LoginAsync();
            var customer = await _serviceLayer.GetCustomerByCodeAsync(CardCode);
            return Ok(customer);
        }

    }
}
