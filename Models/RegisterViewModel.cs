using System.ComponentModel.DataAnnotations;

namespace Egov.Models
{
    public class RegisterViewModel
    {
        public string Name { get; set; }
        [DataType(DataType.EmailAddress)]

        [Display(Name = "Email")]//Helps to display different label name. Only for frontend in UI
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password length should be greater than 7")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]

        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
    }
}
