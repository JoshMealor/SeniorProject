using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Xml.Linq;

namespace SeniorProject.Areas.Authenticated.Models.Admin
{
    public class AddUserWithRoleViewModel
    {
        //Add the required attribute validation and error messages. Also ensuer string length is less than 255
        [Required(ErrorMessage = "Please enter a username.")]
        [StringLength(255)]
        public string Username { get; set; }

        //Add the required attribute validation and error messages. Also compare to the other variable
        [Required(ErrorMessage = "Please enter a password.")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword")]
        public string Password { get; set; }

        //Add the required attribute validation and error messages. Aslo set the attribute name to access with attributes above
        [Required(ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Please choose a role.")]
        public string Role { get; set; }

        public List<SelectListItem>? roleChoices { get; set; }
    }
}
