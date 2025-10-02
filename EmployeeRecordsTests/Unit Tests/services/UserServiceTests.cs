using EmployeeRecordsApi.Services;
using EmployeeRecordsApi.Settings;
using EmployeeRecordsCore.DTOs;
using EmployeeRecordsCore.Models;
using EmployeeRecordsCore.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace EmployeeRecordsTests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly IOptions<JwtSettings> _jwtOptions;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _mockUserRepo = new Mock<IUserRepository>();

            // Fake JWT settings
            var jwtSettings = new JwtSettings
            {
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                Key = "ThisIsASuperSecretTestKey123!",
                DurationInMinutes = 60
            };
            _jwtOptions = Options.Create(jwtSettings);

            _service = new UserService(_mockUserRepo.Object, _jwtOptions);
        }

        // ---------------- LOGIN ----------------
        [Fact]
        public void Login_ShouldReturnToken_WhenCredentialsValid()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                UserName = "testuser",
                Role = UserRole.Administrator,
                PasswordHash = new PasswordHasher<User>().HashPassword(null!, "password123")
            };

            _mockUserRepo.Setup(r => r.GetAll()).Returns(new List<User> { user });

            var loginDto = new LoginDto { UserName = "testuser", Password = "password123" };

            // Act
            var token = _service.Login(loginDto);

            // Assert
            Assert.NotNull(token);
            Assert.IsType<string>(token);
        }

        [Fact]
        public void Login_ShouldReturnNull_WhenUserNotFound()
        {
            // Arrange
            _mockUserRepo.Setup(r => r.GetAll()).Returns(new List<User>());
            var loginDto = new LoginDto { UserName = "missing", Password = "password" };

            // Act
            var token = _service.Login(loginDto);

            // Assert
            Assert.Null(token);
        }

        [Fact]
        public void Login_ShouldReturnNull_WhenPasswordInvalid()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                UserName = "testuser",
                Role = UserRole.Employee,
                PasswordHash = new PasswordHasher<User>().HashPassword(null!, "correct")
            };

            _mockUserRepo.Setup(r => r.GetAll()).Returns(new List<User> { user });

            var loginDto = new LoginDto { UserName = "testuser", Password = "wrong" };

            // Act
            var token = _service.Login(loginDto);

            // Assert
            Assert.Null(token);
        }

        // ---------------- GET USER BY ID ----------------
        [Fact]
        public void GetUserById_ShouldReturnUserDto_WhenExists()
        {
            // Arrange
            var user = new User { Id = 1, UserName = "u1", Email = "u1@test.com", Role = UserRole.Employee };
            _mockUserRepo.Setup(r => r.GetById(1)).Returns(user);

            // Act
            var result = _service.GetUserById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result!.Id);
            Assert.Equal("u1", result.UserName);
            Assert.Equal("Employee", result.Role);
        }

        [Fact]
        public void GetUserById_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            _mockUserRepo.Setup(r => r.GetById(999)).Returns((User?)null);

            // Act
            var result = _service.GetUserById(999);

            // Assert
            Assert.Null(result);
        }

        // ---------------- GET PROJECTS FOR USER ----------------
        [Fact]
        public void GetProjectsForUser_ShouldReturnProjects()
        {
            // Arrange
            var projects = new List<Project>
            {
                new Project { Id = 1, Name = "Proj A", Description = "D1" },
                new Project { Id = 2, Name = "Proj B", Description = "D2" }
            };
            _mockUserRepo.Setup(r => r.GetProjectsForUser(1)).Returns(projects);

            // Act
            var result = _service.GetProjectsForUser(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.IsType<ProjectDto>(p));
        }

        // ---------------- CREATE USER ----------------
        [Fact]
        public void CreateUser_ShouldReturnUserDto_WithHashedPassword()
        {
            // Arrange
            var userDto = new UserDto
            {
                UserName = "newuser",
                Email = "new@test.com",
                Role = "Administrator",
                Password = "password123"
            };

            _mockUserRepo.Setup(r => r.Add(It.IsAny<User>()))
                .Callback<User>(u => u.Id = 10); // simulate DB-generated Id

            // Act
            var result = _service.CreateUser(userDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.Id);
            Assert.Null(result.Password); // password should be cleared in DTO
            _mockUserRepo.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
        }

        // ---------------- DELETE USER ----------------
        [Fact]
        public void DeleteUser_ShouldReturnTrue_WhenExists()
        {
            // Arrange
            var user = new User { Id = 1 };
            _mockUserRepo.Setup(r => r.GetById(1)).Returns(user);

            // Act
            var result = _service.DeleteUser(1);

            // Assert
            Assert.True(result);
            _mockUserRepo.Verify(r => r.Delete(user), Times.Once);
        }

        [Fact]
        public void DeleteUser_ShouldReturnFalse_WhenNotFound()
        {
            // Arrange
            _mockUserRepo.Setup(r => r.GetById(999)).Returns((User?)null);

            // Act
            var result = _service.DeleteUser(999);

            // Assert
            Assert.False(result);
            _mockUserRepo.Verify(r => r.Delete(It.IsAny<User>()), Times.Never);
        }
    }
}
