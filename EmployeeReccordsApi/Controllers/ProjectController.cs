using EmployeeRecordsCore.DTOs;
using EmployeeRecordsCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeRecordsApi.Controllers
{
    /// <summary>
    /// Manages projects: create, read, update, delete, and user assignments.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        /// <summary>
        /// Get all projects.
        /// - Admins see all projects.  
        /// - Employees see only their assigned projects.
        /// </summary>
        [Authorize]
        [HttpGet]
        public IActionResult GetAllProjects()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int currentUserId = userIdClaim != null ? int.Parse(userIdClaim) : 0;
            bool isAdmin = User.IsInRole("Administrator");

            var projects = _projectService.GetAllProjects();

            if (!isAdmin)
                projects = projects.Where(p => p.UserIds.Contains(currentUserId));

            return Ok(projects);
        }

        /// <summary>
        /// Get a project by ID.
        /// - Admins: any project.  
        /// - Employees: only their own projects.
        /// </summary>
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetProjectById(int id)
        {
            var project = _projectService.GetProjectById(id);
            if (project == null)
                return NotFound($"Project with ID {id} not found.");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int currentUserId = userIdClaim != null ? int.Parse(userIdClaim) : 0;
            bool isAdmin = User.IsInRole("Administrator");

            if (!isAdmin && !project.UserIds.Contains(currentUserId))
                return Forbid();

            return Ok(project);
        }

        /// <summary>
        /// Create a new project (Admins only).
        /// </summary>
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public IActionResult CreateProject([FromBody] ProjectDto projectDto)
        {
            if (projectDto == null)
                return BadRequest("Project data is required.");

            var created = _projectService.CreateProject(projectDto);
            return CreatedAtAction(nameof(GetProjectById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update a project (Admins only).
        /// </summary>
        [Authorize(Roles = "Administrator")]
        [HttpPut("{id}")]
        public IActionResult UpdateProject(int id, [FromBody] ProjectDto projectDto)
        {
            if (projectDto == null)
                return BadRequest("Project data is required.");

            var updated = _projectService.UpdateProject(id, projectDto);
            if (!updated)
                return NotFound($"Project with ID {id} not found.");
            return NoContent();
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public IActionResult DeleteProject(int id)
        {
            try
            {
                var success = _projectService.DeleteProject(id);
                if (!success)
                    return NotFound($"Project with ID {id} not found.");
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        /// <summary>
        /// Check if a user belongs to a project (Admins only).
        /// </summary>
        [Authorize(Roles = "Administrator")]
        [HttpGet("{projectId}/user/{userId}/check")]
        public IActionResult IsUserInProject(int projectId, int userId)
        {
            var result = _projectService.IsUserInProject(projectId, userId);
            return Ok(new { UserId = userId, ProjectId = projectId, IsInProject = result });
        }

        /// <summary>
        /// Get all projects for a user.  
        /// - Admins: any user.  
        /// - Employees: only themselves.
        /// </summary>
        [Authorize]
        [HttpGet("user/{userId}")]
        public IActionResult GetProjectsForUser(int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int currentUserId = userIdClaim != null ? int.Parse(userIdClaim) : 0;
            bool isAdmin = User.IsInRole("Administrator");

            if (!isAdmin && currentUserId != userId)
                return Forbid();

            var projects = _projectService.GetProjectsForUser(userId);
            return Ok(projects);
        }

        // ✅ NEW ENDPOINTS

        /// <summary>
        /// Add a user to a project (Admins only).
        /// </summary>
        [Authorize(Roles = "Administrator")]
        [HttpPost("{projectId}/user/{userId}")]
        public IActionResult AddUserToProject(int projectId, int userId)
        {
            var success = _projectService.AddUserToProject(projectId, userId);
            if (!success)
                return NotFound($"Project {projectId} or User {userId} not found.");
            return Ok(new { Message = $"User {userId} added to Project {projectId}" });
        }

        /// <summary>
        /// Remove a user from a project (Admins only).
        /// </summary>
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{projectId}/user/{userId}")]
        public IActionResult RemoveUserFromProject(int projectId, int userId)
        {
            var success = _projectService.RemoveUserFromProject(projectId, userId);
            if (!success)
                return NotFound($"User {userId} is not part of Project {projectId}.");
            return Ok(new { Message = $"User {userId} removed from Project {projectId}" });
        }
    }
}







