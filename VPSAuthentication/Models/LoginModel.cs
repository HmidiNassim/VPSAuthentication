namespace VPSAuthentication.Models
{
    using System.ComponentModel.DataAnnotations;

    //LoginModel uses to validate the model passed in post to the AuthenticateController
    public class LoginModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}