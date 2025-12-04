using EvdeSaglik.Service.DTOs.Common;
using EvdeSaglik.Service.DTOs.Service;
using EvdeSaglik.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvdeSaglik.Controllers.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IServiceManagementService _serviceManagement;

    public ServicesController(IServiceManagementService serviceManagement)
    {
        _serviceManagement = serviceManagement;
    }

    [HttpGet]
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

    [HttpGet("filter")]
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

    [HttpGet("{id}")]
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
}
