using System.ComponentModel.DataAnnotations;

namespace online_store_api.Models.DTOs
{
    public class RegisterRequest
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
