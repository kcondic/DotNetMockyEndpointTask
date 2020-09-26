using System.Collections.Generic;
using DotNetMockyEndpointTask.Models;

namespace DotNetMockyEndpointTask.Services.Interfaces
{
    public interface IProductsService
    {
        public List<Product> FilterAndHighlightProducts(List<Product> fetchedProducts, int? maxPrice, string size, string wordsToHighlight);
        public ProductListAggregate AggregateProducts(List<Product> fetchedProducts);
    }
}
