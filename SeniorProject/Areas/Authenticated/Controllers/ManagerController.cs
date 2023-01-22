
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;

namespace SeniorProject.Areas.Authenticated.Controlllers
{
    

    //Add the routing attribute for the Admin area.This applies to all the actions in the controller. Also add the authorization middleware. It will redirect to login if not Authenticated with an account
    [Area("Authenticated")]
    [Authorize(Roles = "Admin,Manager")]

    public class ManagerController : Controller
    {
        //Add class level variables for services
        private SeniorProject.Models.DataLayer.SeniorProjectDBContext _SPDBContext { get; set; }
        private UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;

        public ManagerController(SeniorProject.Models.DataLayer.SeniorProjectDBContext SPDBContext, UserManager<IdentityUser> userManager,
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
        public async Task<IActionResult> ManageMembersAsync()
        {
            //Create a list of the viewModel to sent
            List<SeniorProject.Areas.Authenticated.Models.AddEditMember> viewModelList = new List<Models.AddEditMember>();

            //Retrieve all Member table records
            List<SeniorProject.Models.DataLayer.TableModels.Member> members = _SPDBContext.Members.ToList();
            //Go through each member add relavent info to the view model and pull the related IdentityUser info
            foreach(var member in members)
            {
                //Instantiate the single view model object
                SeniorProject.Areas.Authenticated.Models.AddEditMember viewModel = new Models.AddEditMember();
                //Go ahead and assign what data we can
                viewModel.MemberID = member.MemberID;
                viewModel.MemberRole = member.MemberRole;
                viewModel.ActiveStatus= member.ActiveStatus;
                viewModel.FirstName = member.FirstName;
                viewModel.LastName = member.LastName;
                viewModel.City= member.City;
                viewModel.State= member.State;
                //filter empty userID in the member table
                if(member.IdentityUserID != null && member.IdentityUserID != "")
                {
                    //There is a user id in the field. Pull the user info
                    IdentityUser user = await _userManager.FindByIdAsync(member.IdentityUserID);
                    //Filter for no user found
                    if(user != null)
                    {
                        //Assign info to the view model
                        viewModel.UserName = user.UserName;
                        viewModel.Email = user.Email;
                        viewModel.Phone = user.PhoneNumber;
                    }
                    else
                    {
                        //Assign empty strings
                        viewModel.UserName = string.Empty;
                        viewModel.Email = string.Empty;
                        viewModel.Phone = string.Empty;
                    }                    
                }
                else
                {
                    //No user id in member table
                    //Assign empty strings
                    viewModel.UserName = string.Empty;
                    viewModel.Email = string.Empty;
                    viewModel.Phone = string.Empty;
                }
                //Add the single view model to the list of view models
                viewModelList.Add(viewModel);
            }

            //Pass this list to the view 
            return View(viewModelList);
        }


        //Add an attribute for routing. Http method: get
        [HttpGet]
        public IActionResult AddMember()
        {
            //Adjust the viewbag data in the view to reflect the action 
            ViewBag.Action = "Add";
            //Create a new user model to pass to the view
            SeniorProject.Areas.Authenticated.Models.AddEditMember viewModel = new Models.AddEditMember();
            //Set all fields to defaults rather than null
            viewModel.MemberID = 0;
            viewModel.MemberRole = string.Empty;
            viewModel.ActiveStatus = false;
            viewModel.FirstName = string.Empty;
            viewModel.LastName = string.Empty;
            viewModel.City = string.Empty;
            viewModel.State = string.Empty;
            viewModel.UserName = string.Empty;
            viewModel.Email = string.Empty;
            viewModel.Phone = string.Empty;

            //Its more secure to avoid serving a list of user info to choose from. This should be known and typed in correctly
            //Pass empty view model
            return View("AddEditMember", viewModel);
        }
        
        
        //Add an attribute for routing. http method: get
        [HttpGet]
        public async Task<IActionResult> EditMemberAsync(int Id)
        {
            //Adjust the viewbag data in the view to reflect the action. The view for add and edit is multiuse.
            ViewBag.Action = "Edit";
            //Instantiate the view model
            SeniorProject.Areas.Authenticated.Models.AddEditMember viewModel = new Models.AddEditMember();

            //Retrieve the member info assocaited with the member Id passed in
            SeniorProject.Models.DataLayer.TableModels.Member member = _SPDBContext.Members.Find(Id);
            //Filter for no member found
            if(member == null)
            {
                //No member found
                //Go back to the summary view of the members and set the content in the element below the table indcating an error
                TempData["Change"] = "Sorry. No records found. Try again.";
                //Redirect 
                return RedirectToAction("ManageMembers", "Admin", new { area = "Authenticated" });
            }

            //A member was found
            //Go ahead and assign what data we can
            viewModel.MemberID = member.MemberID;
            viewModel.MemberRole = member.MemberRole;
            viewModel.ActiveStatus = member.ActiveStatus;
            viewModel.FirstName = member.FirstName;
            viewModel.LastName = member.LastName;
            viewModel.City = member.City;
            viewModel.State = member.State;

            //Filter for empty userId in members table
            if (member.IdentityUserID != null && member.IdentityUserID != "")
            {
                //There is a user id in the field. Pull the user info
                IdentityUser user = await _userManager.FindByIdAsync(member.IdentityUserID);
                //Filter for no user found
                if (user != null)
                {
                    //Assign info to the view model
                    viewModel.UserName = user.UserName;
                    viewModel.Email = user.Email;
                    viewModel.Phone = user.PhoneNumber;
                }
                else
                {
                    //Assign empty strings
                    viewModel.UserName = string.Empty;
                    viewModel.Email = string.Empty;
                    viewModel.Phone = string.Empty;
                }
            }
            else
            {
                //Assign empty strings
                viewModel.UserName = string.Empty;
                viewModel.Email = string.Empty;
                viewModel.Phone = string.Empty;
            }
            return View("AddEditMember", viewModel);
        }


        //Add an attribute for routing. http method: post 
        [HttpPost]
        public async Task<IActionResult> AddEditMemberAsync(SeniorProject.Areas.Authenticated.Models.AddEditMember viewModel)
        {
            //This method is used for both adding and editing:  post methods

            //Preform built in validation for the model
            if (ModelState.IsValid)
            {
                //Determine if adding or editing based on the Id value passed in
                if (viewModel.MemberID == 0)
                {
                    //Adding

                    //Instantiate a table record model 
                    SeniorProject.Models.DataLayer.TableModels.Member member = new SeniorProject.Models.DataLayer.TableModels.Member();

                    //Update all the member fields
                    member.MemberRole = viewModel.MemberRole;
                    member.ActiveStatus = viewModel.ActiveStatus;
                    member.FirstName= viewModel.FirstName;
                    member.LastName= viewModel.LastName;
                    member.City= viewModel.City;
                    member.State = viewModel.State;
                   
                    //Retrive a matching user id with the model input
                    IdentityUser user = await _userManager.FindByNameAsync(viewModel.UserName);
                    if(user != null)
                    {
                        //Tack on the id
                        member.IdentityUserID = user.Id;
                        //update the user phone and email if needed
                        bool changemade = false;
                        if(user.Email != viewModel.Email)
                        {
                            user.Email = viewModel.Email;
                            changemade= true;
                        }
                        if(user.PhoneNumber != viewModel.Phone) 
                        { 
                            user.PhoneNumber= viewModel.Phone;
                            changemade= true;
                        }
                        if(changemade)
                        {
                            //database Identity user table needs updating (AspNetUsers)
                            var resultUpdateUserInfo = await _userManager.UpdateAsync(user);
                            if(resultUpdateUserInfo.Succeeded)
                            {
                                //Good
                                //continue
                            }
                            else
                            {
                                //Set the Asp.net model state with all the errors
                                foreach (var error in resultUpdateUserInfo.Errors)
                                {
                                    ModelState.AddModelError("", error.Description);
                                }
                                return View("AddEditMember", viewModel);
                            }
                        }
                        
                    }
                    else
                    {
                        //Return an error to the add view
                        ModelState.AddModelError("No match", "Username not found. ");
                        return View("AddEditMember",viewModel);
                    }

                    //All filters did not produce an error                    
                    //Set the tempdata to add success
                    TempData["Change"] = "You successfully added " + member.FirstName + ".";
                    //Add the member record to the members table
                    _SPDBContext.Members.Add(member);
                    //Update the database
                    _SPDBContext.SaveChanges();
                    //Return to the summary table
                    return RedirectToAction("ManageMembers", "Manager", new { area = "Authenticated" });
                }
                else
                {
                    //Updating

                    //Instantiate a table record model 
                    SeniorProject.Models.DataLayer.TableModels.Member member = new SeniorProject.Models.DataLayer.TableModels.Member();

                    //Update all the member fields
                    member.MemberID= viewModel.MemberID;
                    member.MemberRole = viewModel.MemberRole;
                    member.ActiveStatus = viewModel.ActiveStatus;
                    member.FirstName = viewModel.FirstName;
                    member.LastName = viewModel.LastName;
                    member.City = viewModel.City;
                    member.State = viewModel.State;

                    //Retrive a matching user id with the model input
                    IdentityUser user = await _userManager.FindByNameAsync(viewModel.UserName);
                    if (user != null)
                    {
                        //Tack on the id
                        member.IdentityUserID = user.Id;
                        //update the user phone and email if needed
                        bool changemade = false;
                        if (user.Email != viewModel.Email)
                        {
                            user.Email = viewModel.Email;
                            changemade = true;
                        }
                        if (user.PhoneNumber != viewModel.Phone)
                        {
                            user.PhoneNumber = viewModel.Phone;
                            changemade = true;
                        }
                        if (changemade)
                        {
                            //database Identity user table needs updating (AspNetUsers)
                            var resultUpdateUserInfo = await _userManager.UpdateAsync(user);
                            if (resultUpdateUserInfo.Succeeded)
                            {
                                //Good
                                //continue
                            }
                            else
                            {
                                //Set the Asp.net model state with all the errors
                                foreach (var error in resultUpdateUserInfo.Errors)
                                {
                                    ModelState.AddModelError("", error.Description);
                                }
                                return View("AddEditMember", viewModel);
                            }
                        }

                    }
                    else
                    {
                        //Return an error to the add view
                        ModelState.AddModelError("No match", "Username not found. ");
                        return View("AddEditMember", viewModel);
                    }

                    //All filters did not produce an error                    
                    //Set the tempdata to update success
                    TempData["Change"] = "You successfully updated " + member.FirstName + ".";
                    //Update the member record in the members table
                    _SPDBContext.Members.Update(member);
                    //Update the database
                    _SPDBContext.SaveChanges();
                    //Return to the summary table
                    return RedirectToAction("ManageMembers", "Manager", new { area = "Authenticated" });

                }
                
            }
            else
            {
                //Model errors should add automatically
                //Return back to the addedit view
                return View("AddEditMember",viewModel);
            }
            

        }
        
        
        //Add an attribute for routing. http method: get
        [HttpGet]
        public IActionResult DeleteMember(int Id)
        {
            //Retrieve the record from the table in the database
            SeniorProject.Models.DataLayer.TableModels.Member member = _SPDBContext.Members.Find(Id);
            //Filter for none found
            if (member == null)
            {
                //None found
                //Return an error
                //Set the tempdata to delete fail
                TempData["Change"] = "Failed to remove member Id: " + Id;
                //Redirect to the summary page
                return RedirectToAction("ManageMembers", "Manager", new { area = "Authenticated" });
            }

            //The member was found 
            //Use the record found as the parameter entity to remove
            _SPDBContext.Members.Remove(member);
            //Commit the changes to the database using the dbcontext savechanges method
            _SPDBContext.SaveChanges();
            //Set the tempdata to delete success
            TempData["Change"] = "You successfully removed " + member.FirstName;
            //Reditect the summary page
            return RedirectToAction("ManageMembers", "Manager",new {area = "Authenticated"});
        }


        

    }
}
