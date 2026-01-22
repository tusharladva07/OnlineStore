using System.ComponentModel.DataAnnotations;

namespace online_store_api.Models.DTOs
{
    public class LoginRequest
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
