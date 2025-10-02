using EmployeeRecordsCore.DTOs;
using EmployeeRecordsCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeRecordsApi.Controllers
{
    /// <summary>
    /// Manages user accounts and authentication.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Authenticate a user and return a token.
        /// </summary>
        /// <param name="loginDto">Login credentials (username and password).</param>
        /// <returns>
        /// 200 OK with a token if login is successful.  
        /// 400 Bad Request if credentials are missing.  
        /// 401 Unauthorized if login fails.
        /// </returns>
        // POST: api/user/login
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.UserName) || string.IsNullOrEmpty(loginDto.Password))
                return BadRequest("Username and password are required.");

            var token = _userService.Login(loginDto);

            if (token == null)
                return Unauthorized("Invalid username or password.");

            return Ok(new { Token = token });
        }



        /// <summary>
        /// Get all projects for a specific user.
        /// </summary>
        [Authorize(Roles = "Administrator,Employee")]
        [HttpGet("{id}/projects")]
        public IActionResult GetProjectsForUser(int id)
        {
            var projects = _userService.GetProjectsForUser(id);
            return Ok(projects);
        }

        // get all users 
        // GET : ./api/user
        [Authorize(Roles = "Administrator,Employee")]
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }





        /// <summary>
        /// Get a user by ID.  
        /// - Admins can fetch any user.  
        /// - Employees can only fetch themselves.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <returns>
        /// 200 OK with user data if authorized and found.  
        /// 403 Forbidden if an employee tries to fetch someone else.  
        /// 404 Not Found if the user does not exist.
        /// </returns>
        // GET: api/user/{id}
        [Authorize(Roles = "Administrator,Employee")]
        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _userService.GetUserById(id);

            if (user == null)
                return NotFound($"User with ID {id} not found.");

            return Ok(user);
        }

        /// <summary>
        /// Create a new user (Admins only).
        /// </summary>
        /// <param name="userDto">User data transfer object.</param>
        /// <returns>
        /// 201 Created with the new user.  
        /// 400 Bad Request if user data is missing.  
        /// 403 Forbidden if not an Administrator.
        /// </returns>
        // POST: api/user
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public IActionResult CreateUser([FromBody] UserDto userDto)
        {
            if (userDto == null)
                return BadRequest("User data is required.");

            var createdUser = _userService.CreateUser(userDto);

            return CreatedAtAction(
                nameof(GetUserById),
                new { id = createdUser.Id },
                createdUser
            );
        }

        /// <summary>
        /// Delete a user by ID (Admins only).
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <returns>
        /// 204 No Content if deleted.  
        /// 404 Not Found if the user does not exist.  
        /// 403 Forbidden if not an Administrator.
        /// </returns>
        // DELETE: api/user/{id}
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var success = _userService.DeleteUser(id);

            if (!success)
                return NotFound($"User with ID {id} not found.");

            return NoContent();
        }
    }
}




