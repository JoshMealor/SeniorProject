using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeniorProject.Models;
using System.Data;
using System.Diagnostics;

namespace SeniorProject.Areas.Authenticated.Controlllers
{
    [Area("Authenticated")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        //Add an attribute for routing. Below that is the Index action 
        [Route("{area}/{controller}/{action}")]
        [HttpGet]
        public IActionResult Portal()
        {
            
            return View();
        }


    }
}