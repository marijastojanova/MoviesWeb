using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(40, MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Compare("ConfirmNewPassword", ErrorMessage = "Passsword does not match.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }
    }
}
