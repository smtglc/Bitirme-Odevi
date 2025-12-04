using EvdeSaglik.Service.DTOs.Appointment;
using EvdeSaglik.Service.DTOs.Common;
using EvdeSaglik.Service.DTOs.Profile;
using EvdeSaglik.Service.DTOs.Service;
using EvdeSaglik.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EvdeSaglik.Controllers.Controllers;

[Authorize(Roles = "Patient")]
[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    private readonly IProfileService _profileService;
    private readonly IServiceManagementService _serviceManagement;
    private readonly IAppointmentService _appointmentService;

    public PatientController(
        IProfileService profileService,
        IServiceManagementService serviceManagement,
        IAppointmentService appointmentService)
    {
        _profileService = profileService;
        _serviceManagement = serviceManagement;
        _appointmentService = appointmentService;
    }

    #region Profile Management

    [HttpGet("profile")]
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

    [HttpPut("profile")]
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

    #endregion

    #region Service Discovery

    [HttpGet("services")]
    public async Task<ActionResult<ApiResponse<List<ServiceListDto>>>> GetAllServices()
    {
        try
        {
            var services = await _serviceManagement.GetAllServicesAsync();
            return Ok(ApiResponse<List<ServiceListDto>>.SuccessResponse(services));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<ServiceListDto>>.FailureResponse(ex.Message));
        }
    }

    [HttpGet("services/filter")]
    public async Task<ActionResult<ApiResponse<List<ServiceListDto>>>> GetFilteredServices(
        [FromQuery] string? specialization,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] Guid? doctorId)
    {
        try
        {
            var filter = new ServiceFilterDto
            {
                Specialization = specialization,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                DoctorId = doctorId
            };

            var services = await _serviceManagement.GetFilteredServicesAsync(filter);
            return Ok(ApiResponse<List<ServiceListDto>>.SuccessResponse(services));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<ServiceListDto>>.FailureResponse(ex.Message));
        }
    }

    [HttpGet("services/{id}")]
    public async Task<ActionResult<ApiResponse<ServiceDetailDto>>> GetServiceDetail(Guid id)
    {
        try
        {
            var service = await _serviceManagement.GetServiceDetailAsync(id);
            return Ok(ApiResponse<ServiceDetailDto>.SuccessResponse(service));
        }
        catch (Exception ex)
        {
            return NotFound(ApiResponse<ServiceDetailDto>.FailureResponse(ex.Message));
        }
    }

    #endregion

    #region Appointment Management

    [HttpGet("appointments")]
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

    [HttpPost("appointments")]
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

    [HttpGet("appointments/{id}")]
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

    [HttpPut("appointments/{id}/cancel")]
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

    #endregion
}
