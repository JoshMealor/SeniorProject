using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SeniorProject.Areas.Authenticated.Models.Admin
{
    public class ManageUsersAndRolesViewModel
    {
        public string IdentityUserID { get; set; }
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter a first name.")]
        [RegularExpression("^[a-zA-Z0-9]+$",
                ErrorMessage = "First name may not contain special characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter a last name.")]
        [RegularExpression("^[a-zA-Z0-9]+$",
                ErrorMessage = "Last name may not contain special characters.")]
        public string LastName { get; set; }

        public bool Active { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        public string RoleName { get; set; }

        public List<SelectListItem> roleChoices { get; set; }

    }
}
