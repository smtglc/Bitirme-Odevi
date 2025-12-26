using EvdeSaglik.Entity.Entities;
using EvdeSaglik.Entity.Enums;
using EvdeSaglik.Repositories.Interfaces;
using EvdeSaglik.Service.DTOs.Appointment;
using EvdeSaglik.Service.DTOs.Doctor;
using EvdeSaglik.Service.DTOs.Service;
using EvdeSaglik.Service.Interfaces;

namespace EvdeSaglik.Service.Services;

public class DoctorService : IDoctorService
{
    private readonly IUnitOfWork _unitOfWork;

    public DoctorService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    #region Service Management

    public async Task<List<ServiceListDto>> GetMyServicesAsync(Guid userId)
    {
        var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
        if (doctor == null)
            throw new Exception("Doctor not found");

        var services = await _unitOfWork.Services.GetServicesByDoctorIdAsync(doctor.Id);

        return services.Select(s => new ServiceListDto
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            BasePrice = s.BasePrice,
            DurationMinutes = s.DurationMinutes,
            Specialization = s.Specialization.ToString(),
            DoctorName = s.Doctor.User.FirstName + " " + s.Doctor.User.LastName,
            DoctorSpecialization = s.Doctor.Specialization
        }).ToList();
    }

    public async Task<ServiceDetailDto> CreateServiceAsync(Guid userId, CreateDoctorServiceDto dto)
    {
        var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
        if (doctor == null)
            throw new Exception("Doctor not found");

        if (!Enum.TryParse<MedicalSpecialization>(dto.Specialization, true, out var specialization))
            throw new Exception("Invalid specialization");

        var service = new Entity.Entities.Service
        {
            Name = dto.Name,
            Description = dto.Description,
            BasePrice = dto.BasePrice,
            DurationMinutes = dto.DurationMinutes,
            Specialization = specialization,
            DoctorId = doctor.Id,
            IsActive = true
        };

        await _unitOfWork.Services.AddAsync(service);
        await _unitOfWork.SaveChangesAsync();

        // Reload with doctor info
        var createdService = await _unitOfWork.Services.GetServiceWithDoctorAsync(service.Id);

        return new ServiceDetailDto
        {
            Id = createdService!.Id,
            Name = createdService.Name,
            Description = createdService.Description,
            BasePrice = createdService.BasePrice,
            DurationMinutes = createdService.DurationMinutes,
            Specialization = createdService.Specialization.ToString(),
            DoctorId = createdService.DoctorId,
            DoctorName = createdService.Doctor.User.FirstName + " " + createdService.Doctor.User.LastName,
            DoctorEmail = createdService.Doctor.User.Email!,
            DoctorPhone = createdService.Doctor.User.PhoneNumber!,
            DoctorSpecialization = createdService.Doctor.Specialization,
            DoctorBio = createdService.Doctor.Bio,
            DoctorExperience = createdService.Doctor.YearsOfExperience
        };
    }

    public async Task<ServiceDetailDto> UpdateServiceAsync(Guid userId, Guid serviceId, UpdateDoctorServiceDto dto)
    {
        var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
        if (doctor == null)
            throw new Exception("Doctor not found");

        var service = await _unitOfWork.Services.GetServiceWithDoctorAsync(serviceId);
        if (service == null)
            throw new Exception("Service not found");

        if (service.DoctorId != doctor.Id)
            throw new UnauthorizedAccessException("You can only update your own services");

        if (!Enum.TryParse<MedicalSpecialization>(dto.Specialization, true, out var specialization))
            throw new Exception("Invalid specialization");

        service.Name = dto.Name;
        service.Description = dto.Description;
        service.BasePrice = dto.BasePrice;
        service.DurationMinutes = dto.DurationMinutes;
        service.Specialization = specialization;
        service.IsActive = dto.IsActive;

        _unitOfWork.Services.Update(service);
        await _unitOfWork.SaveChangesAsync();

        return new ServiceDetailDto
        {
            Id = service.Id,
            Name = service.Name,
            Description = service.Description,
            BasePrice = service.BasePrice,
            DurationMinutes = service.DurationMinutes,
            Specialization = service.Specialization.ToString(),
            DoctorId = service.DoctorId,
            DoctorName = service.Doctor.User.FirstName + " " + service.Doctor.User.LastName,
            DoctorEmail = service.Doctor.User.Email!,
            DoctorPhone = service.Doctor.User.PhoneNumber!,
            DoctorSpecialization = service.Doctor.Specialization,
            DoctorBio = service.Doctor.Bio,
            DoctorExperience = service.Doctor.YearsOfExperience
        };
    }

    public async Task DeleteServiceAsync(Guid userId, Guid serviceId)
    {
        var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
        if (doctor == null)
            throw new Exception("Doctor not found");

        var service = await _unitOfWork.Services.GetByIdAsync(serviceId);
        if (service == null)
            throw new Exception("Service not found");

        if (service.DoctorId != doctor.Id)
            throw new UnauthorizedAccessException("You can only delete your own services");

        // Check if there are any appointments associated with this service
        var hasAppointments = await _unitOfWork.Appointments.HasAppointmentsForServiceAsync(serviceId);
        if (hasAppointments)
        {
            throw new InvalidOperationException("Bu hizmet silinemez çünkü bu hizmete ait randevular bulunmaktadır. Lütfen önce randevuları iptal edin veya hizmeti pasif hale getirin.");
        }
        
        _unitOfWork.Services.Remove(service);
        await _unitOfWork.SaveChangesAsync();
    }

    #endregion

    #region Appointment Management

    public async Task<List<DoctorAppointmentDto>> GetMyAppointmentsAsync(Guid userId)
    {
        var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
        if (doctor == null)
            throw new Exception("Doctor not found");

        var appointments = await _unitOfWork.Appointments.GetByDoctorIdAsync(doctor.Id);

        return appointments.Select(a => new DoctorAppointmentDto
        {
            Id = a.Id,
            PatientId = a.PatientId,
            PatientName = a.Patient.User.FirstName + " " + a.Patient.User.LastName,
            PatientPhone = a.Patient.User.PhoneNumber ?? "",
            PatientAddress = a.Patient.Address,
            ServiceId = a.ServiceId,
            ServiceName = a.Service.Name,
            ScheduledDateTime = a.ScheduledDateTime,
            DurationMinutes = a.DurationMinutes,
            Status = a.Status.ToString(),
            TotalAmount = a.TotalAmount,
            PatientNotes = a.PatientNotes,
            DoctorNotes = a.DoctorNotes
        }).ToList();
    }

    public async Task<List<DoctorAppointmentDto>> GetUpcomingAppointmentsAsync(Guid userId)
    {
        var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
        if (doctor == null)
            throw new Exception("Doctor not found");

        var appointments = await _unitOfWork.Appointments.GetUpcomingAppointmentsByDoctorIdAsync(doctor.Id, DateTime.UtcNow);

        return appointments.Select(a => new DoctorAppointmentDto
        {
            Id = a.Id,
            PatientId = a.PatientId,
            PatientName = a.Patient.User.FirstName + " " + a.Patient.User.LastName,
            PatientPhone = a.Patient.User.PhoneNumber ?? "",
            PatientAddress = a.Patient.Address,
            ServiceId = a.ServiceId,
            ServiceName = a.Service.Name,
            ScheduledDateTime = a.ScheduledDateTime,
            DurationMinutes = a.DurationMinutes,
            Status = a.Status.ToString(),
            TotalAmount = a.TotalAmount,
            PatientNotes = a.PatientNotes,
            DoctorNotes = a.DoctorNotes
        }).ToList();
    }

    public async Task<DoctorAppointmentDto> ConfirmAppointmentAsync(Guid userId, Guid appointmentId)
    {
        var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
        if (doctor == null)
            throw new Exception("Doctor not found");

        var appointment = await _unitOfWork.Appointments.GetAppointmentWithDetailsAsync(appointmentId);
        if (appointment == null)
            throw new Exception("Appointment not found");

        if (appointment.DoctorId != doctor.Id)
            throw new UnauthorizedAccessException("You can only confirm your own appointments");

        if (appointment.Status != AppointmentStatus.Pending)
            throw new Exception("Only pending appointments can be confirmed");

        appointment.Status = AppointmentStatus.Confirmed;
        await _unitOfWork.SaveChangesAsync();

        return MapToDoctorAppointmentDto(appointment);
    }

    public async Task<DoctorAppointmentDto> CancelAppointmentAsync(Guid userId, Guid appointmentId)
    {
        var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
        if (doctor == null)
            throw new Exception("Doctor not found");

        var appointment = await _unitOfWork.Appointments.GetAppointmentWithDetailsAsync(appointmentId);
        if (appointment == null)
            throw new Exception("Appointment not found");

        if (appointment.DoctorId != doctor.Id)
            throw new UnauthorizedAccessException("You can only cancel your own appointments");

        if (appointment.Status == AppointmentStatus.Completed)
            throw new Exception("Cannot cancel completed appointments");

        if (appointment.Status == AppointmentStatus.Cancelled)
            throw new Exception("Appointment is already cancelled");

        appointment.Status = AppointmentStatus.Cancelled;
        await _unitOfWork.SaveChangesAsync();

        return MapToDoctorAppointmentDto(appointment);
    }

    public async Task<DoctorAppointmentDto> CompleteAppointmentAsync(Guid userId, Guid appointmentId)
    {
        var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
        if (doctor == null)
            throw new Exception("Doctor not found");

        var appointment = await _unitOfWork.Appointments.GetAppointmentWithDetailsAsync(appointmentId);
        if (appointment == null)
            throw new Exception("Appointment not found");

        if (appointment.DoctorId != doctor.Id)
            throw new UnauthorizedAccessException("You can only complete your own appointments");

        if (appointment.Status == AppointmentStatus.Completed)
            throw new Exception("Appointment is already completed");

        if (appointment.Status == AppointmentStatus.Cancelled)
            throw new Exception("Cannot complete cancelled appointments");

        appointment.Status = AppointmentStatus.Completed;
        await _unitOfWork.SaveChangesAsync();

        return MapToDoctorAppointmentDto(appointment);
    }

    private DoctorAppointmentDto MapToDoctorAppointmentDto(Appointment appointment)
    {
        return new DoctorAppointmentDto
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            PatientName = appointment.Patient.User.FirstName + " " + appointment.Patient.User.LastName,
            PatientPhone = appointment.Patient.User.PhoneNumber ?? "",
            PatientAddress = appointment.Patient.Address,
            ServiceId = appointment.ServiceId,
            ServiceName = appointment.Service.Name,
            ScheduledDateTime = appointment.ScheduledDateTime,
            DurationMinutes = appointment.DurationMinutes,
            Status = appointment.Status.ToString(),
            TotalAmount = appointment.TotalAmount,
            PatientNotes = appointment.PatientNotes,
            DoctorNotes = appointment.DoctorNotes
        };
    }

    #endregion

    #region Working Hours

    public async Task<List<WorkingHoursDto>> GetMyWorkingHoursAsync(Guid userId)
    {
        var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
        if (doctor == null)
            throw new Exception("Doctor not found");

        var workingHours = await _unitOfWork.WorkingHours.GetByDoctorIdAsync(doctor.Id);

        return workingHours.Select(wh => new WorkingHoursDto
        {
            Id = wh.Id,
            DayOfWeek = wh.DayOfWeek.ToString(),
            StartTime = wh.StartTime,
            EndTime = wh.EndTime,
            IsAvailable = wh.IsAvailable
        }).ToList();
    }

    public async Task<WorkingHoursDto> AddWorkingHoursAsync(Guid userId, CreateWorkingHoursDto dto)
    {
        var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
        if (doctor == null)
            throw new Exception("Doctor not found");

        var dayOfWeek = (DayOfWeek)dto.DayOfWeek;

        var workingHours = new DoctorWorkingHours
        {
            DoctorId = doctor.Id,
            DayOfWeek = dayOfWeek,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            IsAvailable = dto.IsAvailable
        };

        await _unitOfWork.WorkingHours.AddAsync(workingHours);
        await _unitOfWork.SaveChangesAsync();

        return new WorkingHoursDto
        {
            Id = workingHours.Id,
            DayOfWeek = workingHours.DayOfWeek.ToString(),
            StartTime = workingHours.StartTime,
            EndTime = workingHours.EndTime,
            IsAvailable = workingHours.IsAvailable
        };
    }

    public async Task<WorkingHoursDto> UpdateWorkingHoursAsync(Guid userId, Guid id, CreateWorkingHoursDto dto)
    {
        var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
        if (doctor == null)
            throw new Exception("Doctor not found");

        var workingHours = await _unitOfWork.WorkingHours.GetByIdAsync(id);
        if (workingHours == null)
            throw new Exception("Working hours not found");

        if (workingHours.DoctorId != doctor.Id)
            throw new UnauthorizedAccessException("You can only update your own working hours");

        workingHours.DayOfWeek = (DayOfWeek)dto.DayOfWeek;
        workingHours.StartTime = dto.StartTime;
        workingHours.EndTime = dto.EndTime;
        workingHours.IsAvailable = dto.IsAvailable;

        _unitOfWork.WorkingHours.Update(workingHours);
        await _unitOfWork.SaveChangesAsync();

        return new WorkingHoursDto
        {
            Id = workingHours.Id,
            DayOfWeek = workingHours.DayOfWeek.ToString(),
            StartTime = workingHours.StartTime,
            EndTime = workingHours.EndTime,
            IsAvailable = workingHours.IsAvailable
        };
    }

    public async Task DeleteWorkingHoursAsync(Guid userId, Guid id)
    {
        var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
        if (doctor == null)
            throw new Exception("Doctor not found");

        var workingHours = await _unitOfWork.WorkingHours.GetByIdAsync(id);
        if (workingHours == null)
            throw new Exception("Working hours not found");

        if (workingHours.DoctorId != doctor.Id)
            throw new UnauthorizedAccessException("You can only delete your own working hours");

        _unitOfWork.WorkingHours.Remove(workingHours);
        await _unitOfWork.SaveChangesAsync();
    }

    #endregion

    #region Document Management

    public async Task<List<DoctorDocumentDto>> GetMyDocumentsAsync(Guid userId)
    {
        var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
        if (doctor == null)
            throw new Exception("Doctor not found");

        var documents = await _unitOfWork.DoctorDocuments.GetByDoctorIdAsync(doctor.Id);

        return documents.Select(d => new DoctorDocumentDto
        {
            Id = d.Id,
            Title = d.Title,
            Description = d.Description,
            FilePath = d.FilePath,
            FileType = d.FileType,
            FileSize = d.FileSize,
            CreatedAt = d.CreatedAt,
            UpdatedAt = d.UpdatedAt
        }).ToList();
    }

    public async Task<DoctorDocumentDto> GetDocumentByIdAsync(Guid userId, Guid documentId)
    {
        var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
        if (doctor == null)
            throw new Exception("Doctor not found");

        var document = await _unitOfWork.DoctorDocuments.GetByIdAsync(documentId);
        if (document == null)
            throw new Exception("Document not found");

        if (document.DoctorId != doctor.Id)
            throw new UnauthorizedAccessException("You can only access your own documents");

        return new DoctorDocumentDto
        {
            Id = document.Id,
            Title = document.Title,
            Description = document.Description,
            FilePath = document.FilePath,
            FileType = document.FileType,
            FileSize = document.FileSize,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt
        };
    }

    public async Task<DoctorDocumentDto> CreateDocumentAsync(Guid userId, CreateDoctorDocumentDto dto, Microsoft.AspNetCore.Http.IFormFile? file)
    {
        var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
        if (doctor == null)
            throw new Exception("Doctor not found");

        var document = new DoctorDocument
        {
            DoctorId = doctor.Id,
            Title = dto.Title,
            Description = dto.Description
        };

        // Handle file upload
        if (file != null && file.Length > 0)
        {
            // Validate file size (5MB max)
            if (file.Length > 5 * 1024 * 1024)
                throw new Exception("File size cannot exceed 5MB");

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                throw new Exception("Only JPG, PNG, and PDF files are allowed");

            // Create upload directory if it doesn't exist
            var uploadPath = Path.Combine("wwwroot", "uploads", "doctor-documents");
            Directory.CreateDirectory(uploadPath);

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            document.FilePath = $"/uploads/doctor-documents/{fileName}";
            document.FileType = file.ContentType;
            document.FileSize = file.Length;
        }

        await _unitOfWork.DoctorDocuments.CreateAsync(document);
        await _unitOfWork.SaveChangesAsync();

        return new DoctorDocumentDto
        {
            Id = document.Id,
            Title = document.Title,
            Description = document.Description,
            FilePath = document.FilePath,
            FileType = document.FileType,
            FileSize = document.FileSize,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt
        };
    }

    public async Task<DoctorDocumentDto> UpdateDocumentAsync(Guid userId, Guid documentId, UpdateDoctorDocumentDto dto, Microsoft.AspNetCore.Http.IFormFile? file)
    {
        var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
        if (doctor == null)
            throw new Exception("Doctor not found");

        var document = await _unitOfWork.DoctorDocuments.GetByIdAsync(documentId);
        if (document == null)
            throw new Exception("Document not found");

        if (document.DoctorId != doctor.Id)
            throw new UnauthorizedAccessException("You can only update your own documents");

        document.Title = dto.Title;
        document.Description = dto.Description;

        // Handle file upload
        if (file != null && file.Length > 0)
        {
            // Validate file size (5MB max)
            if (file.Length > 5 * 1024 * 1024)
                throw new Exception("File size cannot exceed 5MB");

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                throw new Exception("Only JPG, PNG, and PDF files are allowed");

            // Delete old file if exists
            if (!string.IsNullOrEmpty(document.FilePath))
            {
                var oldFilePath = Path.Combine("wwwroot", document.FilePath.TrimStart('/'));
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
            }

            // Create upload directory if it doesn't exist
            var uploadPath = Path.Combine("wwwroot", "uploads", "doctor-documents");
            Directory.CreateDirectory(uploadPath);

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            document.FilePath = $"/uploads/doctor-documents/{fileName}";
            document.FileType = file.ContentType;
            document.FileSize = file.Length;
        }

        await _unitOfWork.DoctorDocuments.UpdateAsync(document);
        await _unitOfWork.SaveChangesAsync();

        return new DoctorDocumentDto
        {
            Id = document.Id,
            Title = document.Title,
            Description = document.Description,
            FilePath = document.FilePath,
            FileType = document.FileType,
            FileSize = document.FileSize,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt
        };
    }

    public async Task DeleteDocumentAsync(Guid userId, Guid documentId)
    {
        var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
        if (doctor == null)
            throw new Exception("Doctor not found");

        var document = await _unitOfWork.DoctorDocuments.GetByIdAsync(documentId);
        if (document == null)
            throw new Exception("Document not found");

        if (document.DoctorId != doctor.Id)
            throw new UnauthorizedAccessException("You can only delete your own documents");

        // Delete file if exists
        if (!string.IsNullOrEmpty(document.FilePath))
        {
            var filePath = Path.Combine("wwwroot", document.FilePath.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        await _unitOfWork.DoctorDocuments.DeleteAsync(documentId);
        await _unitOfWork.SaveChangesAsync();
    }

    #endregion

    #region Profile Management

    public async Task<DoctorDto> GetMyProfileAsync(Guid userId)
    {
        var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
        if (doctor == null)
            throw new Exception("Doctor not found");

        return new DoctorDto
        {
            Id = doctor.Id,
            UserId = doctor.UserId,
            Email = doctor.User.Email!,
            FirstName = doctor.User.FirstName,
            LastName = doctor.User.LastName,
            PhoneNumber = doctor.User.PhoneNumber ?? "",
            Specialization = doctor.Specialization,
            LicenseNumber = doctor.LicenseNumber,
            YearsOfExperience = doctor.YearsOfExperience,
            Bio = doctor.Bio,
            HourlyRate = doctor.HourlyRate,
            IsApproved = doctor.IsApproved,
            ApprovedAt = doctor.ApprovedAt
        };
    }

    public async Task<DoctorDto> UpdateMyProfileAsync(Guid userId, UpdateDoctorProfileDto dto)
    {
        var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
        if (doctor == null)
            throw new Exception("Doctor not found");

        doctor.Specialization = dto.Specialization;
        doctor.LicenseNumber = dto.LicenseNumber;
        doctor.YearsOfExperience = dto.YearsOfExperience;
        doctor.Bio = dto.Bio;

        _unitOfWork.Doctors.Update(doctor);
        await _unitOfWork.SaveChangesAsync();

        return new DoctorDto
        {
            Id = doctor.Id,
            UserId = doctor.UserId,
            Email = doctor.User.Email!,
            FirstName = doctor.User.FirstName,
            LastName = doctor.User.LastName,
            PhoneNumber = doctor.User.PhoneNumber ?? "",
            Specialization = doctor.Specialization,
            LicenseNumber = doctor.LicenseNumber,
            YearsOfExperience = doctor.YearsOfExperience,
            Bio = doctor.Bio,
            HourlyRate = doctor.HourlyRate,
            IsApproved = doctor.IsApproved,
            ApprovedAt = doctor.ApprovedAt
        };
    }

    #endregion
}
