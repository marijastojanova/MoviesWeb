using System.ComponentModel.DataAnnotations;

namespace MovieAPI.Models
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int Id { get; set; }
        public bool isConfirmed { get; set; }

    }
}
