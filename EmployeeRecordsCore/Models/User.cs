using EmployeeRecordsCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EmployeeRecordsCore.Models
{
    public class User
    {
        public int Id { get; set; }                         // Primary Key
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // stored securely
        public UserRole Role { get; set; }         // "Employee" or "Administrator"

        // Relationships
        public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>(); // Many-to-many
        public ICollection<Tasks> Tasks { get; set; } = new List<Tasks>();           // One-to-many
        public Profile? Profile { get; set; }                                      // One-to-one
    }
}
