using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeniorProject.Areas.Authenticated.Models.BasicMember
{
    public class DonationViewModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DonationID { get; set; }

        [Required(ErrorMessage = "Please input the amount donated.")]
        [DataType(DataType.Currency)]
        public double DonationAmmount { get; set; }

        //PCI public string PaymentMethod { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DonationDate { get; set; }

        public string? DonorNameOrTitle { get; set; }

        //Collected By Member

        public string? MemberFirstName { get; set; }
        public string? MemberLastName { get; set; }
        public int MemberID { get; set; }
    }
}
