using EvdeSaglik.Service.DTOs.Common;
using EvdeSaglik.Service.DTOs.Profile;
using EvdeSaglik.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EvdeSaglik.Controllers.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetMyProfile()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var profile = await _profileService.GetUserProfileAsync(userId);
            return Ok(ApiResponse<UserProfileDto>.SuccessResponse(profile));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<UserProfileDto>.FailureResponse(ex.Message));
        }
    }

    [HttpPut]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var profile = await _profileService.UpdateProfileAsync(userId, dto);
            return Ok(ApiResponse<UserProfileDto>.SuccessResponse(profile, "Profile updated successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<UserProfileDto>.FailureResponse(ex.Message));
        }
    }
}
