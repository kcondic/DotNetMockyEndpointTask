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

        public List<Product> FilterAndHighlightProducts(List<Product> fetchedProducts, int? maxPrice, string size, string wordsToHighlight)
        {
            // make deep copy so mutation doesn't affect cached items
            _products = fetchedProducts.ConvertAll(prod => new Product
            {
                Title = prod.Title,
                Price = prod.Price,
                Sizes = prod.Sizes,
                Description = prod.Description
            });

            FilterProductsByMaxPrice(maxPrice);
            FilterProductsBySize(size);
            HighlightWordsInDescriptionsOfProducts(wordsToHighlight);
            return _products.ToList();
        }

        public ProductListAggregate AggregateProducts(List<Product> fetchedProducts)
        {
            _products = fetchedProducts;

            var minPrice = GetMinProductPrice();
            var maxPrice = GetMaxProductPrice();

            var allSizes = GetAllProductSizes();

            var mostCommonWords = GetMostCommonWords();

            return new ProductListAggregate
            {
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                AllSizes = allSizes,
                MostCommonWordsInDescriptions = mostCommonWords
            };
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
                foreach(var word in wordsToHighlight)
                    product.Description = Regex.Replace(product.Description, $@"\b({word})\b", $@"<em>${{1}}</em>", RegexOptions.IgnoreCase);
        }

        private int GetMinProductPrice()
        {
            return _products.Min(prod => prod.Price);
        }

        private int GetMaxProductPrice()
        {
            return _products.Max(prod => prod.Price);
        }

        private List<string> GetAllProductSizes()
        {
            return _products.SelectMany(prod => prod.Sizes).Distinct().ToList();
        }

        private List<string> GetMostCommonWords()
        {
            //take all descriptions, flatten them and split into word list according to regex boundary and filter only words (eliminate interpunction and whitespace)
            var allWords = _products.Select(prod => prod.Description).SelectMany(desc => Regex.Split(desc, @"\b")).Where(word => Regex.IsMatch(word, @"\b"));

            var mostCommonWords = allWords
                .GroupBy(word => word)
                .OrderByDescending(wordGroup => wordGroup.Count())
                .ThenBy(wordGroup => wordGroup.Key)
                .Skip(5)
                .Take(10)
                .Select(wordGroup => wordGroup.Key)
                .ToList();

            return mostCommonWords;
        }
    }
}
