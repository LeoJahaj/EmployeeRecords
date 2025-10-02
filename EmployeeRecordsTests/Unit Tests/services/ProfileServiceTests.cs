using EmployeeRecordsApi.Services;
using EmployeeRecordsCore.DTOs;
using EmployeeRecordsCore.Models;
using EmployeeRecordsCore.Repositories;
using Moq;
using Xunit;

namespace EmployeeRecordsTests.Services
{
    public class ProfileServiceTests
    {
        private readonly Mock<IProfileRepository> _mockProfileRepo;
        private readonly ProfileService _service;

        public ProfileServiceTests()
        {
            _mockProfileRepo = new Mock<IProfileRepository>();
            _service = new ProfileService(_mockProfileRepo.Object);
        }

        // ---------------- GET PROFILE ----------------
        [Fact]
        public void GetProfileByUserId_ShouldReturnProfile_WhenExists()
        {
            // Arrange
            var profile = new Profile
            {
                UserId = 1,
                FullName = "John Doe",
                Bio = "Developer",
                ProfilePictureUrl = "http://example.com/pic.jpg"
            };
            _mockProfileRepo.Setup(r => r.GetByUserId(1)).Returns(profile);

            // Act
            var result = _service.GetProfileByUserId(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John Doe", result!.FullName);
            Assert.Equal("Developer", result.Bio);
        }

        [Fact]
        public void GetProfileByUserId_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            _mockProfileRepo.Setup(r => r.GetByUserId(99)).Returns((Profile?)null);

            // Act
            var result = _service.GetProfileByUserId(99);

            // Assert
            Assert.Null(result);
        }

        // ---------------- UPDATE PROFILE ----------------
        [Fact]
        public void UpdateProfile_ShouldReturnTrue_WhenExists()
        {
            // Arrange
            var existing = new Profile { UserId = 1, FullName = "Old", Bio = "Old", ProfilePictureUrl = "old.jpg" };
            var dto = new ProfileDto { UserId = 1, FullName = "New Name", Bio = "Updated", ProfilePictureUrl = "new.jpg" };

            _mockProfileRepo.Setup(r => r.GetByUserId(1)).Returns(existing);

            // Act
            var result = _service.UpdateProfile(1, dto);

            // Assert
            Assert.True(result);
            _mockProfileRepo.Verify(r => r.Update(It.Is<Profile>(p =>
                p.FullName == "New Name" &&
                p.Bio == "Updated" &&
                p.ProfilePictureUrl == "new.jpg"
            )), Times.Once);
        }

        [Fact]
        public void UpdateProfile_ShouldReturnFalse_WhenNotExists()
        {
            // Arrange
            _mockProfileRepo.Setup(r => r.GetByUserId(1)).Returns((Profile?)null);

            // Act
            var result = _service.UpdateProfile(1, new ProfileDto());

            // Assert
            Assert.False(result);
        }
    }
}

