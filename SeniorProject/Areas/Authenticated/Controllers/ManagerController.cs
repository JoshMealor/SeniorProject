
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeniorProject.Models;
using System.Security.Cryptography.Xml;

namespace SeniorProject.Areas.Authenticated.Controlllers
{
    

    //Add the routing attribute for the Admin area.This applies to all the actions in the controller. Also add the authorization middleware. It will redirect to login if not Authenticated with an account
    [Area("Authenticated")]
    [Authorize(Roles = "Manager")]

    public class ManagerController : Controller
    {
        //Add a class level variable to hold the dbcontext reference passed in via dependency injection
        private Models.DataLayer.SeniorProjectDBContext _SPDBContext { get; set; }

        public ManagerController(Models.DataLayer.SeniorProjectDBContext SPDBContext)
        {
            //assign the local dbcontext variable the dependcy injected dbcontext object
            this._SPDBContext = SPDBContext;
        }

        //Add an attribute for routing. Below that is the Index action 
        [HttpGet]
        public IActionResult ManageMembers()
        {
            //Retrieve all User records
            List<Models.DataLayer.TableModels.Member> members = _SPDBContext.Members.ToList();
            //Pass this list to the view 
            return View(members);
        }


        //Add an attribute for routing. Below that is the Add action 
        [HttpGet]
        public IActionResult AddMember()
        {
            //Adjust the viewbag data in the view to reflect the action 
            ViewBag.Action = "Add";
            //Create a new user model to pass to the view
            Models.DataLayer.TableModels.Member member = new Models.DataLayer.TableModels.Member();
            return View("AddEditMember", member);
        }
        //Add an attribute for routing. Below that is the Edit action 
        [HttpGet]
        public IActionResult EditMember(int Id)
        {
            //Adjust the viewbag data in the view to reflect the action 
            ViewBag.Action = "Edit";
            Models.DataLayer.TableModels.Member member = new Models.DataLayer.TableModels.Member();
            member = _SPDBContext.Members.Find(Id);            
            return View("AddEditMember", member);
        }
        //Add an attribute for routing. Below that is the AddEdit action 
        [HttpPost]
        public IActionResult AddEditMember(Models.DataLayer.TableModels.Member member)
        {

            //Preform built in validation for the model
            if (ModelState.IsValid)
            {
                //Determine if adding or editting based on the Id value passed in
                if (member.MemberID == 0)
                {
                    //Adding


                    //Set the tempdata to add success
                    TempData["Change"] = "You successfully added " + member.FirstName + ".";


                    //Add the new heffer model to the dbcontext
                    _SPDBContext.Members.Add(member);
                }
                else
                {
                    //Updating

                    //Set the tempdata to update success
                    TempData["Change"] = "You successfully edited member ";

                    //Add the updated model to the dbcontext
                    _SPDBContext.Members.Update(member);
                }
                //Commit the changes to the datbase with the dbcontext savechanges method
                _SPDBContext.SaveChanges();
                //Redirect the user to the controller index action
                return RedirectToAction("ManageMembers", "Manager",new {area = "Authenticated"});
            }
            else
            {
                //Bad data. Return the view with the same data so the user can tweak and try again

                //Set the tempdata to add/update fail
                TempData["Change"] = "This change failed. Please try again";

                return View(member);
            }
        }
        //Add an attribute for routing. Below that is the Delete action 
        [HttpGet]
        public IActionResult DeleteMember(int Id)
        {
            //Retrieve the heffer record based on the Id from the database and assign this to a new heffer model
            Models.DataLayer.TableModels.Member member = _SPDBContext.Members.Find(Id);

            //Set the tempdata to delete success
            TempData["Change"] = "You successfully removed " + member.FirstName;

            //Use this heffer model to remove the matching heffer record in the dbcontext
            _SPDBContext.Members.Remove(member);
            //Commit the changes to the database using the dbcontext savechanges method
            _SPDBContext.SaveChanges();
            //Reditect the user to the heffer controller index action
            return RedirectToAction("ManageMembers", "Manager",new {area = "Authenticated"});
        }


        

    }
}
