using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeniorProject.Models.DataLayer.TableModels
{
    public class Due
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DueID { get; set; }

        [Required(ErrorMessage = "Please input the amount due.")]
        [DataType(DataType.Currency)]
        public double AmountDue { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateTimeDue { get; set; }

        [Required(ErrorMessage = "Please input the amount paid.")]
        [DataType(DataType.Currency)]
        public double AmountPaid { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateTimePaid { get; set; }
        public string PaymentMethod { get; set; }

        [Required(ErrorMessage = "Please enter a member.")]
        public Member Member { get; set; }
        public int MemberID { get; set; }
       


    }
}
