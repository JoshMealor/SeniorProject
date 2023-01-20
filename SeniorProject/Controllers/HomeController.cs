using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace SeniorProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        //Attribute route catch the default set in program.cs
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }

        //Attribute route catch default area, home controller,privacy action
        [Route("{controller}/{action}")]
        public IActionResult Privacy()
        {
            return View();
        }




        //Catches all errors. Including any in the Authenticated area
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Models.ViewLayer.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}