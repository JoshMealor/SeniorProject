using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeniorProject.Areas.Authenticated.Models.Manager
{
    public class DuesViewModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DueID { get; set; }

        [Required(ErrorMessage = "Please input the amount due.")]
        [DataType(DataType.Currency)]
        public double AmountDue { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? DateTimeDue { get; set; }

        [Required(ErrorMessage = "Please input the amount paid.")]
        [DataType(DataType.Currency)]
        public double AmountPaid { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? DateTimePaid { get; set; }

        public string? PaymentMethod { get; set; }


        public int MemberID { get; set; }

        public bool? MemberActiveStatus { get; set; }
        public string? MemberFirstName { get; set; }
        public string? MemberLastName { get; set; }


    }
}
