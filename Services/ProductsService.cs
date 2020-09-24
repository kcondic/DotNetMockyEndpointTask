using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DotNetMockyEndpointTask.Models;

namespace DotNetMockyEndpointTask.Services
{
    public class ProductsService
    {
        private IEnumerable<Product> _products;

        // could maybe be an enum or configurable in appsettings,
        // but decided not to complicate with enum handling in a project like this and think that this would probably never change so it's okay to keep it like this
        private List<string> _availableSizes = new List<string>{"small", "medium", "large"};

        // Find words by looking at word boundary \b at both sides. Won't work for hyphens correctly (e.g. will recognize shirt in t-shirt) but other than that it's okay
        private const string _wordRegex = @"\b{0}\b";

        public List<Product> FilterAndHighlightProducts(List<Product> fetchedProducts, int? maxPrice, string size, string wordsToHighlight)
        {
            _products = fetchedProducts;
            FilterProductsByMaxPrice(maxPrice);
            FilterProductsBySize(size);
            HighlightWordsInDescriptionsOfProducts(wordsToHighlight);
            return _products.ToList();
        }

        private void FilterProductsByMaxPrice(int? maxPrice)
        {
            if (!maxPrice.HasValue || maxPrice <= 0)
                return;

            _products = _products.Where(prod => prod.Price <= maxPrice);
        }

        private void FilterProductsBySize(string size)
        {
            if (string.IsNullOrWhiteSpace(size) || !_availableSizes.Contains(size))
                return;

            _products = _products.Where(prod => prod.Sizes.Contains(size));
        }

        private void HighlightWordsInDescriptionsOfProducts(string wordsToHighlightString)
        {
            if (string.IsNullOrWhiteSpace(wordsToHighlightString) || wordsToHighlightString.Any(char.IsWhiteSpace))
                return;

            var wordsToHighlight = wordsToHighlightString.Split(',');
            foreach (var product in _products)
            {
                foreach(var word in wordsToHighlight)
                    product.Description = Regex.Replace(product.Description, string.Format(_wordRegex, word), $"<em>{word}</em>");
            }
        }
    }
}
