using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SeniorProject.Models.ViewLayer
{
    public class RegisterViewModel
    {
        //Add the required attribute validation and error messages. Also ensuer string length is less than 255
        [Required(ErrorMessage = "Please enter a username.")]
        [StringLength(255)]
        public string Username { get; set; }

        //Add the required attribute validation and error messages. Also compare to the other variable
        [Required(ErrorMessage = "Please enter a password.")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword")]
        public string Password { get; set; }

        //Add the required attribute validation and error messages. Aslo set the attribute name to access with attributes above
        [Required(ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

    }
}
