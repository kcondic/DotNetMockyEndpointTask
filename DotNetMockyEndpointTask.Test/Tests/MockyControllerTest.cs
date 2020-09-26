using DotNetMockyEndpointTask.Controllers.API;
using DotNetMockyEndpointTask.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DotNetMockyEndpointTask.Test.Tests
{
    public class MockyControllerTest
    {
        public MockyControllerTest(IProductsService productsService)
        {
            var mockMockyApiService = new Mock<IMockyApiService>();
            mockMockyApiService.Setup(repo =>
                repo.FetchProducts()).ReturnsAsync(DataInit.Products);
            _controller = new MockyController(mockMockyApiService.Object, productsService);
        }
        private readonly MockyController _controller;

        [Fact]
        public async void TestControllerResponse()
        {
            var response = await _controller.GetProducts();

            var okObjectResult = response as OkObjectResult;
            Assert.NotNull(okObjectResult);

            // controller uses anonymous object so this gets a little messy because of reflection usage. Maybe it would be better to create a class to hold controller response.
            var responseValue = okObjectResult.Value;
            var productsFromResponse = responseValue.GetType().GetProperty("Products")?.GetValue(responseValue, null);
            var aggregatedProductsFromResponse = responseValue.GetType().GetProperty("AggregatedProducts")?.GetValue(responseValue, null);

            Assert.NotNull(productsFromResponse);
            Assert.NotNull(aggregatedProductsFromResponse);
        }
    }
}
