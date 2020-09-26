using DotNetMockyEndpointTask.Services;
using DotNetMockyEndpointTask.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotNetMockyEndpointTask.Test
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(opt =>
            {
                opt.AddFile("app.log");
            });

            services.AddScoped<IProductsService, ProductsService>();
        }
    }
}
