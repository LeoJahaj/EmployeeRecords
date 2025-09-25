using EmployeeRecordsCore.DTOs;
using EmployeeRecordsCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeRecordsApi.Controllers
{
    /// <summary>
    /// Manages tasks: create, read, update, delete.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IProjectService _projectService;

        public TaskController(ITaskService taskService, IProjectService projectService)
        {
            _taskService = taskService;
            _projectService = projectService;
        }

        /// <summary>
        /// Get all tasks for a specific project.
        /// </summary>
        [Authorize(Roles = "Administrator,Employee")]
        [HttpGet("project/{projectId}")]
        public IActionResult GetTasksByProject(int projectId)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (userRole == "Employee" && !_projectService.IsUserInProject(projectId, userId))
                return Forbid();

            var tasks = _taskService.GetTasksByProject(projectId);
            return Ok(tasks);
        }

        /// <summary>
        /// Get a task by its ID.
        /// </summary>
        [Authorize(Roles = "Administrator,Employee")]
        [HttpGet("{id}")]
        public IActionResult GetTaskById(int id)
        {
            var task = _taskService.GetTaskById(id);
            if (task == null) return NotFound();

            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (userRole == "Employee" && task.AssignedToUserId != userId)
                return Forbid();

            return Ok(task);
        }

        /// <summary>
        /// Create a new task.
        /// </summary>
        [Authorize(Roles = "Administrator,Employee")]
        [HttpPost]
        public IActionResult CreateTask([FromBody] TaskDto taskDto)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (userRole == "Employee" && !_projectService.IsUserInProject(taskDto.ProjectId, userId))
                return Forbid();

            var created = _taskService.CreateTask(taskDto);
            return CreatedAtAction(nameof(GetTaskById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update an existing task.
        /// </summary>
        [Authorize(Roles = "Administrator,Employee")]
        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, [FromBody] TaskDto taskDto)
        {
            var task = _taskService.GetTaskById(id);
            if (task == null) return NotFound();

            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (userRole == "Employee" && task.AssignedToUserId != userId)
                return Forbid();

            var updated = _taskService.UpdateTask(id, taskDto);
            if (!updated) return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Delete a task by ID.
        /// </summary>
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            var success = _taskService.DeleteTask(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}


