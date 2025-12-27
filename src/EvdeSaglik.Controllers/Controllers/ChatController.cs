using EvdeSaglik.Service.DTOs.Chat;
using EvdeSaglik.Service.DTOs.Common;
using EvdeSaglik.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EvdeSaglik.Controllers.Controllers;

[Authorize(Roles = "Doctor,Patient")]
[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ChatResponseDto>>> Chat([FromBody] ChatRequestDto request)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _chatService.GetChatResponseAsync(userId, request);
            
            return Ok(ApiResponse<ChatResponseDto>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ChatResponseDto>.FailureResponse(ex.Message));
        }
    }
}
