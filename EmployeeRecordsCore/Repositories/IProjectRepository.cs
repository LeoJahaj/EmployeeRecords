using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeRecordsCore.Models;

namespace EmployeeRecordsCore.Repositories
{
    public interface IProjectRepository
    {
        Project? GetById(int id);
        IEnumerable<Project> GetAll();
        void Add(Project project);
        void Update(Project project);
        void Delete(Project project);
        bool IsUserInProject(int projectId, int userId);
        IEnumerable<Project> GetProjectsForUser(int userId);
        Project GetByIdWithTasks(int id);
        bool AddUserToProject(int projectId, int userId);
        bool RemoveUserFromProject(int projectId, int userId);
    }
}

