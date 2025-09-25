using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeRecordsCore.Models
{
    public class Project
    {
        public int Id { get; set; }                     // Primary Key
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Relationships
        public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>(); // Many-to-many
        public ICollection<Tasks> Tasks { get; set; } = new List<Tasks>(); // One-to-many
    }
}

