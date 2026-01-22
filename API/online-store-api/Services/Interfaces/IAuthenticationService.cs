using online_store_api.Models.DTOs;
using online_store_api.Models.Responses;

namespace online_store_api.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ApiResponse<TokenResponse>> Register(RegisterRequest request);
        Task<ApiResponse<TokenResponse>> Login(LoginRequest request);
    }
}
