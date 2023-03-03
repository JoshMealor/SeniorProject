using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Xml.Linq;

namespace SeniorProject.Areas.Authenticated.Models.Admin
{
    public class EditUserWithRoleViewModel
    {
        [Required]
        public string? Username { get; set; }


        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }

        public string? role { get; set; }
        public List<SelectListItem>? roleChoices { get; set; }

        public string? IdentityUserID { get; set; }



    }
}
