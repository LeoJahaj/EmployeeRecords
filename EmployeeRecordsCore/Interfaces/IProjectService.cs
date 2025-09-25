using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeRecordsCore.DTOs;

namespace EmployeeRecordsCore.Interfaces
{
    public interface IProjectService
    {
        IEnumerable<ProjectDto> GetAllProjects();
        ProjectDto? GetProjectById(int id);
        ProjectDto CreateProject(ProjectDto projectDto);
        bool UpdateProject(int id, ProjectDto projectDto);
        bool DeleteProject(int id);
        bool IsUserInProject(int projectId, int userId);
        IEnumerable<ProjectDto> GetProjectsForUser(int userId);
    }
}

