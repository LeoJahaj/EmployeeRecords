using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeRecordsCore.Models;

namespace EmployeeRecordsCore.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }              // User identifier
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; } // e.g. "Employee" or "Administrator"
        public string Password { get; set; } = string.Empty; // NEW
    }
}

