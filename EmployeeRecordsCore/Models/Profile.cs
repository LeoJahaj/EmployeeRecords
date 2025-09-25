using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeRecordsCore.Models
{
    public class Profile
    {
        public int Id { get; set; }                // Primary Key
        public int UserId { get; set; }            // FK → User
        public string FullName { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;

        // Relationship
        public User? User { get; set; }
    }
}

