using EvdeSaglik.Service.DTOs.Common;
using EvdeSaglik.Service.DTOs.Message;
using EvdeSaglik.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EvdeSaglik.Controllers.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<MessageDto>>>> GetMyMessages()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var messages = await _messageService.GetUserMessagesAsync(userId);
            return Ok(ApiResponse<List<MessageDto>>.SuccessResponse(messages));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<MessageDto>>.FailureResponse(ex.Message));
        }
    }

    [HttpGet("conversation/{otherUserId}")]
    public async Task<ActionResult<ApiResponse<List<MessageDto>>>> GetConversation(Guid otherUserId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var messages = await _messageService.GetConversationAsync(userId, otherUserId);
            return Ok(ApiResponse<List<MessageDto>>.SuccessResponse(messages));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<MessageDto>>.FailureResponse(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<MessageDto>>> SendMessage([FromBody] SendMessageDto dto)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var message = await _messageService.SendMessageAsync(userId, dto);
            return Ok(ApiResponse<MessageDto>.SuccessResponse(message, "Message sent successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<MessageDto>.FailureResponse(ex.Message));
        }
    }

    [HttpPut("{id}/read")]
    public async Task<ActionResult<ApiResponse<object>>> MarkAsRead(Guid id)
    {
        try
        {
            await _messageService.MarkAsReadAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Message marked as read"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message));
        }
    }
}
