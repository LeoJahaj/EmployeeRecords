using EmployeeRecordsApi.Services;
using EmployeeRecordsCore.DTOs;
using EmployeeRecordsCore.Models;
using EmployeeRecordsCore.Repositories;
using Moq;
using Xunit;

namespace EmployeeRecordsTests.Services
{
    public class ProjectServiceTests
    {
        private readonly Mock<IProjectRepository> _mockProjectRepo;
        private readonly ProjectService _service;

        public ProjectServiceTests()
        {
            _mockProjectRepo = new Mock<IProjectRepository>();
            _service = new ProjectService(_mockProjectRepo.Object);
        }

        // ---------------- GET ALL ----------------
        [Fact]
        public void GetAllProjects_ShouldReturnAll()
        {
            // Arrange
            var projects = new List<Project>
            {
                new Project { Id = 1, Name = "P1", Description = "D1", ProjectUsers = new List<ProjectUser>() },
                new Project { Id = 2, Name = "P2", Description = "D2", ProjectUsers = new List<ProjectUser>() }
            };
            _mockProjectRepo.Setup(r => r.GetAll()).Returns(projects);

            // Act
            var result = _service.GetAllProjects();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.IsType<ProjectDto>(p));
        }

        // ---------------- GET BY ID ----------------
        [Fact]
        public void GetProjectById_ShouldReturnDto_WhenExists()
        {
            // Arrange
            var project = new Project
            {
                Id = 1,
                Name = "P1",
                Description = "D1",
                ProjectUsers = new List<ProjectUser> { new ProjectUser { UserId = 5 } }
            };
            _mockProjectRepo.Setup(r => r.GetById(1)).Returns(project);

            // Act
            var result = _service.GetProjectById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("P1", result!.Name);
            Assert.Contains(5, result.UserIds);
        }

        [Fact]
        public void GetProjectById_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            _mockProjectRepo.Setup(r => r.GetById(999)).Returns((Project?)null);

            // Act
            var result = _service.GetProjectById(999);

