using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeniorProject.Models.DataLayer.TableModels
{
    public class Donor
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DonorID { get; set; }
      
        public string SolicitedBy { get; set; }

        [Required(ErrorMessage = "Please enter a user.")]
        public IdentityUser User { get; set; }

        
    }
}
