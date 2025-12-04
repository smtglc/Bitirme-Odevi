using EvdeSaglik.Service.DTOs.Profile;

namespace EvdeSaglik.Service.Interfaces;

public interface IProfileService
{
    Task<UserProfileDto> GetUserProfileAsync(Guid userId);
    Task<UserProfileDto> UpdateProfileAsync(Guid userId, UpdateProfileDto dto);
}