            // Assert
            Assert.Null(result);
        }

        // ---------------- CREATE ----------------
        [Fact]
        public void CreateProject_ShouldReturnDto_WithId()
        {
            // Arrange
            var dto = new ProjectDto { Name = "New", Description = "Desc", UserIds = new List<int> { 1 } };
            _mockProjectRepo.Setup(r => r.Add(It.IsAny<Project>()))
                .Callback<Project>(p => p.Id = 10); // simulate DB id

            // Act
            var result = _service.CreateProject(dto);

            // Assert
            Assert.Equal(10, result.Id);
            _mockProjectRepo.Verify(r => r.Add(It.IsAny<Project>()), Times.Once);
        }

        // ---------------- UPDATE ----------------
        [Fact]
        public void UpdateProject_ShouldReturnTrue_WhenExists()
        {
            // Arrange
            var existing = new Project { Id = 1, Name = "Old", Description = "Old desc", ProjectUsers = new List<ProjectUser>() };
            var dto = new ProjectDto { Name = "Updated", Description = "New", UserIds = new List<int> { 2 } };

            _mockProjectRepo.Setup(r => r.GetById(1)).Returns(existing);

            // Act
            var result = _service.UpdateProject(1, dto);

            // Assert
            Assert.True(result);
            _mockProjectRepo.Verify(r => r.Update(It.Is<Project>(p => p.Name == "Updated")), Times.Once);
        }

        [Fact]
        public void UpdateProject_ShouldReturnFalse_WhenNotExists()
        {
            // Arrange
            _mockProjectRepo.Setup(r => r.GetById(1)).Returns((Project?)null);

            // Act
            var result = _service.UpdateProject(1, new ProjectDto());

            // Assert
            Assert.False(result);
        }

        // ---------------- DELETE ----------------
        [Fact]
        public void DeleteProject_ShouldReturnTrue_WhenExists()
        {
            // Arrange
            var proj = new Project { Id = 1 };
            _mockProjectRepo.Setup(r => r.GetById(1)).Returns(proj);

            // Act
            var result = _service.DeleteProject(1);

            // Assert
            Assert.True(result);
            _mockProjectRepo.Verify(r => r.Delete(proj), Times.Once);
        }

        [Fact]
        public void DeleteProject_ShouldReturnFalse_WhenNotExists()
        {
            // Arrange
            _mockProjectRepo.Setup(r => r.GetById(999)).Returns((Project?)null);

            // Act
            var result = _service.DeleteProject(999);

            // Assert
            Assert.False(result);
        }

        // ---------------- IS USER IN PROJECT ----------------
        [Fact]
        public void IsUserInProject_ShouldReturnTrue_WhenUserExists()
        {
            // Arrange
            var project = new Project
            {
                Id = 1,
                ProjectUsers = new List<ProjectUser> { new ProjectUser { UserId = 5 } }
            };
            _mockProjectRepo.Setup(r => r.GetById(1)).Returns(project);

            // Act
            var result = _service.IsUserInProject(1, 5);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsUserInProject_ShouldReturnFalse_WhenNotFound()
        {
            // Arrange
            _mockProjectRepo.Setup(r => r.GetById(1)).Returns((Project?)null);

            // Act
            var result = _service.IsUserInProject(1, 5);

            // Assert
            Assert.False(result);
        }

        // ---------------- GET PROJECTS FOR USER ----------------
        [Fact]
        public void GetProjectsForUser_ShouldReturnProjects()
        {
            // Arrange
            var projects = new List<Project>
            {
                new Project { Id = 1, ProjectUsers = new List<ProjectUser>{ new ProjectUser{ UserId = 1 } } },
                new Project { Id = 2, ProjectUsers = new List<ProjectUser>{ new ProjectUser{ UserId = 2 } } }
            };
            _mockProjectRepo.Setup(r => r.GetAll()).Returns(projects);

            // Act
            var result = _service.GetProjectsForUser(1);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result.First().Id);
        }

        // ---------------- ADD USER TO PROJECT ----------------
        [Fact]
        public void AddUserToProject_ShouldReturnTrue_WhenSuccess()
        {
            // Arrange
            var project = new Project { Id = 1, ProjectUsers = new List<ProjectUser>() };
            _mockProjectRepo.Setup(r => r.GetById(1)).Returns(project);

            // Act
            var result = _service.AddUserToProject(1, 5);

            // Assert
            Assert.True(result);
            Assert.Contains(project.ProjectUsers, pu => pu.UserId == 5);
        }

        [Fact]
        public void AddUserToProject_ShouldReturnFalse_WhenProjectNotFound()
        {
            // Arrange
            _mockProjectRepo.Setup(r => r.GetById(1)).Returns((Project?)null);

            // Act
            var result = _service.AddUserToProject(1, 5);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void AddUserToProject_ShouldNotDuplicate_WhenUserAlreadyExists()
        {
            // Arrange
            var project = new Project
            {
                Id = 1,
                ProjectUsers = new List<ProjectUser> { new ProjectUser { UserId = 5 } }
            };
            _mockProjectRepo.Setup(r => r.GetById(1)).Returns(project);

            // Act
            var result = _service.AddUserToProject(1, 5);

            // Assert
            Assert.True(result);
            Assert.Single(project.ProjectUsers); // still only 1
        }

        // ---------------- REMOVE USER FROM PROJECT ----------------
        [Fact]
        public void RemoveUserFromProject_ShouldReturnTrue_WhenSuccess()
        {
            // Arrange
            var project = new Project
            {
                Id = 1,
                ProjectUsers = new List<ProjectUser> { new ProjectUser { UserId = 5 } }
            };
            _mockProjectRepo.Setup(r => r.GetById(1)).Returns(project);

            // Act
            var result = _service.RemoveUserFromProject(1, 5);

            // Assert
            Assert.True(result);
            Assert.Empty(project.ProjectUsers);
        }

        [Fact]
        public void RemoveUserFromProject_ShouldReturnFalse_WhenProjectNotFound()
        {
            // Arrange
            _mockProjectRepo.Setup(r => r.GetById(1)).Returns((Project?)null);

            // Act
            var result = _service.RemoveUserFromProject(1, 5);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void RemoveUserFromProject_ShouldReturnFalse_WhenUserNotInProject()
        {
            // Arrange
            var project = new Project { Id = 1, ProjectUsers = new List<ProjectUser>() };
            _mockProjectRepo.Setup(r => r.GetById(1)).Returns(project);

            // Act
            var result = _service.RemoveUserFromProject(1, 5);

            // Assert
            Assert.False(result);
        }
    }
}
