
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
        public async Task<IActionResult> MembersSummaryAsync()
        {
            //Create a list of the viewModel to sent
            List<AddEditMember> viewModelList = new List<AddEditMember>();

            //Retrieve all Member table records
            List<SeniorProject.Models.DataLayer.TableModels.Member> members = _SPDBContext.Members.ToList();
            //Go through each member add relavent info to the view model and pull the related IdentityUser info
            foreach(var member in members)
            {
                //Instantiate the single view model object
                AddEditMember viewModel = new Models.Manager.AddEditMember();
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
            AddEditMember viewModel = new Models.Manager.AddEditMember();
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
            AddEditMember viewModel = new Models.Manager.AddEditMember();

            //Retrieve the member info assocaited with the member Id passed in
            SeniorProject.Models.DataLayer.TableModels.Member member = _SPDBContext.Members.Find(Id);
            //Filter for no member found
            if(member == null)
            {
                //No member found
                //Go back to the summary view of the members and set the content in the element below the table indcating an error
                TempData["Change"] = "Sorry. No records found. Try again.";
                //Redirect 
                return RedirectToAction("MembersSummary", "Manager", new { area = "Authenticated" });
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
        public async Task<IActionResult> AddEditMemberAsync(AddEditMember viewModel)
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
                    return RedirectToAction("MembersSummary", "Manager", new { area = "Authenticated" });
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
                    return RedirectToAction("MembersSummary", "Manager", new { area = "Authenticated" });

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
                return RedirectToAction("MembersSummary", "Manager", new { area = "Authenticated" });
            }

            //The member was found 
            //Use the record found as the parameter entity to remove
            _SPDBContext.Members.Remove(member);
            //Commit the changes to the database using the dbcontext savechanges method
            _SPDBContext.SaveChanges();
            //Set the tempdata to delete success
            TempData["Change"] = "You successfully removed " + member.FirstName;
            //Reditect the summary page
            return RedirectToAction("MembersSummary", "Manager",new {area = "Authenticated"});
        }










        [HttpGet]
        public async Task<IActionResult> DuesSummary()
        {
            //Create a list of the viewModel to send
            List<DuesViewModel> viewModelList = new List<DuesViewModel>();

            //Retrieve all dues table records
            List<SeniorProject.Models.DataLayer.TableModels.Due> dues = _SPDBContext.Dues.ToList();
            //Go through each due entry and add relavent info to the view model
            foreach (var due in dues)
            {
                //Instantiate the single view model object
                DuesViewModel viewModel = new Models.Manager.DuesViewModel();
                //Go ahead and assign what data we can
                viewModel.DueID = due.DueID;
                viewModel.AmountDue= due.AmountDue;
                viewModel.DateTimeDue= due.DateTimeDue;
                viewModel.AmountPaid= due.AmountPaid;
                viewModel.DateTimePaid= due.DateTimePaid;
                viewModel.PaymentMethod= due.PaymentMethod;
                viewModel.MemberID= due.MemberID;
                //filter empty foreign keys to members table
                if (due.MemberID != null && due.MemberID != 0)
                {
                    //There is a foreign key id in the field. Pull the related info
                    SeniorProject.Models.DataLayer.TableModels.Member relatedRecord = _SPDBContext.Members.Find(due.MemberID);
                    //Filter for none found
                    if (relatedRecord != null)
                    {
                        //Assign info to the view model
                        viewModel.MemberActiveStatus = relatedRecord.ActiveStatus;
                        viewModel.MemberFirstName= relatedRecord.FirstName;
                        viewModel.MemberLastName= relatedRecord.LastName;
                    }
                    else
                    {
                        //Assign empty strings if nothing was found
                        viewModel.MemberActiveStatus = false;
                        viewModel.MemberFirstName = relatedRecord.FirstName;
                        viewModel.MemberLastName = relatedRecord.LastName;
                    }
                }
                else
                {
                    //No foriegn key
                    //Assign empty strings
                    viewModel.MemberActiveStatus = false;
                    viewModel.MemberFirstName = string.Empty;
                    viewModel.MemberLastName = string.Empty;
                }
                //Add the single view model to the list of view models
                viewModelList.Add(viewModel);
            }

            //Pass this list to the view 
            return View(viewModelList);

        }


        //Add an attribute for routing. Http method: get
        [HttpGet]
        public IActionResult AddDue()
        {
            //Adjust the viewbag data in the view to reflect the action 
            ViewBag.Action = "Add";
            //Instantiate a new view model to pass to the view
            DuesViewModel viewModel = new Models.Manager.DuesViewModel();
            //Set all fields to defaults rather than null
            viewModel.DueID = 0;
            viewModel.AmountDue= 0;
            viewModel.DateTimeDue = DateTime.Now;
            viewModel.AmountPaid = 0;
            viewModel.DateTimePaid = DateTime.Now;
            viewModel.PaymentMethod = string.Empty;
            viewModel.MemberID= 0;
            viewModel.MemberActiveStatus= false;
            viewModel.MemberFirstName = string.Empty;
            viewModel.MemberLastName = string.Empty;

            //There is a search feature on the view with javascript for searching member names
            //Pass empty view model
            return View("AddEditDues", viewModel);
        }


        //Add an attribute for routing. http method: get
        [HttpGet]
        public async Task<IActionResult> EditDueAsync(int Id)
        {
            //Adjust the viewbag data in the view to reflect the action. The view for add and edit is multiuse.
            ViewBag.Action = "Edit";
            //Instantiate the view model
            DuesViewModel viewModel = new Models.Manager.DuesViewModel();

            //Retrieve the records info assocaited with the primary key Id passed in
            SeniorProject.Models.DataLayer.TableModels.Due record = _SPDBContext.Dues.Find(Id);
            //Filter for no record found
            if (record == null)
            {
                //No record found
                //Go back to the summary view and set the content in the element below the table indcating an error
                TempData["Change"] = "Sorry. No records found. Try again.";
                //Redirect 
                return RedirectToAction("DuesSummary", "Manager", new { area = "Authenticated" });
            }

            //A record was found
            //Go ahead and assign what data we can
            viewModel.DueID= record.DueID;
            viewModel.AmountDue= record.AmountDue;
            viewModel.DateTimeDue= record.DateTimeDue;
            viewModel.AmountPaid= record.AmountPaid;
            viewModel.DateTimePaid= record.DateTimePaid;
            viewModel.PaymentMethod= record.PaymentMethod;
            viewModel.MemberID= record.MemberID;
            

            //Filter for empty foreign key
            if (record.MemberID != null && record.MemberID != 0)
            {
                //There is a foreign key. Get that record
                SeniorProject.Models.DataLayer.TableModels.Member relatedRecord = _SPDBContext.Members.Find(record.MemberID);
                //Filter for no record found
                if (relatedRecord != null)
                {
                    //Assign info to the view model
                    viewModel.MemberActiveStatus = relatedRecord.ActiveStatus;
                    viewModel.MemberFirstName= relatedRecord.FirstName;
                    viewModel.MemberLastName= relatedRecord.LastName;
                }
                else
                {
                    //No record found
                    //Assign defaults
                    viewModel.MemberActiveStatus = false;
                    viewModel.MemberFirstName = string.Empty;
                    viewModel.MemberLastName= string.Empty;
                }
            }
            else
            {
                //No foreign key
                //Assign defaults
                viewModel.MemberActiveStatus = false;
                viewModel.MemberFirstName = string.Empty;
                viewModel.MemberLastName = string.Empty;
            }
            return View("AddEditDues", viewModel);
        }


        //Add an attribute for routing. http method: post 
        [HttpPost]
        public async Task<IActionResult> AddEditDuesAsync(DuesViewModel viewModel)
        {
            //This method is used for both adding and editing:  post methods

            //Preform built in validation for the model
            if (ModelState.IsValid)
            {
                //Determine if adding or editing based on the Id value passed in
                if (viewModel.DueID == 0)
                {
                    //Adding

                    //Instantiate a table record model 
                    SeniorProject.Models.DataLayer.TableModels.Due record = new SeniorProject.Models.DataLayer.TableModels.Due();

                    //Update all the record fields
                    record.AmountDue= viewModel.AmountDue;
                    record.DateTimeDue= viewModel.DateTimeDue.GetValueOrDefault();
                    record.AmountPaid= viewModel.AmountPaid;
                    record.DateTimePaid= viewModel.DateTimePaid.GetValueOrDefault();
                    record.PaymentMethod = viewModel.PaymentMethod;
                   

                    //Retrive a matching related record
                    SeniorProject.Models.DataLayer.TableModels.Member relatedRecord = _SPDBContext.Members.Find(viewModel.MemberID);
                    if (relatedRecord != null)
                    {
                        //Match found
                        //Add the now validated foriegn key to the record
                        record.MemberID = relatedRecord.MemberID;
                        //update anything  in the related record that can be changed from this view/access
                        //This is only the active status. Restrict name change to another view.
                        bool changemade = false;
                        if (relatedRecord.ActiveStatus != viewModel.MemberActiveStatus)
                        {
                            relatedRecord.ActiveStatus = viewModel.MemberActiveStatus.GetValueOrDefault();
                            changemade = true;
                        }
                        //Determing it the related table needs to be updated                       
                        if (changemade)
                        {
                            //Update the related table
                            _SPDBContext.Members.Update(relatedRecord);
                            _SPDBContext.SaveChanges();
                            
                        }

                    }
                    else
                    {
                        //Return an error to the add view
                        ModelState.AddModelError("No match", "Member not found. ");
                        return View("AddEditDues", viewModel);
                    }

                    //All filters did not produce an error                    
                    //Set the tempdata to add success
                    TempData["Change"] = "You successfully added a due entry.";
                    //Add the record 
                    _SPDBContext.Dues.Add(record);
                    //Update the database
                    _SPDBContext.SaveChanges();
                    //Return to the summary table
                    return RedirectToAction("DuesSummary", "Manager", new { area = "Authenticated" });
                }
                else
                {
                    //Updating

                    //Instantiate a table record model 
                    SeniorProject.Models.DataLayer.TableModels.Due record = new SeniorProject.Models.DataLayer.TableModels.Due();

                    //Update all the record fields
                    record.DueID= viewModel.DueID;
                    record.AmountDue = viewModel.AmountDue;
                    record.DateTimeDue = viewModel.DateTimeDue.GetValueOrDefault();
                    record.AmountPaid = viewModel.AmountPaid;
                    record.DateTimePaid = viewModel.DateTimePaid.GetValueOrDefault();
                    record.PaymentMethod = viewModel.PaymentMethod;

                    //Retrive a matching related record
                    SeniorProject.Models.DataLayer.TableModels.Member relatedRecord = _SPDBContext.Members.Find(viewModel.MemberID);
                    if (relatedRecord != null)
                    {
                        //Match found
                        //Add the now validated foriegn key to the record
                        record.MemberID = relatedRecord.MemberID;
                        //update anything that can be changed form this view/access
                        //This is only the active status. Restrict name change to another view.
                        bool changemade = false;
                        if (relatedRecord.ActiveStatus != viewModel.MemberActiveStatus)
                        {
                            relatedRecord.ActiveStatus = viewModel.MemberActiveStatus.GetValueOrDefault();
                            changemade = true;
                        }
                        //Determing it the related table needs to be updated                       
                        if (changemade)
                        {
                            //Update the related table
                            _SPDBContext.Members.Update(relatedRecord);
                            _SPDBContext.SaveChanges();

                        }

                    }
                    else
                    {
                        //Return an error to the add view
                        ModelState.AddModelError("No match", "Member not found. ");
                        return View("AddEditDues", viewModel);
                    }

                    //All filters did not produce an error                    
                    //Set the tempdata to update success
                    TempData["Change"] = "You successfully updated a due record.";
                    //Update the record in the table
                    _SPDBContext.Dues.Update(record);
                    //Update the database
                    _SPDBContext.SaveChanges();
                    //Return to the summary table
                    return RedirectToAction("DuesSummary", "Manager", new { area = "Authenticated" });

                }
            }
            else
            {
                //Model errors should add automatically
                //Return back to the addedit view
                return View("AddEditDues", viewModel);
            }
        }


        //Add an attribute for routing. http method: get
        [HttpGet]
        public IActionResult DeleteDue(int Id)
        {
            //Retrieve the record from the table in the database
            SeniorProject.Models.DataLayer.TableModels.Due record = _SPDBContext.Dues.Find(Id);
            //Filter for none found
            if (record == null)
            {
                //None found
                //Return an error
                //Set the tempdata to delete fail
                TempData["Change"] = "Failed to remove due entry Id: " + Id;
                //Redirect to the summary page
                return RedirectToAction("DuesSummary", "Manager", new { area = "Authenticated" });
            }

            //The record was found
            //Use the record found as the parameter entity to remove
            _SPDBContext.Dues.Remove(record);
            //Commit the changes to the database using the dbcontext savechanges method
            _SPDBContext.SaveChanges();
            //Set the tempdata to delete success
            TempData["Change"] = "You successfully removed due reocord ID " + record.DueID;
            //Reditect the summary page
            return RedirectToAction("DuesSummary", "Manager", new { area = "Authenticated" });
        }



        //The add or edit will have a search box for members records, search by names
        //There will be a javascript ajax post in this controller that returns the match or no results found
        //When match is found a btn javascipt fills in the form with the matching member info
        //User then has to hit submit on form to actually add or edit the due record
        //Ensure authentication on this works
        //The dues due date can be javascript as well once the date payed changes.











    }
}
