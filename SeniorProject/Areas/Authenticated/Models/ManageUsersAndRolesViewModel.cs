using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;

namespace SeniorProject.Areas.Authenticated.Models
{
    public class ManageUsersAndRolesViewModel
    {
        public IdentityUser user { get; set; }

        public string role { get; set; }

        public List<SelectListItem> roleChoices { get; set; }
      
    }
}
