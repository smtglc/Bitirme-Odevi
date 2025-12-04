using EvdeSaglik.Entity.Entities;
using EvdeSaglik.Service.DTOs.Profile;
using EvdeSaglik.Service.Interfaces;
using EvdeSaglik.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace EvdeSaglik.Service.Services;

public class ProfileService : IProfileService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User> _userManager;

    public ProfileService(IUnitOfWork unitOfWork, UserManager<User> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<UserProfileDto> GetUserProfileAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new Exception("User not found");

        var roles = await _userManager.GetRolesAsync(user);
        var profile = new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber!,
            Roles = roles.ToList()
        };

       
        if (roles.Contains("Patient"))
        {
            var patient = await _unitOfWork.Patients.GetByUserIdAsync(userId);
            if (patient != null)
            {
                profile.DateOfBirth = patient.DateOfBirth;
                profile.Address = patient.Address;
                profile.EmergencyContact = patient.EmergencyContact;
            }
        }

        
        if (roles.Contains("Doctor"))
        {
            var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
            if (doctor != null)
            {
                profile.Specialization = doctor.Specialization;
                profile.LicenseNumber = doctor.LicenseNumber;
                profile.YearsOfExperience = doctor.YearsOfExperience;
                profile.Bio = doctor.Bio;
                profile.IsApproved = doctor.IsApproved;
            }
        }

        return profile;
    }

    public async Task<UserProfileDto> UpdateProfileAsync(Guid userId, UpdateProfileDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new Exception("User not found");

        
        if (!string.IsNullOrEmpty(dto.PhoneNumber))
        {
            user.PhoneNumber = dto.PhoneNumber;
            await _userManager.UpdateAsync(user);
        }

        var roles = await _userManager.GetRolesAsync(user);

        
        if (roles.Contains("Patient"))
        {
            var patient = await _unitOfWork.Patients.GetByUserIdAsync(userId);
            if (patient != null)
            {
                if (dto.DateOfBirth.HasValue) patient.DateOfBirth = dto.DateOfBirth;
                if (dto.Address != null) patient.Address = dto.Address;
                if (dto.EmergencyContact != null) patient.EmergencyContact = dto.EmergencyContact;
                await _unitOfWork.SaveChangesAsync();
            }
        }

        
        if (roles.Contains("Doctor"))
        {
            var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
            if (doctor != null && dto.Bio != null)
            {
                doctor.Bio = dto.Bio;
                await _unitOfWork.SaveChangesAsync();
            }
        }

        return await GetUserProfileAsync(userId);
    }
}
