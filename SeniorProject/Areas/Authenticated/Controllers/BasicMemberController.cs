
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

        #region Events
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

        #endregion Events

        #region Donations



        //Add an attribute for routing. Below that is the Index action 
        [HttpGet]
        public async Task<IActionResult> DonationsSummaryAsync()
        {
            //Create a list of the viewModel to send. 
            List<SeniorProject.Areas.Authenticated.Models.BasicMember.DonationViewModel> viewModelList = new List<Models.BasicMember.DonationViewModel>();

            //Retrieve all base table records
            List<SeniorProject.Models.DataLayer.TableModels.Donation> tableRecords = _SPDBContext.Donations.ToList();
            //Retrieve all related table records
            List<SeniorProject.Models.DataLayer.TableModels.Member> relatedRecords = _SPDBContext.Members.ToList();

            //Pull in related info by matching
            foreach(var record in tableRecords)
            {
                //Instantiate a single view model record
                SeniorProject.Areas.Authenticated.Models.BasicMember.DonationViewModel viewModel = new Models.BasicMember.DonationViewModel();
                //Assign what is already available
                viewModel.DonationID = record.DonationID;
                viewModel.DonationAmmount = record.DonationAmmount;
                viewModel.DonationDate = record.DonationDate;
                viewModel.DonorNameOrTitle = record.DonorNameOrTitle;
                viewModel.MemberID= record.MemberID;
                //Retrive the matching related table record
                SeniorProject.Models.DataLayer.TableModels.Member relatedRecord = _SPDBContext.Members.Find(record.MemberID);
                //Assign the related data to the view model
                viewModel.MemberFirstName = relatedRecord.FirstName;
                viewModel.MemberLastName = relatedRecord.LastName;

                //View model complete. Add it to the list of view models
                viewModelList.Add(viewModel);
            }


            //Pass this list to the view 
            return View(viewModelList);
        }


        //Add an attribute for routing. Http method: get
        [HttpGet]
        public IActionResult AddDonation()
        {
            //Adjust the viewbag data in the view to reflect the action 
            ViewBag.Action = "Add";
            //Create a event model to pass to the view
            SeniorProject.Areas.Authenticated.Models.BasicMember.DonationViewModel viewModel = new Models.BasicMember.DonationViewModel();
            //Set all fields to defaults rather than null
            viewModel.DonationID = 0;
            viewModel.DonationAmmount = 0.0;
            viewModel.DonationDate = DateTime.Now;
            viewModel.DonorNameOrTitle = string.Empty;
            viewModel.MemberFirstName = string.Empty;
            viewModel.MemberLastName = string.Empty;
            viewModel.MemberID = 0;


            //Pass empty view model
            return View("AddEditDonation", viewModel);
        }

        //Add an attribute for routing. http method: get
        [HttpGet]
        public async Task<IActionResult> EditDonationAsync(int Id)
        {
            //Adjust the viewbag data in the view to reflect the action. The view for add and edit is multiuse.
            ViewBag.Action = "Edit";
            //Instantiate the view model
            SeniorProject.Areas.Authenticated.Models.BasicMember.DonationViewModel viewModel = new Models.BasicMember.DonationViewModel();

            //Retrieve the base table record's info assocaited with the primary key Id passed in
            SeniorProject.Models.DataLayer.TableModels.Donation record = _SPDBContext.Donations.Find(Id);
            //Filter for no record found
            if (record == null)
            {
                //No record found
                //Go back to the summary view and set the content in the element below the table indcating an error
                TempData["Change"] = "Sorry. No records found. Try again.";
                //Redirect 
                return RedirectToAction("DonationsSummary", "BasicMember", new { area = "Authenticated" });
            }

            //A record was found
            //Go ahead and assign what data we can
            viewModel.DonationID = record.DonationID;
            viewModel.DonationAmmount = record.DonationAmmount;
            viewModel.DonationDate = record.DonationDate;
            viewModel.DonorNameOrTitle = record.DonorNameOrTitle;
            viewModel.MemberID= record.MemberID;

            //Get related table info.
            SeniorProject.Models.DataLayer.TableModels.Member relatedRecord = _SPDBContext.Members.Find(record.MemberID);
            viewModel.MemberFirstName = relatedRecord.FirstName;
            viewModel.MemberLastName = relatedRecord.LastName;


            return View("AddEditDonation", viewModel);
        }


        //Add an attribute for routing. http method: post 
        [HttpPost]
        public async Task<IActionResult> AddEditDonationAsync(SeniorProject.Areas.Authenticated.Models.BasicMember.DonationViewModel viewModel)
        {
            //This method is used for both adding and editing:  post methods

            //Determine if adding or editing based on the Id value passed in
            if (viewModel.DonationID == 0)
            {
                //Adding
                if (!ModelState.IsValid)
                {
                    ViewBag.Action = "Add";
                    //Model errors should add automatically
                    //Return back to the addedit view
                    return View("AddEditDonation", viewModel);
                }


                //Instantiate a table record model 
                SeniorProject.Models.DataLayer.TableModels.Donation record = new SeniorProject.Models.DataLayer.TableModels.Donation();

                //Update all the record fields
                record.DonationAmmount = viewModel.DonationAmmount;
                record.DonationDate = viewModel.DonationDate;
                record.DonorNameOrTitle = viewModel.DonorNameOrTitle;
                record.MemberID = viewModel.MemberID;

                //Retrive a matching related record.
                SeniorProject.Models.DataLayer.TableModels.Member relatedRecord = _SPDBContext.Members.Find(viewModel.MemberID);

                if (relatedRecord == null)
                {
                    ModelState.AddModelError("", "No member found");
                    return View("AddEditDonation", viewModel);
                }
                else
                {
                    record.Member = relatedRecord;
                }

                //All filters did not produce an error                    
                //Set the tempdata to add success
                TempData["Change"] = "You successfully added a donation.";
                //Add the record 
                _SPDBContext.Donations.Add(record);
                //Update the database
                _SPDBContext.SaveChanges();
                //Return to the summary table
                return RedirectToAction("DonationsSummary", "BasicMember", new { area = "Authenticated" });
            }
            else
            {
                //Updating
                if (!ModelState.IsValid)
                {
                    ViewBag.Action = "Edit";
                    //Model errors should add automatically
                    //Return back to the addedit view
                    return View("AddEditDonation", viewModel);
                }

                //Instantiate a table record model 
                SeniorProject.Models.DataLayer.TableModels.Donation record = new SeniorProject.Models.DataLayer.TableModels.Donation();

                //Update all the record fields
                record.DonationID = viewModel.DonationID;
                record.DonationAmmount = viewModel.DonationAmmount;
                record.DonationDate = viewModel.DonationDate;
                record.DonorNameOrTitle = viewModel.DonorNameOrTitle;
                record.MemberID = viewModel.MemberID;

                //Retrive a matching related record.
                SeniorProject.Models.DataLayer.TableModels.Member relatedRecord = _SPDBContext.Members.Find(viewModel.MemberID);

                if (relatedRecord == null)
                {
                    ModelState.AddModelError("", "No member found");
                    return View("AddEditDonation", viewModel);
                }
                else
                {
                    record.Member = relatedRecord;
                }

                //All filters did not produce an error                    
                //Set the tempdata to update success
                TempData["Change"] = "You successfully updated a donation record.";
                //Update the record in the table
                _SPDBContext.Donations.Update(record);
                //Update the database
                _SPDBContext.SaveChanges();
                //Return to the summary table
                return RedirectToAction("DonationsSummary", "BasicMember", new { area = "Authenticated" });

            }
        }


        //Add an attribute for routing. http method: get
        [HttpGet]
        public IActionResult DeleteDonation(int Id)
        {
            //Retrieve the record from the table in the database
            SeniorProject.Models.DataLayer.TableModels.Donation record = _SPDBContext.Donations.Find(Id);
            //Filter for none found
            if (record == null)
            {
                //None found
                //Return an error
                //Set the tempdata to delete fail
                TempData["Change"] = "Failed to remove donation entry Id: " + Id;
                //Redirect to the summary page
                return RedirectToAction("DonationsSummary", "BasicMember", new { area = "Authenticated" });
            }

            //The record was found
            //Use the record found as the parameter entity to remove
            _SPDBContext.Donations.Remove(record);
            //Commit the changes to the database using the dbcontext savechanges method
            _SPDBContext.SaveChanges();
            //Set the tempdata to delete success
            TempData["Change"] = "You successfully removed donation record ID " + record.DonationID;
            //Reditect the summary page
            return RedirectToAction("DonationsSummary", "BasicMember", new { area = "Authenticated" });
        }



        #endregion Donations



        #region EventInvitations


        //Add an attribute for routing. Below that is the Index action 
        [HttpGet]
        public async Task<IActionResult> EventInvitationsSummaryAsync()
        {
            //Create a list of the viewModel to send. 
            List<SeniorProject.Areas.Authenticated.Models.BasicMember.EventInvitationsViewModel> viewModelList = new List<Models.BasicMember.EventInvitationsViewModel>();

            //Retrieve all base table records
            List<SeniorProject.Models.DataLayer.TableModels.EventInvitation> tableRecords = _SPDBContext.EventInvitations.ToList();
            //Reuse a variables for the member table records
            SeniorProject.Models.DataLayer.TableModels.Event relatedRecord_Event;
            SeniorProject.Models.DataLayer.TableModels.Member relatedRecord_Member;

            //Pull in related info by matching
            foreach (var record in tableRecords)
            {
                //Instantiate a single view model record
                SeniorProject.Areas.Authenticated.Models.BasicMember.EventInvitationsViewModel viewModel = new Models.BasicMember.EventInvitationsViewModel();
                //Assign what is already available
                viewModel.EventInvitationID = record.EventInvitationID;
                viewModel.InvitationBody= record.InvitationBody;
                viewModel.InvitationResponseBody= record.InvitationResponseBody;
                viewModel.EventID = record.EventID;
                //Retrieve the matching related table record: Event
                relatedRecord_Event = _SPDBContext.Events.Find(record.EventID);
                //Assign related data to the view model
                viewModel.EventName = relatedRecord_Event.EventName;
                viewModel.EventLocation = relatedRecord_Event.EventLocation;
                viewModel.StartDateTime = relatedRecord_Event.StartDateTime;
                viewModel.EndDateTime = relatedRecord_Event.EndDateTime;
                
                viewModel.MemberID_Sender = record.MemberID_Sender;
                //Retrive the matching related table record: Member Sender
                relatedRecord_Member = _SPDBContext.Members.Find(record.MemberID_Sender);
                //Assign the related data to the view model
                viewModel.MemberFirstName_Sender = relatedRecord_Member.FirstName;
                viewModel.MemberLastName_Sender = relatedRecord_Member.LastName;


                viewModel.MemberID_Reciever = record.MemberID_Reciever;
                //Retrive the matching related table record: Member Reciever            
                relatedRecord_Member = _SPDBContext.Members.Find(record.MemberID_Reciever);
                viewModel.MemberFirstName_Reciever = relatedRecord_Member.FirstName;
                viewModel.MemberLastName_Reciever = relatedRecord_Member.LastName;

                //View model complete. Add it to the list of view models
                viewModelList.Add(viewModel);
            }


            //Pass this list to the view 
            return View(viewModelList);
        }


        //Add an attribute for routing. Http method: get
        [HttpGet]
        public IActionResult AddEventInvitation()
        {
            //Adjust the viewbag data in the view to reflect the action 
            ViewBag.Action = "Add";
            //Create a event model to pass to the view
            SeniorProject.Areas.Authenticated.Models.BasicMember.EventInvitationsViewModel viewModel = new Models.BasicMember.EventInvitationsViewModel();
            //Set all fields to defaults rather than null
            //zero here will flag the POST method on if this is a add or edit
            viewModel.EventInvitationID = 0;
            viewModel.InvitationBody = string.Empty;
            viewModel.InvitationResponseBody = string.Empty;
            viewModel.EventName = string.Empty;
            viewModel.EventLocation = string.Empty;
            viewModel.StartDateTime = DateTime.Now;
            viewModel.EndDateTime= DateTime.Now;
            viewModel.EventID = 0;
            viewModel.MemberFirstName_Sender = string.Empty;
            viewModel.MemberLastName_Sender = string.Empty;
            viewModel.MemberID_Sender = 0;
            viewModel.MemberFirstName_Reciever = string.Empty;
            viewModel.MemberLastName_Reciever = string.Empty;
            viewModel.MemberID_Reciever = 0;


            //Pass empty view model
            return View("AddEditEventInvitation", viewModel);
        }

        //Add an attribute for routing. http method: get
        [HttpGet]
        public async Task<IActionResult> EditEventInvitationAsync(int Id)
        {
            //Adjust the viewbag data in the view to reflect the action. The view for add and edit is multiuse.
            ViewBag.Action = "Edit";
            //Instantiate the view model
            SeniorProject.Areas.Authenticated.Models.BasicMember.EventInvitationsViewModel viewModel = new Models.BasicMember.EventInvitationsViewModel();

            //Retrieve the base table record's info assocaited with the primary key Id passed in
            SeniorProject.Models.DataLayer.TableModels.EventInvitation record = _SPDBContext.EventInvitations.Find(Id);
            //Filter for no record found
            if (record == null)
            {
                //No record found
                //Go back to the summary view and set the content in the element below the table indcating an error
                TempData["Change"] = "Sorry. No records found. Try again.";
                //Redirect 
                return RedirectToAction("EventInvitationsSummary", "BasicMember", new { area = "Authenticated" });
            }

            //A record was found. Assign this data to the view model
            viewModel.EventInvitationID = record.EventInvitationID;
            viewModel.InvitationBody = record.InvitationBody;
            viewModel.InvitationResponseBody = record.InvitationResponseBody;
            
            //Retrieve related info: Event
            SeniorProject.Models.DataLayer.TableModels.Event relatedRecord_Event = _SPDBContext.Events.Find(record.EventID);
            viewModel.EventName = relatedRecord_Event.EventName;
            viewModel.EventLocation = relatedRecord_Event.EventLocation;
            viewModel.StartDateTime = relatedRecord_Event.StartDateTime;
            viewModel.EndDateTime = relatedRecord_Event.EndDateTime;
            viewModel.EventID = relatedRecord_Event.EventID;

            //Retrieve related info: Member Sender
            viewModel.MemberID_Sender = record.MemberID_Sender;
            SeniorProject.Models.DataLayer.TableModels.Member relatedRecord_Member = _SPDBContext.Members.Find(record.MemberID_Sender);
            viewModel.MemberFirstName_Sender = relatedRecord_Member.FirstName;
            viewModel.MemberLastName_Sender = relatedRecord_Member.LastName;

            //Retrieve related info: Member Reciever
            viewModel.MemberID_Reciever = record.MemberID_Reciever;
            relatedRecord_Member = _SPDBContext.Members.Find(record.MemberID_Reciever);
            viewModel.MemberFirstName_Reciever = relatedRecord_Member.FirstName;
            viewModel.MemberLastName_Reciever = relatedRecord_Member.LastName;

            return View("AddEditEventInvitation", viewModel);
        }


        //Add an attribute for routing. http method: post 
        [HttpPost]
        public async Task<IActionResult> AddEditEventInvitationAsync(SeniorProject.Areas.Authenticated.Models.BasicMember.EventInvitationsViewModel viewModel)
        {
            //This method is used for both adding and editing:  post methods

            //Determine if adding or editing based on the Id value passed in
            if (viewModel.EventInvitationID == 0)
            {
                //Adding
                if (!ModelState.IsValid)
                {
                    ViewBag.Action = "Add";
                    //Model errors should add automatically
                    //Return back to the addedit view
                    return View("AddEditEventInvitation", viewModel);
                }


                //Instantiate a table record model 
                SeniorProject.Models.DataLayer.TableModels.EventInvitation record = new SeniorProject.Models.DataLayer.TableModels.EventInvitation();

                //Update all the record fields
                record.InvitationBody = viewModel.InvitationBody;
                record.InvitationResponseBody = viewModel.InvitationResponseBody;
                record.EventID = viewModel.EventID;
                //Retrieve a matching related record
                record.Event = _SPDBContext.Events.Find(viewModel.EventID);
                record.MemberID_Sender = viewModel.MemberID_Sender;
                record.Member_Sender = _SPDBContext.Members.Find(viewModel.MemberID_Sender);
                record.MemberID_Reciever = viewModel.MemberID_Reciever;
                record.Member_Reciever = _SPDBContext.Members.Find(viewModel.MemberID_Reciever);


               

                //All filters did not produce an error                    
                //Set the tempdata to add success
                TempData["Change"] = "You successfully added a event invitation.";
                //Add the record 
                _SPDBContext.EventInvitations.Add(record);
                //Update the database
                _SPDBContext.SaveChanges();
                //Return to the summary table
                return RedirectToAction("EventInvitationsSummary", "BasicMember", new { area = "Authenticated" });
            }
            else
            {
                //Updating
                if (!ModelState.IsValid)
                {
                    ViewBag.Action = "Edit";
                    //Model errors should add automatically
                    //Return back to the addedit view
                    return View("AddEditEventInvitation", viewModel);
                }

                //Instantiate a table record model 
                SeniorProject.Models.DataLayer.TableModels.EventInvitation record = _SPDBContext.EventInvitations.Find(viewModel.EventInvitationID);

                //Update all the record fields
                record.InvitationBody = viewModel.InvitationBody;
                record.InvitationResponseBody = viewModel.InvitationResponseBody;
                record.EventID = viewModel.EventID;
                //Retrieve a matching related record
                record.Event = _SPDBContext.Events.Find(viewModel.EventID);
                record.MemberID_Sender = viewModel.MemberID_Sender;
                record.Member_Sender = _SPDBContext.Members.Find(viewModel.MemberID_Sender);
                record.MemberID_Reciever = viewModel.MemberID_Reciever;
                record.Member_Reciever = _SPDBContext.Members.Find(viewModel.MemberID_Reciever);

                

                //All filters did not produce an error                    
                //Set the tempdata to update success
                TempData["Change"] = "You successfully updated a event invitation record.";
                //Update the record in the table
                _SPDBContext.EventInvitations.Update(record);
                //Update the database
                _SPDBContext.SaveChanges();
                //Return to the summary table
                return RedirectToAction("EventInvitationsSummary", "BasicMember", new { area = "Authenticated" });

            }
        }


        //Add an attribute for routing. http method: get
        [HttpGet]
        public IActionResult DeleteEventInvitation(int Id)
        {
            //Retrieve the record from the table in the database
            SeniorProject.Models.DataLayer.TableModels.EventInvitation record = _SPDBContext.EventInvitations.Find(Id);
            //Filter for none found
            if (record == null)
            {
                //None found
                //Return an error
                //Set the tempdata to delete fail
                TempData["Change"] = "Failed to remove event invitation entry Id: " + Id;
                //Redirect to the summary page
                return RedirectToAction("EventInvitationsSummary", "BasicMember", new { area = "Authenticated" });
            }

            //The record was found
            //Use the record found as the parameter entity to remove
            _SPDBContext.EventInvitations.Remove(record);
            //Commit the changes to the database using the dbcontext savechanges method
            _SPDBContext.SaveChanges();
            //Set the tempdata to delete success
            TempData["Change"] = "You successfully removed event invitation record ID " + record.EventInvitationID;
            //Reditect the summary page
            return RedirectToAction("EventInvitationsSummary", "BasicMember", new { area = "Authenticated" });
        }



        #endregion Donations








    }
}
