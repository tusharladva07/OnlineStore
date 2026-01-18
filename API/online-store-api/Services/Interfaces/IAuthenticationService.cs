using online_store_api.Models.DTOs;

namespace online_store_api.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<string> Register(RegisterRequest request);
        Task<string> Login(LoginRequest request);
    }
}
