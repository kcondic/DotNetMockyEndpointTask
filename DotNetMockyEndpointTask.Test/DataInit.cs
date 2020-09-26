using System.Collections.Generic;
using DotNetMockyEndpointTask.Models;

namespace DotNetMockyEndpointTask.Test
{
    public static class DataInit
    {
        public static List<Product> Products = new List<Product>
        {
            new Product
            {
                Title = "Old socks",
                Price = 11,
                Sizes = new List<string> { "medium", "large" },
                Description = "These socks have a distinct smell"
            },
            new Product
            {
                Title = "Purple sweater",
                Price = 15,
                Sizes = new List<string> { "large" },
                Description = "This sweater is very warm"
            },
            new Product
            {
                Title = "Silk scarf",
                Price = 12,
                Sizes = new List<string> { "small", "large" },
                Description = "This scarf is very soft"
            }
        };
    }
}
