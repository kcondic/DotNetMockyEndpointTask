﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetMockyEndpointTask.Models;
using DotNetMockyEndpointTask.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotNetMockyEndpointTask.Controllers.API
{
    [Route("api/mocky")]
    [ApiController]
    public class MockyController : ControllerBase
    {
        public MockyController(MockyApiService mockyApiService, ProductsService productsService)
        {
            _mockyApiService = mockyApiService;
            _productsService = productsService;
        }
        private readonly MockyApiService _mockyApiService;
        private readonly ProductsService _productsService;

        [HttpGet("get-products")]
        public async Task<IActionResult> GetProducts(int? maxPrice = null, string size = null, string wordsToHighlight = null)
        {
            var products = await _mockyApiService.FetchProducts();

            var filteredAndHighlightedProducts = _productsService.FilterAndHighlightProducts(products, maxPrice, size, wordsToHighlight);

            //not sure from task if products or filteredAndHighlightedProducts are to be passed here...
            var productAggregateInfo = _productsService.AggregateProducts(products);

            return Ok(new
            {
                Products = filteredAndHighlightedProducts,
                AggregatedProducts = productAggregateInfo
            });
        }
    }
}
