using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SeniorProject.Models.ViewLayer
{
    public class LoginViewModel
    {
        //Add the required attribute validation and error messages. Also ensuer string length is less than 255
        [Required(ErrorMessage = "Please enter a username.")]
        [StringLength(255)]
        public string Username { get; set; }

        //Add the required attribute validation and error messages. Also ensuer string length is less than 255
        [Required(ErrorMessage = "Please enter a password.")]
        [StringLength(255)]
        public string Password { get; set; }

        //public string ReturnUrl { get; set; }

        //public bool RememberMe { get; set; }

    }
}
