using System.Collections.Generic;

namespace DotNetMockyEndpointTask.Models
{
    public class ProductListAggregate
    {
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public List<string> AllSizes { get; set; }
        public List<string> MostCommonWordsInDescriptions { get; set; }
    }
}
