using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;

namespace SeniorProject.Areas.Authenticated.Models.Admin
{
    public class ManageUsersAndRolesViewModel
    {
        public string IdentityUserID { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool Active { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }

        public string RoleName { get; set; }

        public List<SelectListItem> roleChoices { get; set; }

    }
}
