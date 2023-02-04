
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
    [Authorize(Roles = "Admin,Manager,BasicMember")]

    public class BasicMemberController : Controller
    {
        //Add class level variables for services
        private SeniorProject.Models.DataLayer.SeniorProjectDBContext _SPDBContext { get; set; }
        private UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;

        public BasicMemberController(SeniorProject.Models.DataLayer.SeniorProjectDBContext SPDBContext, UserManager<IdentityUser> userManager,
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


        //Add an attribute for routing. Http method: get
        [HttpGet]
        public IActionResult AddEvent()
        {
            //Adjust the viewbag data in the view to reflect the action 
            ViewBag.Action = "Add";
            //Create a event model to pass to the view
            SeniorProject.Models.DataLayer.TableModels.Event viewModel = new SeniorProject.Models.DataLayer.TableModels.Event();
            //Set all fields to defaults rather than null
            viewModel.EventID = 0;
            viewModel.EventName = string.Empty;
            viewModel.EventDescription = string.Empty;
            viewModel.StartDateTime = DateTime.Now;
            viewModel.EndDateTime = DateTime.Now;
            viewModel.EventLocation = string.Empty;

         
            //Pass empty view model
            return View("AddEditEvent", viewModel);
        }

        //Add an attribute for routing. http method: get
        [HttpGet]
        public async Task<IActionResult> EditEventAsync(int Id)
        {
            //Adjust the viewbag data in the view to reflect the action. The view for add and edit is multiuse.
            ViewBag.Action = "Edit";
            //Instantiate the view model
            SeniorProject.Models.DataLayer.TableModels.Event viewModel = new SeniorProject.Models.DataLayer.TableModels.Event();

            //Retrieve the records info assocaited with the primary key Id passed in
            SeniorProject.Models.DataLayer.TableModels.Event record = _SPDBContext.Events.Find(Id);
            //Filter for no record found
            if (record == null)
            {
                //No record found
                //Go back to the summary view and set the content in the element below the table indcating an error
                TempData["Change"] = "Sorry. No records found. Try again.";
                //Redirect 
                return RedirectToAction("EventsSummary", "BasicMember", new { area = "Authenticated" });
            }

            //A record was found
            //Go ahead and assign what data we can
            viewModel.EventID = record.EventID;
            viewModel.EventName = record.EventName;
            viewModel.EventDescription = record.EventDescription;
            viewModel.StartDateTime = record.StartDateTime;
            viewModel.EndDateTime = record.EndDateTime;
            viewModel.EventLocation = record.EventLocation;
        
            //No related table info.

           
            return View("AddEditEvent", viewModel);
        }


        //Add an attribute for routing. http method: post 
        [HttpPost]
        public async Task<IActionResult> AddEditEventAsync(SeniorProject.Models.DataLayer.TableModels.Event viewModel)
        {
            //This method is used for both adding and editing:  post methods

            //Preform built in validation for the model
            if (ModelState.IsValid)
            {
                //Determine if adding or editing based on the Id value passed in
                if (viewModel.EventID == 0)
                {
                    //Adding

                    //Instantiate a table record model 
                    SeniorProject.Models.DataLayer.TableModels.Event record = new SeniorProject.Models.DataLayer.TableModels.Event();

                    //Update all the record fields
                    record.EventName = viewModel.EventName;
                    record.EventDescription = viewModel.EventDescription;
                    record.StartDateTime = viewModel.StartDateTime;
                    record.EndDateTime = viewModel.EndDateTime;
                    record.EventLocation = viewModel.EventLocation;


                    //Retrive a matching related record. None here.
                   

                    //All filters did not produce an error                    
                    //Set the tempdata to add success
                    TempData["Change"] = "You successfully added a event.";
                    //Add the record 
                    _SPDBContext.Events.Add(record);
                    //Update the database
                    _SPDBContext.SaveChanges();
                    //Return to the summary table
                    return RedirectToAction("EventsSummary", "BasicMember", new { area = "Authenticated" });
                }
                else
                {
                    //Updating

                    //Instantiate a table record model 
                    SeniorProject.Models.DataLayer.TableModels.Event record = new SeniorProject.Models.DataLayer.TableModels.Event();

                    //Update all the record fields
                    record.EventID = viewModel.EventID;
                    record.EventName = viewModel.EventName;
                    record.EventDescription = viewModel.EventDescription;
                    record.StartDateTime = viewModel.StartDateTime;
                    record.EndDateTime = viewModel.EndDateTime;
                    record.EventLocation = viewModel.EventLocation;

                    //Retrive a matching related record. No related records
                    

                    //All filters did not produce an error                    
                    //Set the tempdata to update success
                    TempData["Change"] = "You successfully updated a event record.";
                    //Update the record in the table
                    _SPDBContext.Events.Update(record);
                    //Update the database
                    _SPDBContext.SaveChanges();
                    //Return to the summary table
                    return RedirectToAction("EventsSummary", "BasicMember", new { area = "Authenticated" });

                }
            }
            else
            {
                //Model errors should add automatically
                //Return back to the addedit view
                return View("AddEditEvent", viewModel);
            }
        }


        //Add an attribute for routing. http method: get
        [HttpGet]
        public IActionResult DeleteEvent(int Id)
        {
            //Retrieve the record from the table in the database
            SeniorProject.Models.DataLayer.TableModels.Event record = _SPDBContext.Events.Find(Id);
            //Filter for none found
            if (record == null)
            {
                //None found
                //Return an error
                //Set the tempdata to delete fail
                TempData["Change"] = "Failed to remove due entry Id: " + Id;
                //Redirect to the summary page
                return RedirectToAction("EventsSummary", "BasicMember", new { area = "Authenticated" });
            }

            //The record was found
            //Use the record found as the parameter entity to remove
            _SPDBContext.Events.Remove(record);
            //Commit the changes to the database using the dbcontext savechanges method
            _SPDBContext.SaveChanges();
            //Set the tempdata to delete success
            TempData["Change"] = "You successfully removed event reocord ID " + record.EventID;
            //Reditect the summary page
            return RedirectToAction("EventsSummary", "BasicMember", new { area = "Authenticated" });
        }












    }
}
