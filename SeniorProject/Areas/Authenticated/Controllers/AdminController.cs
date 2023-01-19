
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;

namespace SeniorProject.Areas.Authenticated.Controlllers
{
    

    //Add the routing attribute for the Admin area.This applies to all the actions in the controller.
    //Also add the authorization middleware. It will redirect to login if not Authorized
    [Area("Authenticated")]
    [Authorize(Roles = "Admin")]

    public class AdminController : Controller
    {
        //Add dbcontext and identity class reference variables
        private SeniorProject.Models.DataLayer.SeniorProjectDBContext _SPDBContext { get; set; }
        private UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;


        public AdminController(SeniorProject.Models.DataLayer.SeniorProjectDBContext SPDBContext, UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            
            this._SPDBContext = SPDBContext;
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;

        }

        //Add an attribute for routing. 
        [HttpGet]
        public async Task<IActionResult> ManageUsersAsync()
        {
            //Instantiate a list of the view model which holds user and role info.
            List<Authenticated.Models.ManageUsersAndRolesViewModel> viewModelList = new List<Authenticated.Models.ManageUsersAndRolesViewModel>();
            //Retrieve all User records and their role
            List<IdentityUser> userList = await _userManager.Users.ToListAsync();
            foreach (IdentityUser user in userList)
            {
                //Instantiate a single view model object.
                Authenticated.Models.ManageUsersAndRolesViewModel viewModel = new Models.ManageUsersAndRolesViewModel();
                //Assign the current iteration's user to the view model's user property
                viewModel.user = user;
                //Get a list of all the roles
                List<IdentityRole> roleList = await _roleManager.Roles.ToListAsync();
                //Find the matching role for this iteration's user. There should only be one per user
                foreach (IdentityRole role in roleList)
                {
                    if (await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        //Assign the matching role to the view model's role property
                        viewModel.role = role.Name;
                    }
                }
                //Add the single view model to the list of view models
                viewModelList.Add(viewModel);
            }
            //Return the view and pass the list of view models
            return View(viewModelList);
        }


        //Add an attribute for routing. Below that is the Add action 
        [HttpGet]
        public IActionResult AddUser()
        {
            //Adjust the viewbag data in the view to reflect the action 
            ViewBag.Action = "Add";
            //Instantiate the view model
            Authenticated.Models.AddEditUserWithRoleViewModel viewModel = new Authenticated.Models.AddEditUserWithRoleViewModel();
            //Add all the roles in the database as options that can be used in the select element on the form in the view
            foreach (IdentityRole role in _roleManager.Roles)
            {
                //The view model has a list of these objects so instantiate the single object and add it to the list
                SelectListItem item= new SelectListItem();
                item.Text = role.Name;
                item.Value = role.Name;
                viewModel.roleChoices.Add(item);
            }
            //Pass the view model to the view
            return View("AddEditUser", viewModel);
        }


        //Add an attribute for routing. Below that is the Edit action 
        [HttpGet]
        public async Task<IActionResult> EditUserAsync(string Id)
        {
            //Ensure they are logged out first

            //Adjust the viewbag data in the view to reflect the action 
            ViewBag.Action = "Edit";
            Authenticated.Models.AddEditUserWithRoleViewModel viewModel = new Authenticated.Models.AddEditUserWithRoleViewModel();

            IdentityUser user = await _userManager.FindByIdAsync(Id);
            viewModel.user = user;
            foreach (IdentityRole role in _roleManager.Roles)
            {

                SelectListItem item = new SelectListItem();
                item.Text = role.Name;
                item.Value = role.Name;
               
                if (await _userManager.IsInRoleAsync(user,
                              role.Name))
                {
                    viewModel.role = role.Name;
                    //make sure the selected reflects this
                    item.Selected= true;
                }
                viewModel.roleChoices.Add(item);
            }
           

                       
            return View("AddEditUser", viewModel);
        }
        
        
        //Add an attribute for routing. Below that is the AddEdit action 
        [HttpPost]
        public async Task<IActionResult> AddEditUserAsync(Authenticated.Models.AddEditUserWithRoleViewModel viewModel)
        {

            //Preform built in validation for the model
            if (ModelState.IsValid)
            {
                //Determine if adding or editting based on the Id value passed in
                if (viewModel.user.Id == "")
                {
                    //Adding


                    //Create a new user model from the view model.
                    var user = new IdentityUser { UserName = viewModel.Username };
                    //The userManager will handle the database interaction of creating the account
                    var result = await _userManager.CreateAsync(user, viewModel.Password);
                    //Determine if the user was added
                    if (result.Succeeded)
                    {
                        //Now add the new user to the role chosen. The asp tag helper should insert selected value into the role property
                        
                        var result2 = _userManager.AddToRoleAsync(user,viewModel.role);


                        TempData["Change"] = "You successfully added: " + viewModel.Username + " to role: " + viewModel.role;
                        //Redirect to the User Management page
                        return RedirectToAction("ManageUsers", "Admin", new { area = "Authenticated" });
                    }
                    else
                    {
                        //Set the Asp.net model state with all the errors
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View("AddEditUser", viewModel);
                    }                  
                    
                }
                else
                {
                    //Updating

                    //sign them out first

                    //The view model is valid.
                    //Delete the user and then add back so that the password hashing and such is done
                    //find the matching user
                    IdentityUser user = await _userManager.FindByIdAsync(viewModel.user.Id);
                    //remove from any roles
                    foreach (IdentityRole role in _roleManager.Roles)
                    {
                        if (await _userManager.IsInRoleAsync(user, role.Name))
                        {
                            var result3 = await _userManager.RemoveFromRoleAsync(user, role.Name);
                        }
                    }

                    //Delete from database
                    var result2 = await _userManager.DeleteAsync(user);

                    //Create a new user model from the view model.
                    var newuser = new IdentityUser { UserName = viewModel.Username };
                    //The userManager will handle the database interaction of creating the account
                    var result = await _userManager.CreateAsync(user, viewModel.Password);
                    //Determine if the user was added
                    if (result.Succeeded)
                    {
                        //Now add the new user to the role chosen. The asp tag helper should insert selected value into the role property

                        var result3 = _userManager.AddToRoleAsync(user, viewModel.role);


                        TempData["Change"] = "You successfully updated: " + viewModel.Username + " with role: " + viewModel.role;
                        //Redirect to the User Management page
                        return RedirectToAction("ManageUsers", "Admin", new { area = "Authenticated" });
                    }
                    else
                    {
                        //Set the Asp.net model state with all the errors
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View("AddEditUser", viewModel);
                    }




                }
                
            }
            else
            {


                return View("AddEditUser", viewModel);
            }
        }



        //Add an attribute for routing. Below that is the Delete action 
        [HttpGet]
        public async Task<IActionResult> DeleteUserAsync(string Id)
        {
            //Sign the user out first
            //Delete the user and then add back so that the password hashing and such is done
            //find the matching user
            IdentityUser user = await _userManager.FindByIdAsync(Id);
            //remove from any roles
            foreach (IdentityRole role in _roleManager.Roles)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    var result3 = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
            }

            //Delete from database
            var result2 = await _userManager.DeleteAsync(user);

            //Set the tempdata to delete success
            TempData["Change"] = "You successfully removed " + user.UserName;

            
            //Reditect the user to the heffer controller index action
            return RedirectToAction("ManageUsers", "Admin",new {area = "Authenticated"});
        }


        

    }
}
