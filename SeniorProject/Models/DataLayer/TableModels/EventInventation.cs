using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeniorProject.Models.DataLayer.TableModels
{
    public class EventInventation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventInventationID { get; set; }
        
       
       

        public string InvitationBody { get; set; }
        public string InvitationResponseBody { get; set; }

        [Required(ErrorMessage = "Please enter an event.")]
        public Models.DataLayer.TableModels.Event Event { get; set; }

        [Required(ErrorMessage = "Please enter a user.")]
        public Models.DataLayer.User User { get; set; }
    }
}
