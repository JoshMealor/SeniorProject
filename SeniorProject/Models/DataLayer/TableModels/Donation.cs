using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeniorProject.Models.DataLayer.TableModels
{
    public class Donation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DonationID { get; set; }
       
        public double DonationAmmount { get; set; }
        public string PaymentMethod { get; set; }

        public DateTime DonationDate { get; set; }

        [Required(ErrorMessage = "Please enter a donor.")] 
        public Donor Donor { get; set; }
        public int DonorId { get; set; }
    }
}
