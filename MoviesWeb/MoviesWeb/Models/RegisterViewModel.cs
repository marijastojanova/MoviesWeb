using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MoviesWeb.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(40,MinimumLength =8)]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword",ErrorMessage ="Passsword does not match.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
