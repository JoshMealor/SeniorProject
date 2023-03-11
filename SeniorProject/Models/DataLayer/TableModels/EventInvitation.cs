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

        //Member who sent the invitation

        [Required(ErrorMessage = "Please enter a member.")]
        public Member Member_Sender { get; set; }
        public int MemberID_Sender { get; set; }

        //Member who recieved the invitation
        [Required(ErrorMessage = "Please enter a member.")]
        public Member Member_Reciever { get; set; }
        public int MemberID_Reciever { get; set; }

        
    }
}
