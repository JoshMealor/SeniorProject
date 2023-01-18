using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeniorProject.Models.DataLayer.TableModels
{
    public class Member
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MemberID { get; set; }
        
        public string MemberRole { get; set; }

        public bool ActiveStatus { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        


        [Required(ErrorMessage = "Please enter a user.")]
        public Models.DataLayer.User User { get; set; }

    


    }
}
