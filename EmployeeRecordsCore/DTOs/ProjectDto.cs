using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeRecordsCore.DTOs
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<int> UserIds { get; set; } = new List<int>();
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }  // nullable if project is ongoing
    }
}

