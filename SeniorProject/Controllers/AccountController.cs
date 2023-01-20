using SeniorProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SeniorProject.Controllers
{
    //Attribute route for default area, Account controller, action is a method in this controller
    [Route("{controller}/{action}")]
    public class AccountController : Controller
    {
        //Declare class level variables to hold the 3 Asp.Net Core Identity management classes. 
        private UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;

        //Create a constuctor that takes the dependency injection of the class variables above
        public AccountController(UserManager<IdentityUser> userManager,SignInManager<IdentityUser> signInManager,RoleManager<IdentityRole> roleManager)
        {
            //Assign the parameters from dependency injection to the class level varaibles
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
        }

        //Add the routing http method attribute: get request for the register action
        [HttpGet]
        public async Task<IActionResult> RegisterAsync()
        {
            Models.ViewLayer.RegisterViewModel viewModel = new Models.ViewLayer.RegisterViewModel();

            //Check if they are logged in already
            if (HttpContext.User.Identity != null)
            {
                if (HttpContext.User.Identity.IsAuthenticated == true)
                {
                    //Error, they need to sign out first
                    return RedirectToAction("PleaseLogoutFirst","Account");
                }
                else
                {
                    //No loger signed in
                    //Allow registration
                    return View(viewModel);
                }
            }
            else
            {
                //Never signed in
                //Allow registration
                return View(viewModel);
            }
        }

        //Add the routing http method attribute: post request for the register action
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(Models.ViewLayer.RegisterViewModel model)
        {
           
            //Do the same error checking as the get method of this action even though this should not happen in typical use but is possible
            //Check if they are logged in already
            if (HttpContext.User.Identity != null)
            {
                if (HttpContext.User.Identity.IsAuthenticated == true)
                {
                    //Error, they need to sign out first
                    return RedirectToAction("PleaseLogoutFirst", "Account");
                }
                else
                {
                    //No loger signed in
                    //Allow registration
                    //Determine if the model is valid. This is done with attributes and the Asp.net helper classes
                    if (ModelState.IsValid)
                    {
                        //Create a new user model from the view model.
                        var user = new IdentityUser { UserName = model.Username };
                        //The userManager will handle the database interaction of creating the account
                        var result = await _userManager.CreateAsync(user, model.Password);
                        //Determine if the user was added
                        if (result.Succeeded)
                        {
                            //Sign in the new user
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            //Redirect to the portal
                            return RedirectToAction("Portal", "Home", new { area = "Authenticated" });
                        }
                        else
                        {
                            //Set the Asp.net model state with all the errors
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                            //Return the register page with the errors in the model state
                            return View(model);
                        }
                    }
                    else
                    {
                        //Return the register page with the errors in the model state
                        return View(model);
                    }
                }
            }
            else
            {
                //Never signed in
                //Allow registration
                //Determine if the model is valid. This is done with attributes and the Asp.net helper classes
                if (ModelState.IsValid)
                {
                    //Create a new user model from the view model.
                    var user = new IdentityUser { UserName = model.Username };
                    //The userManager will handle the database interaction of creating the account
                    var result = await _userManager.CreateAsync(user, model.Password);
                    //Determine if the user was added
                    if (result.Succeeded)
                    {
                        //Sign in the new user
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        //Redirect to the portal
                        return RedirectToAction("Portal", "Home", new { area = "Authenticated" });
                    }
                    else
                    {
                        //Set the Asp.net model state with all the errors
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        //Return the register page with the errors in the model state
                        return View(model);
                    }
                }
                else
                {
                    //Return the register page with the errors in the model state
                    return View(model);
                }
            }           
        }

        //Add the routing post attribute for the http post request for the logout action
        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            //Ensure the user gets signed out
            await _signInManager.SignOutAsync();
            //Redirect to the home page
            return RedirectToAction("Index", "Home");
        }

        //Add the routing get attribute for the http get request for the login action
        [HttpGet]
        public IActionResult LogIn()
        {
            //Create a new view model
            var viewModel = new Models.ViewLayer.LoginViewModel();
            
            //Check if they are logged in already
            if (HttpContext.User.Identity != null)
            {
                if (HttpContext.User.Identity.IsAuthenticated == true)
                {
                    //Error, they need to sign out first
                    return RedirectToAction("PleaseLogoutFirst", "Account");
                }
                else
                {
                    //No loger signed in
                    //Allow login
                    return View(viewModel);
                }
            }
            else
            {
                //Never signed in
                //Allow login
                return View(viewModel);
            }
        }

        //Add the routing post attribute for the http post request for the login action
        [HttpPost]
        public async Task<IActionResult> LogIn(Models.ViewLayer.LoginViewModel model)
        {

            

            //Check if they are logged in already
            if (HttpContext.User.Identity != null)
            {
                if (HttpContext.User.Identity.IsAuthenticated == true)
                {
                    //Error, they need to sign out first
                    return RedirectToAction("PleaseLogoutFirst", "Account");
                }
                else
                {
                    //No loger signed in
                    //Allow login
                    //Determine if the view model is valid via the attributes and the Asp.net helpers
                    if (ModelState.IsValid)
                    {
                        //The view model is valid.
                        //Sign in
                        var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false,
                            lockoutOnFailure: false);
                        //Determine if the logout succeded
                        if (result.Succeeded)
                        {
                            //The login succeded return to the home page
                            return RedirectToAction("Portal", "Home", new { area = "Authenticated" });
                        }
                        else
                        {
                            ModelState.AddModelError("", "Sign in failure");
                            //Return the register page with the errors in the model state
                            return View(model);
                        }
                    }
                    else
                    {
                        //The login failed so add the model state errors
                        ModelState.AddModelError("", "Invalid username/password.");
                        //Pass back the view model to the login view
                        return View(model);
                    }
                }
            }
            else
            {
                //Never signed in
                //Allow login
                //Determine if the view model is valid via the attributes and the Asp.net helpers
                if (ModelState.IsValid)
                {
                    //The view model is valid.
                    //Sign in
                    var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false,
                        lockoutOnFailure: false);
                    //Determine if the logout succeded
                    if (result.Succeeded)
                    {
                        //The login succeded return to the home page
                        return RedirectToAction("Portal", "Home", new { area = "Authenticated" });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Sign in failure");
                        //Return the register page with the errors in the model state
                        return View(model);
                    }
                }
                else
                {
                    //The login failed so add the model state errors
                    ModelState.AddModelError("", "Invalid username/password.");
                    //Pass back the view model to the login view
                    return View(model);
                }
            }
        }


        public async Task<IActionResult> PleaseLogoutFirstAsync()
        {
            Models.ViewLayer.AccessDeniedViewModel viewModel = new Models.ViewLayer.AccessDeniedViewModel();

            string userName = HttpContext.User.Identity.Name;
            IdentityUser user = await _userManager.FindByNameAsync(userName);
            viewModel.userName = user.UserName;
            //Get a list of all the roles
            List<IdentityRole> roleList = await _roleManager.Roles.ToListAsync();
            //Find the matching role for this iteration's user. There should only be one per user
            foreach (IdentityRole role in roleList)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    //Assign the matching role to the view model's role property
                    viewModel.roleName = role.Name;
                }
            }
            return View(viewModel);
        }


        public async Task<IActionResult> AccessDenied()
        {
            Models.ViewLayer.AccessDeniedViewModel viewModel = new Models.ViewLayer.AccessDeniedViewModel();

            string userName = HttpContext.User.Identity.Name;
            IdentityUser user = await _userManager.FindByNameAsync(userName);
            viewModel.userName = user.UserName;
            //Get a list of all the roles
            List<IdentityRole> roleList = await _roleManager.Roles.ToListAsync();
            //Find the matching role for this iteration's user. There should only be one per user
            foreach (IdentityRole role in roleList)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    //Assign the matching role to the view model's role property
                    viewModel.roleName = role.Name;
                }
            }
            return View(viewModel);
        }




    }
}
