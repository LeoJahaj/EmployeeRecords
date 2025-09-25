using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeRecordsCore.Models
{
    public class Tasks
    {
        public int Id { get; set; }                   // Primary Key
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }

        // Foreign Keys
        public int ProjectId { get; set; }
        public int AssignedToUserId { get; set; }

        // Relationships
        public Project? Project { get; set; }
        public User? AssignedToUser { get; set; }
    }
}

