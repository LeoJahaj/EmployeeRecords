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
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly JwtSettings _jwtSettings;

        public UserService(IUserRepository userRepository, IOptions<JwtSettings> jwtOptions)
        {
            _userRepository = userRepository;
            _passwordHasher = new PasswordHasher<User>();
            _jwtSettings = jwtOptions.Value;
        }

        public string? Login(LoginDto loginDto)
        {
            // Find user by username
            var user = _userRepository
                .GetAll()
                .FirstOrDefault(u => u.UserName == loginDto.UserName);

            if (user == null) return null;

            // Verify entered password against stored hash
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);

            if (result == PasswordVerificationResult.Failed)
                return null;

            // ✅ Generate and return JWT token
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
                Role = user.Role
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
                // Add other properties you have in ProjectDto
            });
        }

        public UserDto CreateUser(UserDto userDto)
        {
            var user = new User
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                Role = userDto.Role
            };

            // Hash the password before saving
            var rawPassword = userDto.Password; // ensure UserDto has a Password property
            user.PasswordHash = _passwordHasher.HashPassword(user, rawPassword);

            _userRepository.Add(user);

            // Update dto with new Id from DB and hide password
            userDto.Id = user.Id;
            userDto.Password = null;
            return userDto;
        }

        public bool DeleteUser(int id)
        {
            var user = _userRepository.GetById(id);
            if (user == null) return false;

            _userRepository.Delete(user);
            return true;
        }

        // 🔑 Helper: JWT generation
        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.UserName ?? ""),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Role, user.Role.ToString()) // enum → string
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


