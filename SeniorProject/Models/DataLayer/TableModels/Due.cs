using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeniorProject.Models.DataLayer.TableModels
{
    public class Due
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DueID { get; set; }
       
        public double AmountDue { get; set; }
        public DateTime DateTimeDue { get; set; }
        public double AmountPaid { get; set; }
        public DateTime DateTimePaid { get; set; }
        public string PaymentMethod { get; set; }

        [Required(ErrorMessage = "Please enter a member.")]
        public Models.DataLayer.TableModels.Member Member { get; set; }
       


    }
}
