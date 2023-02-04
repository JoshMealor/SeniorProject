
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SeniorProject.Areas.Authenticated.Models.Admin;
using System.Security.Cryptography.Xml;

namespace SeniorProject.Areas.Authenticated.Controlllers
{


    //Add the routing attribute for the Admin area.This applies to all the actions in the controller.
    //Also add the authorization middleware. It will redirect to default area, accounts controller, access denied action
    [Area("Authenticated")]
    [Authorize(Roles = "Admin")]

    public class AdminController : Controller
    {
        //Add class level variables for services
        private SeniorProject.Models.DataLayer.SeniorProjectDBContext _SPDBContext { get; set; }
        private UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;


        public AdminController(SeniorProject.Models.DataLayer.SeniorProjectDBContext SPDBContext, UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            //Assign all dependency injected services to their matching class level varaible
            this._SPDBContext = SPDBContext;
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
        }

        
        public async Task<IActionResult> ManageUsersAsync()
        {
            //No need to check if user is signed in and what role. This is done with controller attributes middleware
            
            //Instantiate a list of the view model which holds user and role info.
            List<ManageUsersAndRolesViewModel> viewModelList = new List<ManageUsersAndRolesViewModel>();
            //Retrieve all User records
            List<IdentityUser> userList = await _userManager.Users.ToListAsync();
            //Retrieve all role records
            List<IdentityRole> roleList = await _roleManager.Roles.ToListAsync();

            foreach (IdentityUser user in userList)
            {
                //Go through each user and find the role they are in

                //Instantiate a single view model object to add certain chunks of info to. Avoiding any sensitive data sent.
                ManageUsersAndRolesViewModel viewModel = new Models.Admin.ManageUsersAndRolesViewModel();
                //Go ahead and add what data we can to the view model
                viewModel.IdentityUserID = user.Id;
                viewModel.UserName = user.UserName;
                viewModel.Phone = user.PhoneNumber;
                viewModel.Email= user.Email;
                
                //Find the matching role for this iteration's user. There should only be one per user.
                foreach (IdentityRole role in roleList)
                {
                    if (await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        //Assign the matching role to the view model's role property
                        viewModel.RoleName = role.Name;
                    }
                }

                //Find the matching member's table entry to get info from this
                var memberRecord = _SPDBContext.Members.Where(a => a.IdentityUserID == user.Id).FirstOrDefault();
                if(memberRecord != null)
                {
                    //add the member table info to the view model
                    viewModel.FirstName= memberRecord.FirstName;
                    viewModel.LastName= memberRecord.LastName;
                    viewModel.Active = memberRecord.ActiveStatus;
                }
                else
                {
                    //They may not be a member yet so just fill with default data
                    viewModel.FirstName = "Not A Member";
                    viewModel.LastName = "Not A Member";
                    viewModel.Active = false;
                }

                //Done pulling data for the single user
                //Add the single view model to the list of view models
                viewModelList.Add(viewModel);
            }
            //Return the view and pass the list of view models
            return View(viewModelList);
        }


