using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetMockyEndpointTask.Models;

namespace DotNetMockyEndpointTask.Services.Interfaces
{
    public interface IMockyApiService
    {
        public Task<List<Product>> FetchProducts();
    }
}
