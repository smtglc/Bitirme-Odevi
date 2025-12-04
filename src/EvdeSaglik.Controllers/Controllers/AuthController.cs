using EvdeSaglik.Service.DTOs.Auth;
using EvdeSaglik.Service.DTOs.Common;
using EvdeSaglik.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EvdeSaglik.Controllers.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response, "Registration successful"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<AuthResponseDto>.FailureResponse(ex.Message));
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response, "Login successful"));
        }
        catch (Exception ex)
        {
            return Unauthorized(ApiResponse<AuthResponseDto>.FailureResponse(ex.Message));
        }
    }
}
