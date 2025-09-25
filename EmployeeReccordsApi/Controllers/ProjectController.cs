using EmployeeRecordsCore.DTOs;
using EmployeeRecordsCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeRecordsApi.Controllers
{
    /// <summary>
    /// Manages projects: create, read, update, and delete.
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
        /// - Employees see only the projects they belong to.
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
            {
                // Filter projects so employees only see the ones they belong to
                projects = projects.Where(p => p.UserIds.Contains(currentUserId));
            }

            return Ok(projects);
        }

        /// <summary>
        /// Get a project by ID.
        /// - Admins can fetch any project.  
        /// - Employees can only fetch projects they belong to.
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
        /// Update an existing project (Admins only).
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

        /// <summary>
        /// Delete a project by ID (Admins only).
        /// </summary>
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public IActionResult DeleteProject(int id)
        {
            var success = _projectService.DeleteProject(id);
            if (!success)
                return NotFound($"Project with ID {id} not found or has open tasks.");
            return NoContent();
        }




        /// <summary>
        /// Check if a user belongs to a specific project.
        /// Admin only.
        /// </summary>
        /// <param name="projectId">The project ID.</param>
        /// <param name="userId">The user ID.</param>
        /// <returns>
        /// 200 OK with true/false.  
        /// 403 Forbidden if not an Administrator.
        /// </returns>
        // GET: api/project/{projectId}/user/{userId}/check
        [Authorize(Roles = "Administrator")]
        [HttpGet("{projectId}/user/{userId}/check")]
        public IActionResult IsUserInProject(int projectId, int userId)
        {
            var result = _projectService.IsUserInProject(projectId, userId);
            return Ok(new { UserId = userId, ProjectId = projectId, IsInProject = result });
        }

        /// <summary>
        /// Get all projects for a specific user.
        /// - Admins can get projects for any user.  
        /// - Employees can only get their own projects.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>
        /// 200 OK with a list of projects.  
        /// 403 Forbidden if an employee tries to fetch projects for another user.
        /// </returns>
        // GET: api/project/user/{userId}
        [Authorize]
        [HttpGet("user/{userId}")]
        public IActionResult GetProjectsForUser(int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int currentUserId = userIdClaim != null ? int.Parse(userIdClaim) : 0;
            bool isAdmin = User.IsInRole("Administrator");

            // ⛔ Block employees from requesting another user's projects
            if (!isAdmin && currentUserId != userId)
                return Forbid();

            var projects = _projectService.GetProjectsForUser(userId);
            return Ok(projects);
        }

    }
}




