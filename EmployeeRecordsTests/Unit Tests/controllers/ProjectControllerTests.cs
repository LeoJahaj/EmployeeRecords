using Xunit;
using Moq;
using EmployeeRecordsApi.Controllers;
using EmployeeRecordsCore.DTOs;
using EmployeeRecordsCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace EmployeeRecordsTests.Controllers
{
    public class ProjectControllerTests
    {
        private readonly Mock<IProjectService> _mockProjectService;
        private readonly ProjectController _controller;

        public ProjectControllerTests()
        {
            _mockProjectService = new Mock<IProjectService>();
            _controller = new ProjectController(_mockProjectService.Object);
        }

        // ---------------- GET ALL ----------------
        [Fact]
        public void GetAllProjects_ShouldReturnProjects()
        {
            // Arrange
            var projects = new List<ProjectDto>
            {
                new ProjectDto { Id = 1, Name = "Proj A", Description = "Test" },
                new ProjectDto { Id = 2, Name = "Proj B", Description = "Test 2" }
            };
            _mockProjectService.Setup(s => s.GetAllProjects()).Returns(projects);

            // Act
            var result = _controller.GetAllProjects() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var returned = Assert.IsAssignableFrom<IEnumerable<ProjectDto>>(result.Value);
            Assert.Equal(2, ((List<ProjectDto>)returned).Count);
        }

        // ---------------- GET BY ID ----------------
        [Fact]
        public void GetProjectById_ShouldReturnProject_WhenExists()
        {
            // Arrange
            var project = new ProjectDto { Id = 1, Name = "Proj A", Description = "Test" };
            _mockProjectService.Setup(s => s.GetProjectById(1)).Returns(project);

            // Act
            var result = _controller.GetProjectById(1) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var returned = Assert.IsType<ProjectDto>(result.Value);
            Assert.Equal(1, returned.Id);
        }

        [Fact]
        public void GetProjectById_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            _mockProjectService.Setup(s => s.GetProjectById(999)).Returns((ProjectDto?)null);

            // Act
            var result = _controller.GetProjectById(999) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }

        // ---------------- CREATE ----------------
        [Fact]
        public void CreateProject_ShouldReturnCreated_WhenValid()
        {
            // Arrange
            var project = new ProjectDto { Id = 1, Name = "NewProj", Description = "Desc" };
            _mockProjectService.Setup(s => s.CreateProject(It.IsAny<ProjectDto>())).Returns(project);

            // Act
            var result = _controller.CreateProject(new ProjectDto()) as CreatedAtActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode);
            var returned = Assert.IsType<ProjectDto>(result.Value);
            Assert.Equal("NewProj", returned.Name);
        }

        // ---------------- UPDATE ----------------
        [Fact]
        public void UpdateProject_ShouldReturnNoContent_WhenSuccess()
        {
            // Arrange
            _mockProjectService.Setup(s => s.UpdateProject(1, It.IsAny<ProjectDto>())).Returns(true);

            // Act
            var result = _controller.UpdateProject(1, new ProjectDto());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void UpdateProject_ShouldReturnNotFound_WhenFails()
        {
            // Arrange
            _mockProjectService.Setup(s => s.UpdateProject(999, It.IsAny<ProjectDto>())).Returns(false);

            // Act
            var result = _controller.UpdateProject(999, new ProjectDto()) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }

        // ---------------- DELETE ----------------
        [Fact]
        public void DeleteProject_ShouldReturnNoContent_WhenDeleted()
        {
            // Arrange
            _mockProjectService.Setup(s => s.DeleteProject(1)).Returns(true);

            // Act
            var result = _controller.DeleteProject(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void DeleteProject_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            _mockProjectService.Setup(s => s.DeleteProject(999)).Returns(false);

            // Act
            var result = _controller.DeleteProject(999) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }

        // ---------------- CHECK USER ----------------
        [Fact]
        public void IsUserInProject_ShouldReturnOk()
        {
            // Arrange
            _mockProjectService.Setup(s => s.IsUserInProject(1, 2)).Returns(true);

            // Act
            var result = _controller.IsUserInProject(1, 2) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.True(((dynamic)result.Value).IsInProject);
        }

        // ---------------- GET USER PROJECTS ----------------
        [Fact]
        public void GetProjectsForUser_ShouldReturnProjects()
        {
            // Arrange
            var projects = new List<ProjectDto>
            {
                new ProjectDto { Id = 1, Name = "Proj A" }
            };
            _mockProjectService.Setup(s => s.GetProjectsForUser(1)).Returns(projects);

            // Act
            var result = _controller.GetProjectsForUser(1) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var returned = Assert.IsAssignableFrom<IEnumerable<ProjectDto>>(result.Value);
            Assert.Single(returned);
        }

        // ---------------- ADD USER TO PROJECT ----------------
        [Fact]
        public void AddUserToProject_ShouldReturnOk_WhenSuccess()
        {
            // Arrange
            _mockProjectService.Setup(s => s.AddUserToProject(1, 2)).Returns(true);

            // Act
            var result = _controller.AddUserToProject(1, 2) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Contains("added", result.Value!.ToString());
        }

        [Fact]
        public void AddUserToProject_ShouldReturnNotFound_WhenFails()
        {
            // Arrange
            _mockProjectService.Setup(s => s.AddUserToProject(1, 2)).Returns(false);

            // Act
            var result = _controller.AddUserToProject(1, 2) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }

        // ---------------- REMOVE USER FROM PROJECT ----------------
        [Fact]
        public void RemoveUserFromProject_ShouldReturnOk_WhenSuccess()
        {
            // Arrange
            _mockProjectService.Setup(s => s.RemoveUserFromProject(1, 2)).Returns(true);

            // Act
            var result = _controller.RemoveUserFromProject(1, 2) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Contains("removed", result.Value!.ToString());
        }

        [Fact]
        public void RemoveUserFromProject_ShouldReturnNotFound_WhenFails()
        {
            // Arrange
            _mockProjectService.Setup(s => s.RemoveUserFromProject(1, 2)).Returns(false);

            // Act
            var result = _controller.RemoveUserFromProject(1, 2) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }
    }
}

