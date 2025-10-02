using EmployeeRecordsApi.Services;
using EmployeeRecordsCore.DTOs;
using EmployeeRecordsCore.Models;
using EmployeeRecordsCore.Repositories;
using Moq;
using Xunit;

namespace EmployeeRecordsTests.Services
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _mockTaskRepo;
        private readonly TaskService _service;

        public TaskServiceTests()
        {
            _mockTaskRepo = new Mock<ITaskRepository>();
            _service = new TaskService(_mockTaskRepo.Object);
        }

        // ---------------- GET BY PROJECT ----------------
        [Fact]
        public void GetTasksByProject_ShouldReturnTasks()
        {
            // Arrange
            var tasks = new List<Tasks>
            {
                new Tasks { Id = 1, Title = "T1", Description = "D1", ProjectId = 1, AssignedToUserId = 5 },
                new Tasks { Id = 2, Title = "T2", Description = "D2", ProjectId = 1, AssignedToUserId = 6 }
            };
            _mockTaskRepo.Setup(r => r.GetByProjectId(1)).Returns(tasks);

            // Act
            var result = _service.GetTasksByProject(1);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, t => Assert.IsType<TaskDto>(t));
        }

        // ---------------- GET BY ID ----------------
        [Fact]
        public void GetTaskById_ShouldReturnTask_WhenExists()
        {
            // Arrange
            var task = new Tasks { Id = 1, Title = "T1", Description = "D1", ProjectId = 1, AssignedToUserId = 5 };
            _mockTaskRepo.Setup(r => r.GetById(1)).Returns(task);

            // Act
            var result = _service.GetTaskById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("T1", result!.Title);
        }

        [Fact]
        public void GetTaskById_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            _mockTaskRepo.Setup(r => r.GetById(999)).Returns((Tasks?)null);

            // Act
            var result = _service.GetTaskById(999);

            // Assert
            Assert.Null(result);
        }

        // ---------------- CREATE ----------------
        [Fact]
        public void CreateTask_ShouldReturnDto_WithId()
        {
            // Arrange
            var dto = new TaskDto { Title = "NewTask", Description = "D", ProjectId = 1, AssignedToUserId = 5 };
            _mockTaskRepo.Setup(r => r.Add(It.IsAny<Tasks>()))
                .Callback<Tasks>(t => t.Id = 10); // simulate DB id

            // Act
            var result = _service.CreateTask(dto);

            // Assert
            Assert.Equal(10, result.Id);
            _mockTaskRepo.Verify(r => r.Add(It.IsAny<Tasks>()), Times.Once);
        }

        // ---------------- UPDATE ----------------
        [Fact]
        public void UpdateTask_ShouldReturnTrue_WhenExists()
        {
            // Arrange
            var existing = new Tasks { Id = 1, Title = "Old", Description = "Old", ProjectId = 1, AssignedToUserId = 5 };
            var dto = new TaskDto { Title = "Updated", Description = "New", ProjectId = 2, AssignedToUserId = 6, IsCompleted = true };

            _mockTaskRepo.Setup(r => r.GetById(1)).Returns(existing);

            // Act
            var result = _service.UpdateTask(1, dto);

            // Assert
            Assert.True(result);
            _mockTaskRepo.Verify(r => r.Update(It.Is<Tasks>(t => t.Title == "Updated")), Times.Once);
        }

        [Fact]
        public void UpdateTask_ShouldReturnFalse_WhenNotExists()
        {
            // Arrange
            _mockTaskRepo.Setup(r => r.GetById(1)).Returns((Tasks?)null);

            // Act
            var result = _service.UpdateTask(1, new TaskDto());

            // Assert
            Assert.False(result);
        }

        // ---------------- DELETE ----------------
        [Fact]
        public void DeleteTask_ShouldReturnTrue_WhenExists()
        {
            // Arrange
            var task = new Tasks { Id = 1 };
            _mockTaskRepo.Setup(r => r.GetById(1)).Returns(task);

            // Act
            var result = _service.DeleteTask(1);

            // Assert
            Assert.True(result);
            _mockTaskRepo.Verify(r => r.Delete(task), Times.Once);
        }

        [Fact]
        public void DeleteTask_ShouldReturnFalse_WhenNotExists()
        {
            // Arrange
            _mockTaskRepo.Setup(r => r.GetById(999)).Returns((Tasks?)null);

            // Act
            var result = _service.DeleteTask(999);

            // Assert
            Assert.False(result);
        }
    }
}

