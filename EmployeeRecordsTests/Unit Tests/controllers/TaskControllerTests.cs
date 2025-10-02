using Xunit;
using Moq;
using EmployeeRecordsApi.Controllers;
using EmployeeRecordsCore.DTOs;
using EmployeeRecordsCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace EmployeeRecordsTests.Controllers
{
    public class TaskControllerTests
    {
        private readonly Mock<ITaskService> _mockTaskService;
        private readonly Mock<IProjectService> _mockProjectService;
        private readonly TaskController _controller;

        public TaskControllerTests()
        {
            _mockTaskService = new Mock<ITaskService>();
            _mockProjectService = new Mock<IProjectService>();
            _controller = new TaskController(_mockTaskService.Object, _mockProjectService.Object);
        }

        // ---------------- GET TASKS BY PROJECT ----------------
        [Fact]
        public void GetTasksByProject_ShouldReturnTasks()
        {
            // Arrange
            var tasks = new List<TaskDto>
            {
                new TaskDto { Id = 1, Title = "Task 1", ProjectId = 1, AssignedToUserId = 2 },
                new TaskDto { Id = 2, Title = "Task 2", ProjectId = 1, AssignedToUserId = 3 }
            };
            _mockTaskService.Setup(s => s.GetTasksByProject(1)).Returns(tasks);

            // Act
            var result = _controller.GetTasksByProject(1) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var returned = Assert.IsAssignableFrom<IEnumerable<TaskDto>>(result.Value);
            Assert.Equal(2, ((List<TaskDto>)returned).Count);
        }

        // ---------------- GET TASK BY ID ----------------
        [Fact]
        public void GetTaskById_ShouldReturnTask_WhenExists()
        {
            // Arrange
            var task = new TaskDto { Id = 1, Title = "Task 1" };
            _mockTaskService.Setup(s => s.GetTaskById(1)).Returns(task);

            // Act
            var result = _controller.GetTaskById(1) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var returned = Assert.IsType<TaskDto>(result.Value);
            Assert.Equal(1, returned.Id);
        }

        [Fact]
        public void GetTaskById_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            _mockTaskService.Setup(s => s.GetTaskById(999)).Returns((TaskDto?)null);

            // Act
            var result = _controller.GetTaskById(999) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }

        // ---------------- CREATE ----------------
        [Fact]
        public void CreateTask_ShouldReturnCreated_WhenValid()
        {
            // Arrange
            var task = new TaskDto { Id = 1, Title = "New Task" };
            _mockTaskService.Setup(s => s.CreateTask(It.IsAny<TaskDto>())).Returns(task);

            // Act
            var result = _controller.CreateTask(new TaskDto()) as CreatedAtActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode);
            var returned = Assert.IsType<TaskDto>(result.Value);
            Assert.Equal("New Task", returned.Title);
        }

        // ---------------- UPDATE ----------------
        [Fact]
        public void UpdateTask_ShouldReturnNoContent_WhenSuccess()
        {
            // Arrange
            _mockTaskService.Setup(s => s.GetTaskById(1)).Returns(new TaskDto { Id = 1, Title = "Task" });
            _mockTaskService.Setup(s => s.UpdateTask(1, It.IsAny<TaskDto>())).Returns(true);

            // Act
            var result = _controller.UpdateTask(1, new TaskDto());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void UpdateTask_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            _mockTaskService.Setup(s => s.GetTaskById(999)).Returns((TaskDto?)null);

            // Act
            var result = _controller.UpdateTask(999, new TaskDto()) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }

        // ---------------- DELETE ----------------
        [Fact]
        public void DeleteTask_ShouldReturnNoContent_WhenDeleted()
        {
            // Arrange
            _mockTaskService.Setup(s => s.DeleteTask(1)).Returns(true);

            // Act
            var result = _controller.DeleteTask(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void DeleteTask_ShouldReturnNotFound_WhenNotExists()
        {
            // Arrange
            _mockTaskService.Setup(s => s.DeleteTask(999)).Returns(false);

            // Act
            var result = _controller.DeleteTask(999) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }
    }
}
