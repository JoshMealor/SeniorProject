using SeniorProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace SeniorProject.Controllers
{
    //Use the attribute routing to ensure the middlware sets all actions in this controller to the propper route
    [Route("{controller}/{action}")]
    public class AccountController : Controller
    {
        //Declate a class level variable for the User manager class of the Asp.netcore.identity namespace and pass in the user class in the models folder
        private UserManager<Models.DataLayer.User> userManager;
        //Declate a class level variable for the Signin manager class of the Asp.netcore.identity namespace and pass in the user class in the models folder
        private SignInManager<Models.DataLayer.User> signInManager;

        //Create a constuctor that takes the dependency injection of the two class variables above
        public AccountController(UserManager<Models.DataLayer.User> userMngr,
            SignInManager<Models.DataLayer.User> signInMngr)
        {
            //Assign the parameters from dependency injection to the class level varaibles
            userManager = userMngr;
            signInManager = signInMngr;
        }

        //Add the routing get attribute for http get request for the register action
        [HttpGet]
        public IActionResult Register()
        {
            //Return the register view
            return View();
        }
        //Add the routing post attribute for the http post request for the rgister action
        [HttpPost]
        public async Task<IActionResult> Register(Models.ViewLayer.RegisterViewModel model)
        {
            //Determine if the model is valid. This is done with attributes and the Asp.net helper classes
            if (ModelState.IsValid)
            {
                //Create a new user model from the view model.
                var user = new Models.DataLayer.User { UserName = model.Username };
                //The userManager will handle the database interaction of creating the account
                var result = await userManager.CreateAsync(user,
                                       model.Password);
                //Determine if the user was added
                if (result.Succeeded)
                {
                    //Sign in the new user
                    await signInManager.SignInAsync(user,
                              isPersistent: false);
                    //Redirect to the Home page
                    return RedirectToAction("Portal", "Home",new { area="Authenticated"});
                }
                else
                {
                    //Set the Asp.net model state with all the errors
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            //Return the register page with the errors in the model state
            return View(model);
        }

        //Add the routing post attribute for the http post request for the logout action
        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            //Ensure the user gets signed out
            await signInManager.SignOutAsync();
            //Redirect to the home page
            return RedirectToAction("Index", "Home");
        }

        //Add the routing get attribute for the http get request for the login action
        [HttpGet]
        public IActionResult LogIn()
        {
            //Create a new view model
            var model = new Models.ViewLayer.LoginViewModel();
            //Pass this the the login view and return it
            return View(model);
        }

        //Add the routing post attribute for the http post request for the login action
        [HttpPost]
        public async Task<IActionResult> LogIn(Models.ViewLayer.LoginViewModel model)
        {
            //Determine if the view model is valid via the attributes and the Asp.net helpers
            if (ModelState.IsValid)
            {
                //The view model is valid.
                //Sign out the user and pass in the user's credentials
                var result = await signInManager.PasswordSignInAsync(
                    model.Username, model.Password,false,                   
                    lockoutOnFailure: false);
                //Determine if the logout succeded
                if (result.Succeeded)
                {
                    //The logout succeded return to the home page
                    return RedirectToAction("Index", "Home");
                }
            }
            //The logout failed so add the model state errors
            ModelState.AddModelError("", "Invalid username/password.");
            //Pass back the view model to the login view
            return View(model);
        }


    }
}
