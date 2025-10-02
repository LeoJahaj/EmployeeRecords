using EmployeeRecordsApi.Settings;
using EmployeeRecordsCore.DTOs;
using EmployeeRecordsCore.Interfaces;
using EmployeeRecordsCore.Models;
using EmployeeRecordsCore.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmployeeRecordsApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly JwtSettings _jwtSettings;

        // ✅ inject both repositories
        public UserService(IUserRepository userRepository, IProfileRepository profileRepository, IOptions<JwtSettings> jwtOptions)
        {
            _userRepository = userRepository;
            _profileRepository = profileRepository;
            _passwordHasher = new PasswordHasher<User>();
            _jwtSettings = jwtOptions.Value;
        }

        public string? Login(LoginDto loginDto)
        {
            var user = _userRepository
                .GetAll()
                .FirstOrDefault(u => u.UserName == loginDto.UserName);

            if (user == null) return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);

            if (result == PasswordVerificationResult.Failed)
                return null;

            return GenerateJwtToken(user);
        }

        public UserDto? GetUserById(int id)
        {
            var user = _userRepository.GetById(id);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role.ToString() // ✅ enum → string
            };
        }

        public IEnumerable<ProjectDto> GetProjectsForUser(int userId)
        {
            var projects = _userRepository.GetProjectsForUser(userId);

            return projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description
            });
        }

        public UserDto CreateUser(UserDto userDto)
        {
            var user = new User
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                Role = Enum.Parse<UserRole>(userDto.Role, true) // ✅ string → enum
            };

            var rawPassword = userDto.Password ?? throw new ArgumentException("Password is required");
            user.PasswordHash = _passwordHasher.HashPassword(user, rawPassword);

            _userRepository.Add(user);

            // ✅ After user is created → create default Profile
            var profile = new Profile
            {
                UserId = user.Id,
                FullName = userDto.UserName ?? "",
                Bio = "New employee",
                ProfilePictureUrl = ""
            };
            _profileRepository.Add(profile);

            // ✅ Return UserDto with Id + without password
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role.ToString(), // ✅ fix enum → string
                Password = null
            };
        }

        public IEnumerable<UserDto> GetAllUsers()
        {
            var users = _userRepository.GetAll();

            return users.Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                // Optional: Map Profile data if needed
                //Profile = u.Profile != null ? new ProfileDto
                //{
                //    FirstName = u.Profile.FirstName,
                //    LastName = u.Profile.LastName
                //} : null,
                //// Optional: Projects list if needed
                //ProjectIds = u.ProjectUsers?.Select(pu => pu.ProjectId).ToList()
            }).ToList();
        }


        public bool DeleteUser(int id)
        {
            var user = _userRepository.GetById(id);
            if (user == null) return false;

            _userRepository.Delete(user);
            return true;
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}



