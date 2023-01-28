using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeniorProject.Areas.Authenticated.Models
{
    public class DuesViewModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DueID { get; set; }
       
        public double AmountDue { get; set; }
        public DateTime? DateTimeDue { get; set; }
        public double AmountPaid { get; set; }
        public DateTime? DateTimePaid { get; set; }
        public string? PaymentMethod { get; set; }

       
        public int MemberID { get; set; }
       
        public bool? MemberActiveStatus { get; set; }
        public string? MemberFirstName { get; set; }
        public string? MemberLastName { get; set; }


    }
}
