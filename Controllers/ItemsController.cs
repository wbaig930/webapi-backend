using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webapi_backend.Models;
using webapi_backend.Services;

namespace webapi_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly ServiceLayer _serviceLayer;
    
        public ItemsController(ServiceLayer serviceLayer)
        {
            _serviceLayer = serviceLayer;
        }


        [HttpGet("items")]
        public async Task<IActionResult> GetItemsAsync()
        {
            await _serviceLayer.LoginAsync();
            var items = await _serviceLayer.GetItemsAsync();
            return Ok(items);
        }

        [HttpGet("items/{itemCode}")]
        public async Task<IActionResult> GetItemByCodeAsync(string itemCode)
        {
            await _serviceLayer.LoginAsync();
            var item = await _serviceLayer.GetItemByCodeAsync(itemCode);
            return Ok(item);
        }



    } 
}
