using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DotNetMockyEndpointTask.Services.Interfaces;
using Moq;
using Xunit;

namespace DotNetMockyEndpointTask.Test.Tests
{
    public class ProductsServiceTest
    {
        public ProductsServiceTest(IProductsService productsService)
        {
            _mockMockyApiService = new Mock<IMockyApiService>();
            _mockMockyApiService.Setup(repo =>
                repo.FetchProducts()).ReturnsAsync(DataInit.Products);
            _productsService = productsService;
        }
        private readonly Mock<IMockyApiService> _mockMockyApiService;
        private readonly IProductsService _productsService;

        [Fact]
        public async void TestFilterProductsWithoutFiltering()
        {
            var products = await _mockMockyApiService.Object.FetchProducts();

            var filteredProducts = _productsService.FilterAndHighlightProducts(products, null, null, null);

            Assert.Equal(products.Count, filteredProducts.Count);
        }

        [Fact]
        public async void TestFilterProductsByMaxPrice()
        {
            var products = await _mockMockyApiService.Object.FetchProducts();
            var maxPrice = 13;

            var filteredProducts = _productsService.FilterAndHighlightProducts(products, maxPrice, null, null);

            Assert.All(filteredProducts, prod => Assert.True(prod.Price <= maxPrice));

            var productsThatWereLeftOut = products.Where(prod => filteredProducts.All(filteredProd => filteredProd.Title != prod.Title)).ToList();
            
            Assert.All(productsThatWereLeftOut, prod => Assert.True(prod.Price > maxPrice));
        }

        [Fact]
        public async void TestFilterProductsBySize()
        {
            var products = await _mockMockyApiService.Object.FetchProducts();
            var size = "medium";

            var filteredProducts = _productsService.FilterAndHighlightProducts(products, null, size, null);

            Assert.All(filteredProducts, prod => Assert.Contains(size, prod.Sizes));

            var productsThatWereLeftOut = products.Where(prod => filteredProducts.All(filteredProd => filteredProd.Title != prod.Title)).ToList();

            Assert.All(productsThatWereLeftOut, prod => Assert.DoesNotContain(size, prod.Sizes));
        }

        [Fact]
        public async void TestHighlightWords()
        {
            var products = await _mockMockyApiService.Object.FetchProducts();
            var wordsToHighlight = "socks,is";

            var highlightedProducts = _productsService.FilterAndHighlightProducts(products, null, null, wordsToHighlight);

            var separateWordsToHighlight = wordsToHighlight.Split(',');

            var wordsWithinEmRegex = @"(?<=<em>)\b.*?\b(?=</em>)";

            Assert.All(highlightedProducts, prod =>
            {
                var wordsWithinEm = Regex.Matches(prod.Description, wordsWithinEmRegex).Select(m => m.Value).ToList();
                Assert.All(wordsWithinEm, word => Assert.Contains(word, separateWordsToHighlight));
            });
        }

        [Fact] 
        // I don't think there's a better way to validate this other than hardcode expected values.
        // Not ideal because when test data changes test will need to be updated also...
        public async void TestAggregateProducts()
        {
            var products = await _mockMockyApiService.Object.FetchProducts();

            var expectedMinPrice = 11;
            var expectedMaxPrice = 15;
            var expectedAllSizes = new List<string> { "medium", "large", "small" };
            var expectedMostCommonWords = new List<string> {"have", "scarf", "smell", "socks", "soft", "sweater", "These", "warm"};

            var aggregatedProducts = _productsService.AggregateProducts(products);

            Assert.True(aggregatedProducts.MinPrice == expectedMinPrice);       
            Assert.True(aggregatedProducts.MaxPrice == expectedMaxPrice);
            Assert.True(aggregatedProducts.AllSizes.SequenceEqual(expectedAllSizes));
            Assert.True(aggregatedProducts.MostCommonWordsInDescriptions.SequenceEqual(expectedMostCommonWords));
        }
    }
}
