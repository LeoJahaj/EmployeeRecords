using Xunit;
using Moq;
using EmployeeRecordsApi.Controllers;
using EmployeeRecordsCore.DTOs;
using EmployeeRecordsCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeRecordsTests.Controllers
{
    public class ProfileControllerTests
    {
        private readonly Mock<IProfileService> _mockProfileService;
        private readonly ProfileController _controller;

        public ProfileControllerTests()
        {
            _mockProfileService = new Mock<IProfileService>();
            _controller = new ProfileController(_mockProfileService.Object);
        }

        // ---------------- GET PROFILE ----------------
        [Fact]
        public void GetProfile_ShouldReturnProfile_WhenExists()
        {
            // Arrange
            var profile = new ProfileDto { UserId = 1, FullName = "John Doe", Bio = "Developer" };
            _mockProfileService.Setup(s => s.GetProfileByUserId(1)).Returns(profile);

            // Act
            var result = _controller.GetProfile(1) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var returned = Assert.IsType<ProfileDto>(result.Value);
            Assert.Equal("John Doe", returned.FullName);
        }

        [Fact]
        public void GetProfile_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            _mockProfileService.Setup(s => s.GetProfileByUserId(1)).Returns((ProfileDto?)null);

            // Act
            var result = _controller.GetProfile(1) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }

        // ---------------- UPDATE PROFILE ----------------
        [Fact]
        public void UpdateProfile_ShouldReturnNoContent_WhenSuccess()
        {
            // Arrange
            var dto = new ProfileDto { UserId = 1, FullName = "Updated Name", Bio = "Updated Bio" };
            _mockProfileService.Setup(s => s.UpdateProfile(1, dto)).Returns(true);

            // Act
            var result = _controller.UpdateProfile(1, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void UpdateProfile_ShouldReturnNotFound_WhenProfileMissing()
        {
            // Arrange
            var dto = new ProfileDto { UserId = 1, FullName = "Updated Name" };
            _mockProfileService.Setup(s => s.UpdateProfile(1, dto)).Returns(false);

            // Act
            var result = _controller.UpdateProfile(1, dto) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }
    }
}

