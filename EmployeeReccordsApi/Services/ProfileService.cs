using EmployeeRecordsCore.DTOs;
using EmployeeRecordsCore.Interfaces;
using EmployeeRecordsCore.Models;
using EmployeeRecordsCore.Repositories;



namespace EmployeeRecordsApi.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepository;

        public ProfileService(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public ProfileDto? GetProfileByUserId(int userId)
        {
            var profile = _profileRepository.GetByUserId(userId);
            if (profile == null) return null;

            return new ProfileDto
            {
                UserId = profile.UserId,
                FullName = profile.FullName,
                Bio = profile.Bio,
                ProfilePictureUrl = profile.ProfilePictureUrl
            };
        }

        public bool UpdateProfile(int userId, ProfileDto profileDto)
        {
            var profile = _profileRepository.GetByUserId(userId);
            if (profile == null) return false;

            // Update entity properties from DTO
            profile.FullName = profileDto.FullName;
            profile.Bio = profileDto.Bio;
            profile.ProfilePictureUrl = profileDto.ProfilePictureUrl;

            _profileRepository.Update(profile);
            return true;
        }
    }
}



