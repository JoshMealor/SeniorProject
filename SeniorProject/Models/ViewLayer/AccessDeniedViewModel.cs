using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SeniorProject.Models.ViewLayer
{
    public class AccessDeniedViewModel
    {
        
        public string userName { get; set; }
        public string roleName { get; set; }

    }
}
