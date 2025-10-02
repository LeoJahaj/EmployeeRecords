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
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }

        // Store role as string ("Administrator" / "Employee") for requests/responses
        public string Role { get; set; } = string.Empty;

        public string? Password { get; set; } // used only when creating/updating
    }
}

