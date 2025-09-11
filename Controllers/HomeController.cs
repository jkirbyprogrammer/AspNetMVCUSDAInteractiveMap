using AspNetMVCUSDAInteractiveMap.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace AspNetMVCUSDAInteractiveMap.Controllers
{
    public class HomeController(ILogger<HomeController> logger, IWebHostEnvironment hostingEnvironment) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly IWebHostEnvironment _hostingEnvironment = hostingEnvironment;

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult DashboardDemo(string year = "2025", string type = "ussec")
        {
            DashboardModel dashboardModel = new(year, type);

            return View(dashboardModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
