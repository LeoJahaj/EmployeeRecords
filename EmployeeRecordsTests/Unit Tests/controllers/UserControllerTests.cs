using Xunit;
using Moq;
using EmployeeRecordsApi.Controllers;
using EmployeeRecordsCore.DTOs;
using EmployeeRecordsCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace EmployeeRecordsTests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new UserController(_mockUserService.Object);
        }

        // ---------------- LOGIN ----------------
        [Fact]
        public void Login_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var loginDto = new LoginDto { UserName = "validuser", Password = "password123" };
            _mockUserService.Setup(s => s.Login(loginDto)).Returns("fake-jwt-token");

            // Act
            var result = _controller.Login(loginDto) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Contains("fake-jwt-token", result.Value!.ToString());
        }

        [Fact]
        public void Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginDto = new LoginDto { UserName = "invalid", Password = "wrong" };
            _mockUserService.Setup(s => s.Login(loginDto)).Returns((string?)null);

            // Act
            var result = _controller.Login(loginDto) as UnauthorizedObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
        }

        // ---------------- GET USER ----------------
        [Fact]
        public void GetUserById_ShouldReturnOk_WhenUserExists()
        {
            // Arrange
            var user = new UserDto { Id = 1, UserName = "validuser", Email = "test@test.com", Role = "Employee" };
            _mockUserService.Setup(s => s.GetUserById(1)).Returns(user);

            // Act
            var result = _controller.GetUserById(1) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var returnedUser = Assert.IsType<UserDto>(result.Value);
            Assert.Equal(user.Id, returnedUser.Id);
        }

        [Fact]
        public void GetUserById_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetUserById(999)).Returns((UserDto?)null);

            // Act
            var result = _controller.GetUserById(999) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }

        // ---------------- CREATE USER ----------------
        [Fact]
        public void CreateUser_ShouldReturnCreatedUser_WhenValid()
        {
            // Arrange
            var newUser = new UserDto { Id = 99, UserName = "newuser", Email = "new@test.com", Role = "Employee" };
            _mockUserService.Setup(s => s.CreateUser(It.IsAny<UserDto>())).Returns(newUser);

            // Act
            var result = _controller.CreateUser(new UserDto()) as CreatedAtActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode);
            var user = Assert.IsType<UserDto>(result.Value);
            Assert.Equal(99, user.Id);
        }

        // ---------------- DELETE USER ----------------
        [Fact]
        public void DeleteUser_ShouldReturnNoContent_WhenUserExists()
        {
            // Arrange
            _mockUserService.Setup(s => s.DeleteUser(1)).Returns(true);

            // Act
            var result = _controller.DeleteUser(1) as NoContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(204, result.StatusCode);
        }

        [Fact]
        public void DeleteUser_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            _mockUserService.Setup(s => s.DeleteUser(999)).Returns(false);

            // Act
            var result = _controller.DeleteUser(999) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }

        // ---------------- GET PROJECTS ----------------
        [Fact]
        public void GetProjectsForUser_ShouldReturnProjects()
        {
            // Arrange
            var projects = new List<ProjectDto>
            {
                new ProjectDto { Id = 10, Name = "Demo", Description = "Test" }
            };
            _mockUserService.Setup(s => s.GetProjectsForUser(1)).Returns(projects);

            // Act
            var result = _controller.GetProjectsForUser(1) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var returned = Assert.IsAssignableFrom<IEnumerable<ProjectDto>>(result.Value);
            Assert.Single(returned);
        }
    }
}
