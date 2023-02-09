using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeniorProject.Models.DataLayer.TableModels
{
    public class Member
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MemberID { get; set; }
        
        public string MemberRole { get; set; }

        [Required(ErrorMessage = "Choose member active status.")]
        [DataType(DataType.Text)]
        public bool ActiveStatus { get; set; }

        [Required(ErrorMessage = "Please enter a first name.")]
        [RegularExpression("^[a-zA-Z0-9]+$",
                ErrorMessage = "First name may not contain special characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter a last name.")]
        [RegularExpression("^[a-zA-Z0-9]+$",
                ErrorMessage = "Last name may not contain special characters.")]
        public string LastName { get; set; }

        public string City { get; set; }
        public string State { get; set; } 

        [Required(ErrorMessage = "Please enter a user.")]
        public IdentityUser IdentityUser { get; set; }
        public string IdentityUserID { get; set; }

    


    }
}
