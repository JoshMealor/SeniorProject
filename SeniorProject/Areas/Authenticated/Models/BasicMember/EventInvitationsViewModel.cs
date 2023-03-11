using SeniorProject.Models.DataLayer.TableModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeniorProject.Areas.Authenticated.Models.BasicMember
{
    public class EventInvitationsViewModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventInvitationID { get; set; }

        public string? InvitationBody { get; set; }
        public string? InvitationResponseBody { get; set; }

        public string? EventName { get; set; }
        public string? EventLocation { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int EventID { get; set; }

        //Member who sent the invitation

        public string? MemberFirstName_Sender { get; set; }
        public string? MemberLastName_Sender { get; set; }
        public int MemberID_Sender { get; set; }

        //Member who recieved the invitation
     
        public string? MemberFirstName_Reciever { get; set; }
        public string? MemberLastName_Reciever { get; set; }
        public int MemberID_Reciever { get; set; }


       
    }
}
