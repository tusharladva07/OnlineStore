using System.ComponentModel.DataAnnotations;

namespace online_store_api.Models.DTOs
{
    public class LoginRequest
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
