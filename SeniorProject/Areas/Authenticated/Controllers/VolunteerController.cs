
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeniorProject.Areas.Authenticated.Models.Manager;
using System.Security.Cryptography.Xml;

namespace SeniorProject.Areas.Authenticated.Controlllers
{


    //Add the routing attribute for the Admin area.This applies to all the actions in the controller. Also add the authorization middleware. It will redirect to login if not Authenticated with an account
    [Area("Authenticated")]
    [Authorize(Roles = "Admin,Manager,BasicMember,Volunteer")]

    public class VolunteerController : Controller
    {
        //Add class level variables for services
        private SeniorProject.Models.DataLayer.SeniorProjectDBContext _SPDBContext { get; set; }
        private UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;

        public VolunteerController(SeniorProject.Models.DataLayer.SeniorProjectDBContext SPDBContext, UserManager<IdentityUser> userManager,
           SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            //Assign all dependency injected services to their matching class level varaible
            this._SPDBContext = SPDBContext;
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
        }

        //Add an attribute for routing. Below that is the Index action 
        [HttpGet]
        public async Task<IActionResult> EventsSummaryAsync()
        {
            //Create a list of the viewModel to send. The table will be used as a view model
            List<SeniorProject.Models.DataLayer.TableModels.Event> viewModelList = new List<SeniorProject.Models.DataLayer.TableModels.Event>();

            //Retrieve all event table records
            viewModelList = _SPDBContext.Events.ToList();
            
            //No related info needed to pull into view model

            //Pass this list to the view 
            return View(viewModelList);
        }


      













    }
}
