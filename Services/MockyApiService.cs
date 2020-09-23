using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DotNetMockyEndpointTask.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotNetMockyEndpointTask.Services
{
    public class MockyApiService
    {
        public MockyApiService(IConfiguration configuration, HttpClient httpClient, IMemoryCache productsCache)
        {
            _httpClient = httpClient;
            _apiCacheDurationSeconds = int.Parse(configuration["MockyApi:CacheDurationSeconds"]);
            _productsCache = productsCache;
        }

        private readonly HttpClient _httpClient;
        private readonly int _apiCacheDurationSeconds;
        private readonly IMemoryCache _productsCache;

        public async Task<List<Product>> FetchProducts()
        {
            if (_productsCache.TryGetValue("products", out List<Product> productsFromCache))
                return productsFromCache;

            var apiResponse = await _httpClient.GetStringAsync(_httpClient.BaseAddress);
            var deserializedApiResponse = (JObject)JsonConvert.DeserializeObject(apiResponse);
            var productsFromApi = deserializedApiResponse["products"].ToObject<List<Product>>();

            _productsCache.Set("products", productsFromApi, GetProductsCacheOptions());

            return productsFromApi;
        }

        private MemoryCacheEntryOptions GetProductsCacheOptions()
        {
            return new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(_apiCacheDurationSeconds)
            };
        }
    }
}
