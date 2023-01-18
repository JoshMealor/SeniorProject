using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeniorProject.Models.DataLayer.TableModels
{
    public class Donnation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DonnationID { get; set; }
       
        public double DonationAmmount { get; set; }
        public string PaymentMethod { get; set; }

        public DateTime DonationDate { get; set; }

        [Required(ErrorMessage = "Please enter a donor.")]       
        public Models.DataLayer.TableModels.Donor Donor { get; set; }
    }
}
