using DotNetMockyEndpointTask.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotNetMockyEndpointTask.Controllers.API
{
    [Route("api/mocky")]
    [ApiController]
    public class MockyController : ControllerBase
    {
        public MockyController(MockyApiService mockyApiService)
        {
            _mockyApiService = mockyApiService;
        }
        private readonly MockyApiService _mockyApiService;

        [HttpGet("get-products")]
        public IActionResult GetProducts(int? maxPrice = null, string size = null, string wordsToHighight = null)
        {
            var products = _mockyApiService.FetchProducts();
            return Ok(products);
        }
    }
}
