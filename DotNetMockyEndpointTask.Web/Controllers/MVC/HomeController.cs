using Microsoft.AspNetCore.Mvc;

namespace DotNetMockyEndpointTask.Controllers.MVC
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
