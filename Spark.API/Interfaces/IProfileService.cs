using Spark.API.DTOs.Profile;

namespace Spark.API.Interfaces
{
    public interface IProfileService
    {
        Task<ProfileDto> GetProfile(Guid userId);
        Task<ProfileDto> UpdateProfile(Guid userId, UpdateProfileDto dto);

        Task<ProfileDto> DeleteProfile(Guid userId);

        Task<ProfileDto> UpdateProfileImage(Guid userId, IFormFile image);

        Task<ProfileDto> GetUserProfile(string username);
    }
}
