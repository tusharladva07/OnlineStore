using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using online_store_api.Models.DTOs;
using online_store_api.Models.Responses;
using online_store_api.Services.Interfaces;

namespace online_store_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public UserController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<TokenResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<TokenResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<TokenResponse>))]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                var errorMessage = string.Join(", ", errors);
                return BadRequest(ApiResponse<TokenResponse>.Failure($"Validation error: {errorMessage}"));
            }

            var response = await _authenticationService.Login(request);

            // Return 200 for success and warning, 400 for failure
            if (response.ResponseStatus == "success" || response.ResponseStatus == "warning")
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<TokenResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<TokenResponse>))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ApiResponse<TokenResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse<TokenResponse>))]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                var errorMessage = string.Join(", ", errors);
                return BadRequest(ApiResponse<TokenResponse>.Failure($"Validation error: {errorMessage}"));
            }

            var response = await _authenticationService.Register(request);

            // Return 200 for success and warning
            if (response.ResponseStatus == "success" || response.ResponseStatus == "warning")
            {
                return Ok(response);
            }
            // Return 409 for duplicate user
            else if (response.ResponseMessage.Contains("already exists") || response.ResponseMessage.Contains("already taken"))
            {
                return Conflict(response);
            }
            // Return 400 for other failures
            else
            {
                return BadRequest(response);
            }
        }
    }
}
