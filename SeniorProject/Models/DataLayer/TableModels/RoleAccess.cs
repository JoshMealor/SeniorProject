using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeniorProject.Models.DataLayer.TableModels
{
    public class RoleAccess
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleAccessID { get; set; }

        public string TableName { get; set; }

        public bool Read { get; set; }
        public bool Write { get; set; }
        public bool Delete { get; set; }
      
      

        [Required(ErrorMessage = "Please enter a user.")]
        public IdentityRole IdentityRole { get; set; }

        
    }
}
