using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MoviesWeb.Models
{
    public class LoginViewModel
    {

        [Required(ErrorMessage ="Email required.")]
        [EmailAddress]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]   
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password required.")]
        [JsonPropertyName("password")]
        public string Password { get; set; }

        [Display(Name ="Remember me?")]
        public bool RememberMe { get; set; }

    }
}
