using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using online_store_api.Models.DTOs;
using online_store_api.Models.Entities;
using online_store_api.Models.Responses;
using online_store_api.Services.Interfaces;

namespace OnlineStore.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public AuthenticationService(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<ApiResponse<TokenResponse>> Register(RegisterRequest request)
        {
            try
            {
                // Check if user already exists by email
                var userByEmail = await _userManager.FindByEmailAsync(request.Email);
                if (userByEmail is not null)
                {
                    return ApiResponse<TokenResponse>.Failure($"User with email {request.Email} already exists.");
                }

                // Check if user already exists by username
                var userByUsername = await _userManager.FindByNameAsync(request.UserName);
                if (userByUsername is not null)
                {
                    return ApiResponse<TokenResponse>.Failure($"Username {request.UserName} is already taken.");
                }

                // Create new user
                User user = new()
                {
                    Email = request.Email,
                    UserName = request.UserName,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    var errorMessage = GetErrorsText(result.Errors);
                    return ApiResponse<TokenResponse>.Failure($"Unable to register user. {errorMessage}");
                }

                // Generate and return token
                var loginResponse = await Login(new LoginRequest { Email = request.Email, Password = request.Password });
                return loginResponse;
            }
            catch (Exception ex)
            {
                return ApiResponse<TokenResponse>.Failure($"Registration failed: {ex.Message}");
            }
        }

        public async Task<ApiResponse<TokenResponse>> Login(LoginRequest request)
        {
            try
            {
                // Find user by username or email
                var user = await _userManager.FindByNameAsync(request.Email);

                if (user is null)
                {
                    user = await _userManager.FindByEmailAsync(request.Email);
                }

                // Validate user exists and password is correct
                if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
                {
                    return ApiResponse<TokenResponse>.Failure("Invalid email or password.");
                }

                // Generate JWT token
                var authClaims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.UserName),
                    new(ClaimTypes.Email, user.Email),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                var token = GetToken(authClaims);
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                // Return wrapped response with token and user info
                var tokenResponse = new TokenResponse
                {
                    Token = tokenString,
                    Username = user.UserName,
                    Email = user.Email
                };

                return ApiResponse<TokenResponse>.Success("Login successful. Welcome back!", tokenResponse);
            }
            catch (Exception ex)
            {
                return ApiResponse<TokenResponse>.Failure($"Login failed: {ex.Message}");
            }
        }

        private JwtSecurityToken GetToken(IEnumerable<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

            return token;
        }

        private string GetErrorsText(IEnumerable<IdentityError> errors)
        {
            return string.Join(", ", errors.Select(error => error.Description).ToArray());
        }
    }
}
