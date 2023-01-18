using Microsoft.AspNetCore.Mvc;
using SeniorProject.Models;
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

        //Add a method for the index action Get route is default
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }

        //Add a method for the privacy action. Get route is default
        [Route("{controller}/{action}")]
        public IActionResult Privacy()
        {
            return View();
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Models.ViewLayer.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}