        [HttpGet]
        public async Task<IActionResult> AddUserAsync()
        {
            //Adjust the viewbag data in the view to reflect the action. The addedit view is multiuse
            ViewBag.Action = "Add";
            //Instantiate the view model
            AddUserWithRoleViewModel viewModel = new Authenticated.Models.Admin.AddUserWithRoleViewModel();
            viewModel.roleChoices = new List<SelectListItem>();
            //Retrieve all role records
            List<IdentityRole> roleList = await _roleManager.Roles.ToListAsync();
            //Add all the roles in the database as options that can be used in the select element on the form in the view
            foreach (IdentityRole role in roleList)
            {
                //The view model has a list of these objects so instantiate the single object and add it to the list
                SelectListItem item= new SelectListItem();
                item.Text = role.Name;
                item.Value = role.Name;
                viewModel.roleChoices.Add(item);
            }

            //Set all other properties to empty
            viewModel.Username = string.Empty;
            viewModel.Email = string.Empty;
            viewModel.Phone = string.Empty;
            viewModel.Role = string.Empty;
           
            //Pass the view model to the view
            return View("AddUser", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddUserAsync(AddUserWithRoleViewModel viewModel)
        {
            //If there is an error the viewmodel needs a list of role choices
            //Instantiate the viewmodel role list
            viewModel.roleChoices= new List<SelectListItem>();
            //Retrieve all role records
            List<IdentityRole> roleList = await _roleManager.Roles.ToListAsync();
            //Go through each role and add to the the select list. For the matching one ensure rolename is set to this
            foreach (IdentityRole role in roleList)
            {
                //The view model has a list of the selectlistitem objects for the role choices
                SelectListItem item = new SelectListItem();
                item.Text = role.Name;
                item.Value = role.Name;
                if(viewModel.Role == role.Name)
                {
                    item.Selected= true;
                }
                //Add the selectlistitem to the list in the viewModel
                viewModel.roleChoices.Add(item);
            }


            //Preform built in validation for the model. 
            if (ModelState.IsValid)
            {
                //Adding

                //Ensure the viewmodel role has not been tampered before creating a new user
                string validRoleChoice = "";
                //Get a list of roles in system
                List<IdentityRole> rolesInSystem = _roleManager.Roles.ToList();
                foreach (IdentityRole role in rolesInSystem)
                {
                    if (role.Name == viewModel.Role)
                    {
                        validRoleChoice = role.Name;
                    }
                }

                if (validRoleChoice != "")
                {
                    //Valid role choice
                    //Add the new user to this role

                    //Create a new user model from the view model.
                    var user = new IdentityUser { UserName = viewModel.Username };
                    user.Email = viewModel.Email;
                    user.PhoneNumber = viewModel.Phone;
                    //The userManager will handle the database interaction of creating the account
                    var resultCreateUser = await _userManager.CreateAsync(user, viewModel.Password);
                    if (resultCreateUser.Succeeded)
                    {
                        //Now add the new user to the role chosen. The asp tag helper should insert selected value into the role property
                        //Get the newly created user
                        IdentityUser newUser = await _userManager.FindByNameAsync(viewModel.Username);

                        var resultAddToRole = await _userManager.AddToRoleAsync(newUser, validRoleChoice);
                        if (resultAddToRole.Succeeded)
                        {
                            TempData["Change"] = "You successfully added: " + viewModel.Username + " to role: " + validRoleChoice;
                            //Redirect to the User Management page
                            return RedirectToAction("ManageUsers", "Admin", new { area = "Authenticated" });
                        }
                        else
                        {
                            //Set the Asp.net model state with all the errors
                            foreach (var error in resultAddToRole.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                            return View("AddUser", viewModel);
                        }
                    }
                    else
                    {
                        //Set the Asp.net model state with all the errors
                        foreach (var error in resultCreateUser.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View("AddUser", viewModel);
                    }
                }
                else
                {
                    ModelState.AddModelError("Tamper", "The role choice was tampered.");
                    return View("AddUser", viewModel);
                }
            }
            else
            {
                //Model errors are added automatically
                return View("AddUser", viewModel);
            }
        }


        [HttpGet]
        public async Task<IActionResult> EditUserAsync(string Id)
        {
            //Ensure they are not attempting to edit the built in Admin admin account
            //Retrieve the matching IdentityUser
            IdentityUser user = await _userManager.FindByIdAsync(Id);
            if(user.UserName == "admin")
            {
                //Error                
                TempData["Change"] = "Don't change the built in Admin account.";
                //Redirect to the User Management page
                return RedirectToAction("ManageUsers", "Admin", new { area = "Authenticated" });
            }
            
            //Instantiate the view model
            EditUserWithRoleViewModel viewModel = new Authenticated.Models.Admin.EditUserWithRoleViewModel();
            //Instantiate the list in the view model
            viewModel.roleChoices = new List<SelectListItem>();
            
            //Go ahead and add available data to viewModel
            viewModel.Username = user.UserName;
            viewModel.Email= user.Email;
            viewModel.PhoneNumber= user.PhoneNumber;
            viewModel.IdentityUserID = user.Id;

            //Retrieve all role records
            List<IdentityRole> roleList = await _roleManager.Roles.ToListAsync();
            //Go through each role and add to the the select list. For the matching one ensure rolename is set to this
            foreach (IdentityRole role in roleList)
            {
                //The view model has a list of the selectlistitem objects for the role choices
                SelectListItem item = new SelectListItem();
                item.Text = role.Name;
                item.Value = role.Name;
                //Determine if the user is a part of this role
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    //Set the viewmodel property
                    viewModel.role = role.Name;
                    //make sure the selected reflects this
                    item.Selected= true;
                }
                //Add the selectlistitem to the list in the viewModel
                viewModel.roleChoices.Add(item);
            }
              
            return View("EditUser", viewModel);
        }
        
        
        //Add an attribute for routing. The http post method
        [HttpPost]
        public async Task<IActionResult> EditUserAsync(EditUserWithRoleViewModel viewModel)
        {
            //If there is an error the viewmodel needs a list of role choices
            //Instantiate the viewmodel role list
            viewModel.roleChoices = new List<SelectListItem>();
            //Retrieve all role records
            List<IdentityRole> roleList = await _roleManager.Roles.ToListAsync();
            //Go through each role and add to the the select list. For the matching one ensure rolename is set to this
            foreach (IdentityRole role in roleList)
            {
                //The view model has a list of the selectlistitem objects for the role choices
                SelectListItem item = new SelectListItem();
                item.Text = role.Name;
                item.Value = role.Name;
                if (viewModel.role == role.Name)
                {
                    item.Selected = true;
                }
                //Add the selectlistitem to the list in the viewModel
                viewModel.roleChoices.Add(item);
            }


            //Ensure they are not attempting to edit the built in Admin admin account
            //Retrieve the matching IdentityUser
            IdentityUser user = await _userManager.FindByIdAsync(viewModel.IdentityUserID);
            if (user.UserName == "admin")
            {
                //Error                
                TempData["Change"] = "Don't change the built in Admin account.";
                //Redirect to the User Management page
                return RedirectToAction("ManageUsers", "Admin", new { area = "Authenticated" });
            }



            //Preform built in validation for the model. 
            if (ModelState.IsValid)
            {
                //Editing

                //Get a list of roles in system
                List<IdentityRole> rolesInSystem = _roleManager.Roles.ToList();
                //Ensure the viewmodel role has not been tampered
                string validRoleChoice = "";
                foreach (IdentityRole role in rolesInSystem)
                {
                    if (role.Name == viewModel.role)
                    {
                        validRoleChoice = role.Name;
                    }
                }

                if(validRoleChoice != "")
                {
                    //Valid

                    //User retrieved above in the admin filter
                    //Update the IdentityUser object with some of the IdentityUser specific viewmodel changes
                    user.UserName = viewModel.Username;
                    user.Email = viewModel.Email;
                    user.PhoneNumber = viewModel.PhoneNumber;
                    var resultUpdateUser = await _userManager.UpdateAsync(user);
                    if(resultUpdateUser.Succeeded)
                    {
                        //Get the fresh IdentityUser object
                        IdentityUser userUpdated = await _userManager.FindByIdAsync(user.Id);

                        //Determine if the role is the same and remove from previous role if needed
                        //Go through each role and determine if the user is in it
                        foreach (IdentityRole role in rolesInSystem)
                        {
                            if (await _userManager.IsInRoleAsync(userUpdated, role.Name))
                            {
                                //Determine if change is needed
                                if (validRoleChoice != role.Name)
                                {
                                    //Change needed.
                                    //Drop from the iterator role
                                    var resultRemoveFromRole = await _userManager.RemoveFromRoleAsync(userUpdated, role.Name);

                                    if (resultRemoveFromRole.Succeeded)
                                    {
                                        //Add to new role
                                        var resultAddToNewRole = await _userManager.AddToRoleAsync(userUpdated, validRoleChoice);
                                        if (resultAddToNewRole.Succeeded)
                                        {
                                            //Done return success                                            
                                            TempData["Change"] = "You successfully updated " + userUpdated.UserName;


                                            return RedirectToAction("ManageUsers", "Admin", new { area = "Authenticated" });
                                        }
                                        else
                                        {
                                            //Set the Asp.net model state with all the errors
                                            foreach (var error in resultRemoveFromRole.Errors)
                                            {
                                                ModelState.AddModelError("", error.Description);
                                            }
                                            return View("EditUser", viewModel);
                                        }
                                    }
                                    else
                                    {
                                        //Set the Asp.net model state with all the errors
                                        foreach (var error in resultRemoveFromRole.Errors)
                                        {
                                            ModelState.AddModelError("", error.Description);
                                        }
                                        return View("EditUser", viewModel);
                                    }
                                }
                                else
                                {
                                    //no change needed
                                }
                            }
                        }
                        //Possible that no change was needed in role
                        //Done return success
                        //Set the tempdata to delete success
                        TempData["Change"] = "You successfully updated " + userUpdated.UserName;

                        //Reditect the user to the manage users area
                        return RedirectToAction("ManageUsers", "Admin", new { area = "Authenticated" });
                    }
                    else
                    {
                        //Set the Asp.net model state with all the errors
                        foreach (var error in resultUpdateUser.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View("EditUser", viewModel);
                    }
                }
                else
                {
                    //Set the Asp.net model state with all the errors                 
                    ModelState.AddModelError("Tamper", "The role choices have been tampered");                    
                    return View("EditUser", viewModel);
                }
            }
            else
            {

                //Adds the errors automatically if the modelstate is bad
                return View("EditUser", viewModel);
            }
        }



        //Add an attribute for routing. Below that is the Delete action 
        [HttpGet]
        public async Task<IActionResult> DeleteUserAsync(string Id)
        {
            //todo: add a  confimation popup

            //Ensure they are not attempting to edit the built in Admin admin account
            //Retrieve the matching IdentityUser
            IdentityUser user = await _userManager.FindByIdAsync(Id);
            if (user.UserName == "admin")
            {
                //Error                
                TempData["Change"] = "Don't change the built in Admin account.";
                //Redirect to the User Management page
                return RedirectToAction("ManageUsers", "Admin", new { area = "Authenticated" });
            }


            //create a string variable to store any errors
            string errorsFromRoleRemoval = "";
            
            //Retrieve a list of roles
            List<IdentityRole> roles = _roleManager.Roles.ToList();
            //remove from any roles
            foreach (IdentityRole role in roles)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    var resultRoleRemove = await _userManager.RemoveFromRoleAsync(user, role.Name);
                    if (resultRoleRemove.Succeeded)
                    {
                        //Loop and continue
                    }
                    else
                    {
                        //todo: handle error
                        //Error                
                        TempData["Change"] = "Unable to remove user from role prior to deletion.";
                        //Redirect to the User Management page
                        return RedirectToAction("ManageUsers", "Admin", new { area = "Authenticated" });
                    }
                }
            }
            //Now that they are removed from any role, remove the user
            var resultDeleteUser = await _userManager.DeleteAsync(user);
            if (resultDeleteUser.Succeeded)
            {
                //Set the tempdata to delete success
                TempData["Change"] = "You successfully removed " + user.UserName;
            }
            else
            {
                //Set the tempdata to delete success
                TempData["Change"] = "You failed to removed " + user.UserName + " with error(s): " + resultDeleteUser.Errors;
            }
            //Reditect the user to the heffer controller index action
            return RedirectToAction("ManageUsers", "Admin",new {area = "Authenticated"});
        }
    }
}
