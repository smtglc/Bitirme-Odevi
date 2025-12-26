using EvdeSaglik.Service.DTOs.Appointment;
using EvdeSaglik.Service.DTOs.Common;
using EvdeSaglik.Service.DTOs.Doctor;
using EvdeSaglik.Service.DTOs.Service;
using EvdeSaglik.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EvdeSaglik.Controllers.Controllers;

[Authorize(Roles = "Doctor")]
[ApiController]
[Route("api/[controller]")]
public class DoctorController : ControllerBase
{
    private readonly IDoctorService _doctorService;

    public DoctorController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    #region Service Management

    [HttpGet("services")]
    public async Task<ActionResult<ApiResponse<List<ServiceListDto>>>> GetMyServices()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var services = await _doctorService.GetMyServicesAsync(userId);
            return Ok(ApiResponse<List<ServiceListDto>>.SuccessResponse(services));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<ServiceListDto>>.FailureResponse(ex.Message));
        }
    }

    [HttpPost("services")]
    public async Task<ActionResult<ApiResponse<ServiceDetailDto>>> CreateService([FromBody] CreateDoctorServiceDto dto)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var service = await _doctorService.CreateServiceAsync(userId, dto);
            return Ok(ApiResponse<ServiceDetailDto>.SuccessResponse(service, "Service created successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ServiceDetailDto>.FailureResponse(ex.Message));
        }
    }

    [HttpPut("services/{id}")]
    public async Task<ActionResult<ApiResponse<ServiceDetailDto>>> UpdateService(Guid id, [FromBody] UpdateDoctorServiceDto dto)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var service = await _doctorService.UpdateServiceAsync(userId, id, dto);
            return Ok(ApiResponse<ServiceDetailDto>.SuccessResponse(service, "Service updated successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ServiceDetailDto>.FailureResponse(ex.Message));
        }
    }

    [HttpDelete("services/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteService(Guid id)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _doctorService.DeleteServiceAsync(userId, id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Service deleted successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message));
        }
    }

    #endregion

    #region Appointment Management

    [HttpGet("appointments")]
    public async Task<ActionResult<ApiResponse<List<DoctorAppointmentDto>>>> GetMyAppointments()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appointments = await _doctorService.GetMyAppointmentsAsync(userId);
            return Ok(ApiResponse<List<DoctorAppointmentDto>>.SuccessResponse(appointments));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<DoctorAppointmentDto>>.FailureResponse(ex.Message));
        }
    }

    [HttpGet("appointments/upcoming")]
    public async Task<ActionResult<ApiResponse<List<DoctorAppointmentDto>>>> GetUpcomingAppointments()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appointments = await _doctorService.GetUpcomingAppointmentsAsync(userId);
            return Ok(ApiResponse<List<DoctorAppointmentDto>>.SuccessResponse(appointments));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<DoctorAppointmentDto>>.FailureResponse(ex.Message));
        }
    }

    [HttpPut("appointments/{id}/confirm")]
    public async Task<ActionResult<ApiResponse<DoctorAppointmentDto>>> ConfirmAppointment(Guid id)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appointment = await _doctorService.ConfirmAppointmentAsync(userId, id);
            return Ok(ApiResponse<DoctorAppointmentDto>.SuccessResponse(appointment, "Appointment confirmed successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<DoctorAppointmentDto>.FailureResponse(ex.Message));
        }
    }

    [HttpPut("appointments/{id}/cancel")]
    public async Task<ActionResult<ApiResponse<DoctorAppointmentDto>>> CancelAppointment(Guid id)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appointment = await _doctorService.CancelAppointmentAsync(userId, id);
            return Ok(ApiResponse<DoctorAppointmentDto>.SuccessResponse(appointment, "Appointment cancelled successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<DoctorAppointmentDto>.FailureResponse(ex.Message));
        }
    }

    [HttpPut("appointments/{id}/complete")]
    public async Task<ActionResult<ApiResponse<DoctorAppointmentDto>>> CompleteAppointment(Guid id)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appointment = await _doctorService.CompleteAppointmentAsync(userId, id);
            return Ok(ApiResponse<DoctorAppointmentDto>.SuccessResponse(appointment, "Appointment completed successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<DoctorAppointmentDto>.FailureResponse(ex.Message));
        }
    }

    #endregion

    #region Working Hours

    [HttpGet("working-hours")]
    public async Task<ActionResult<ApiResponse<List<WorkingHoursDto>>>> GetMyWorkingHours()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var workingHours = await _doctorService.GetMyWorkingHoursAsync(userId);
            return Ok(ApiResponse<List<WorkingHoursDto>>.SuccessResponse(workingHours));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<WorkingHoursDto>>.FailureResponse(ex.Message));
        }
    }

    [HttpPost("working-hours")]
    public async Task<ActionResult<ApiResponse<WorkingHoursDto>>> AddWorkingHours([FromBody] CreateWorkingHoursDto dto)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var workingHours = await _doctorService.AddWorkingHoursAsync(userId, dto);
            return Ok(ApiResponse<WorkingHoursDto>.SuccessResponse(workingHours, "Working hours added successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<WorkingHoursDto>.FailureResponse(ex.Message));
        }
    }

    [HttpPut("working-hours/{id}")]
    public async Task<ActionResult<ApiResponse<WorkingHoursDto>>> UpdateWorkingHours(Guid id, [FromBody] CreateWorkingHoursDto dto)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var workingHours = await _doctorService.UpdateWorkingHoursAsync(userId, id, dto);
            return Ok(ApiResponse<WorkingHoursDto>.SuccessResponse(workingHours, "Working hours updated successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<WorkingHoursDto>.FailureResponse(ex.Message));
        }
    }

    [HttpDelete("working-hours/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteWorkingHours(Guid id)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _doctorService.DeleteWorkingHoursAsync(userId, id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Working hours deleted successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message));
        }
    }

    #endregion

    #region Document Management

    [HttpGet("documents")]
    public async Task<ActionResult<ApiResponse<List<DoctorDocumentDto>>>> GetMyDocuments()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var documents = await _doctorService.GetMyDocumentsAsync(userId);
            return Ok(ApiResponse<List<DoctorDocumentDto>>.SuccessResponse(documents));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<List<DoctorDocumentDto>>.FailureResponse(ex.Message));
        }
    }

    [HttpGet("documents/{id}")]
    public async Task<ActionResult<ApiResponse<DoctorDocumentDto>>> GetDocumentById(Guid id)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var document = await _doctorService.GetDocumentByIdAsync(userId, id);
            return Ok(ApiResponse<DoctorDocumentDto>.SuccessResponse(document));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<DoctorDocumentDto>.FailureResponse(ex.Message));
        }
    }

    [HttpPost("documents")]
    public async Task<ActionResult<ApiResponse<DoctorDocumentDto>>> CreateDocument([FromForm] CreateDoctorDocumentDto dto, IFormFile? file)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var document = await _doctorService.CreateDocumentAsync(userId, dto, file);
            return Ok(ApiResponse<DoctorDocumentDto>.SuccessResponse(document, "Document created successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<DoctorDocumentDto>.FailureResponse(ex.Message));
        }
    }

    [HttpPut("documents/{id}")]
    public async Task<ActionResult<ApiResponse<DoctorDocumentDto>>> UpdateDocument(Guid id, [FromForm] UpdateDoctorDocumentDto dto, IFormFile? file)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var document = await _doctorService.UpdateDocumentAsync(userId, id, dto, file);
            return Ok(ApiResponse<DoctorDocumentDto>.SuccessResponse(document, "Document updated successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<DoctorDocumentDto>.FailureResponse(ex.Message));
        }
    }

    [HttpDelete("documents/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteDocument(Guid id)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _doctorService.DeleteDocumentAsync(userId, id);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Document deleted successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message));
        }
    }

    #endregion

    #region Profile Management

    [HttpGet("profile")]
    public async Task<ActionResult<ApiResponse<DoctorDto>>> GetProfile()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var profile = await _doctorService.GetMyProfileAsync(userId);
            return Ok(ApiResponse<DoctorDto>.SuccessResponse(profile));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<DoctorDto>.FailureResponse(ex.Message));
        }
    }

    [HttpPut("profile")]
    public async Task<ActionResult<ApiResponse<DoctorDto>>> UpdateProfile([FromBody] UpdateDoctorProfileDto dto)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var profile = await _doctorService.UpdateMyProfileAsync(userId, dto);
            return Ok(ApiResponse<DoctorDto>.SuccessResponse(profile, "Profile updated successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<DoctorDto>.FailureResponse(ex.Message));
        }
    }

    #endregion
}
