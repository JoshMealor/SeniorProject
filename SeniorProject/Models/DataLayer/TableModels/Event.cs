using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeniorProject.Models.DataLayer.TableModels
{
    public class Event
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventID { get; set; }

        [Required(ErrorMessage = "Please enter the event name.")]
        [StringLength(255)]
        public string EventName { get; set; }

        [Required(ErrorMessage = "Please enter an event description.")]
        [StringLength(255)]
        public string EventDescription { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime StartDateTime { get; set; }
        
        [DataType(DataType.DateTime)]
        public DateTime EndDateTime { get; set; }
        public string EventLocation { get; set; }

    }
}
