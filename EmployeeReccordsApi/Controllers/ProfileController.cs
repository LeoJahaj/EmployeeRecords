using EmployeeRecordsCore.DTOs;
using EmployeeRecordsCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeRecordsApi.Controllers
{
    /// <summary>
    /// Manages user profiles.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        /// <summary>
        /// Get the profile of a user by user ID.
        /// - Admins can fetch any profile.  
        /// - Employees can only fetch their own profile.
        /// </summary>
        [Authorize]
        [HttpGet("{userId}")]
        public IActionResult GetProfile(int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int currentUserId = userIdClaim != null ? int.Parse(userIdClaim) : 0;
            bool isAdmin = User.IsInRole("Administrator");

            // ⛔ Block employees trying to view other profiles
            if (!isAdmin && currentUserId != userId)
                return Forbid();

            var profile = _profileService.GetProfileByUserId(userId);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        /// <summary>
        /// Update the profile of a user.
        /// - Admins can update any profile.  
        /// - Employees can only update their own profile.
        /// </summary>
        [Authorize]
        [HttpPut("{userId}")]
        public IActionResult UpdateProfile(int userId, [FromBody] ProfileDto profileDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int currentUserId = userIdClaim != null ? int.Parse(userIdClaim) : 0;
            bool isAdmin = User.IsInRole("Administrator");

            // ⛔ Block employees trying to update other profiles
            if (!isAdmin && currentUserId != userId)
                return Forbid();

            var updated = _profileService.UpdateProfile(userId, profileDto);
            if (!updated) return NotFound();
            return NoContent();
        }
    }
}


