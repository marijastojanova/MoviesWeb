using System.ComponentModel.DataAnnotations;

namespace MoviesWeb.Models
{
    public class User
    {
      
        //public string? Username { get; set; }
        //public string? Phone { get; set; }
        public string? Name { get; set; }
        public int? Id { get; set; }
        //public string? Salt { get; set; }
        public string? LastName { get; set; }
        public bool Administrator { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(40, MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "Passsword does not match.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

    }
}
