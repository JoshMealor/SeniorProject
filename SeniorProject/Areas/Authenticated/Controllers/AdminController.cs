
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeniorProject.Models;
using System.Security.Cryptography.Xml;

namespace SeniorProject.Areas.Authenticated.Controlllers
{
    

    //Add the routing attribute for the Admin area.This applies to all the actions in the controller. Also add the authorization middleware. It will redirect to login if not Authenticated with an account
    [Area("Authenticated")]
    [Authorize(Roles = "Admin")]

    public class AdminController : Controller
    {
        //Add a class level variable to hold the dbcontext reference passed in via dependency injection
        private Models.DataLayer.SeniorProjectDBContext _SPDBContext { get; set; }

        public AdminController(Models.DataLayer.SeniorProjectDBContext SPDBContext)
        {
            //assign the local dbcontext variable the dependcy injected dbcontext object
            this._SPDBContext = SPDBContext;
        }

        //Add an attribute for routing. Below that is the Index action 
        [HttpGet]
        public IActionResult ManageUsers()
        {
            //Retrieve all User records
            List<Models.DataLayer.User> users = _SPDBContext.Users.ToList();
            //Pass this list to the view 
            return View(users);
        }


        //Add an attribute for routing. Below that is the Add action 
        [HttpGet]
        public IActionResult AddUser()
        {
            //Adjust the viewbag data in the view to reflect the action 
            ViewBag.Action = "Add";
            //Create a new user model to pass to the view
            Models.DataLayer.User user = new Models.DataLayer.User();
            return View("AddEditUser", user);
        }
        //Add an attribute for routing. Below that is the Edit action 
        [HttpGet]
        public IActionResult EditUser(int Id)
        {
            //Adjust the viewbag data in the view to reflect the action 
            ViewBag.Action = "Edit";
            Models.DataLayer.User user = new Models.DataLayer.User();
            user = _SPDBContext.Users.Find(Id);            
            return View("AddEditUser", user);
        }
        //Add an attribute for routing. Below that is the AddEdit action 
        [HttpPost]
        public IActionResult AddEditUser(Models.DataLayer.User user)
        {

            //Preform built in validation for the model
            if (ModelState.IsValid)
            {
                //Determine if adding or editting based on the Id value passed in
                if (user.Id == "")
                {
                    //Adding


                    //Set the tempdata to add success
                    TempData["Change"] = "You successfully added " + user.UserName + ".";


                    //Add the new heffer model to the dbcontext
                    _SPDBContext.Users.Add(user);
                }
                else
                {
                    //Updating

                    //Set the tempdata to update success
                    TempData["Change"] = "You successfully edited user ";

                    //Add the updated model to the dbcontext
                    _SPDBContext.Users.Update(user);
                }
                //Commit the changes to the datbase with the dbcontext savechanges method
                _SPDBContext.SaveChanges();
                //Redirect the user to the controller index action
                return RedirectToAction("Index", "ManageUsers");
            }
            else
            {
                //Bad data. Return the view with the same data so the user can tweak and try again

                //Set the tempdata to add/update fail
                TempData["Change"] = "This change failed. Please try again";

                return View(user);
            }
        }
        //Add an attribute for routing. Below that is the Delete action 
        [HttpGet]
        public IActionResult DeleteUser(int Id)
        {
            //Retrieve the heffer record based on the Id from the database and assign this to a new heffer model
            Models.DataLayer.User user = _SPDBContext.Users.Find(Id);

            //Set the tempdata to delete success
            TempData["Change"] = "You successfully removed " + user.UserName;

            //Use this heffer model to remove the matching heffer record in the dbcontext
            _SPDBContext.Users.Remove(user);
            //Commit the changes to the database using the dbcontext savechanges method
            _SPDBContext.SaveChanges();
            //Reditect the user to the heffer controller index action
            return RedirectToAction("ManageUsers", "Admin");
        }


        

    }
}
