using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeniorProject.Models.DataLayer.TableModels
{
    public class EventInvitation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventInvitationID { get; set; }
        
        public string InvitationBody { get; set; }
        public string InvitationResponseBody { get; set; }

        [Required(ErrorMessage = "Please enter an event.")]
        public Event Event { get; set; }
        public int EventID { get; set; }

        [Required(ErrorMessage = "Please enter a user.")]
        public IdentityUser IdentityUser { get; set; }
        public string IdentityUserID { get; set; }
    }
}
