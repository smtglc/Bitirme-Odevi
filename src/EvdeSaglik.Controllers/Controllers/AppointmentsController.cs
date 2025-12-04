using EvdeSaglik.Service.DTOs.Appointment;
using EvdeSaglik.Service.DTOs.Common;
using EvdeSaglik.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EvdeSaglik.Controllers.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet("my-appointments")]
    public async Task<ActionResult<ApiResponse<List<AppointmentDto>>>> GetMyAppointments()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appointments = await _appointmentService.GetMyAppointmentsAsync(userId);
            return Ok(ApiResponse<List<AppointmentDto>>.SuccessResponse(appointments));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<AppointmentDto>>.FailureResponse(ex.Message));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> CreateAppointment([FromBody] CreateAppointmentDto dto)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appointment = await _appointmentService.CreateAppointmentAsync(userId, dto);
            return Ok(ApiResponse<AppointmentDto>.SuccessResponse(appointment, "Appointment created successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<AppointmentDto>.FailureResponse(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> GetAppointmentDetail(Guid id)
    {
        try
        {
            var appointment = await _appointmentService.GetAppointmentDetailAsync(id);
            return Ok(ApiResponse<AppointmentDto>.SuccessResponse(appointment));
        }
        catch (Exception ex)
        {
            return NotFound(ApiResponse<AppointmentDto>.FailureResponse(ex.Message));
        }
    }

    [HttpPut("{id}/cancel")]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> CancelAppointment(Guid id)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appointment = await _appointmentService.CancelAppointmentAsync(id, userId);
            return Ok(ApiResponse<AppointmentDto>.SuccessResponse(appointment, "Appointment cancelled successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<AppointmentDto>.FailureResponse(ex.Message));
        }
    }
}
