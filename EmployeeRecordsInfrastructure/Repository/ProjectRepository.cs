using EmployeeRecordsCore.Models;
using EmployeeRecordsCore.Repositories;
using EmployeeRecordsInfrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeRecordsInfrastructure.Repository
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _db;

        public ProjectRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public Project? GetById(int id) => _db.Projects
            .Include(p => p.ProjectUsers)
                .ThenInclude(pu => pu.User)
            .Include(p => p.Tasks)
            .FirstOrDefault(p => p.Id == id);

        public Project GetByIdWithTasks(int id)
        {
            return _db.Projects
                .Include(p => p.Tasks)        // 👈 include tasks
                .Include(p => p.ProjectUsers) // keep existing users too
                .FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Project> GetAll() => _db.Projects
            .Include(p => p.ProjectUsers)
                .ThenInclude(pu => pu.User)
            .Include(p => p.Tasks)
            .ToList();

        public void Add(Project project)
        {
            _db.Projects.Add(project);
            _db.SaveChanges();
        }

        public void Update(Project project)
        {
            _db.Projects.Update(project);
            _db.SaveChanges();
        }

        public void Delete(Project project)
        {
            _db.Projects.Remove(project);
            _db.SaveChanges();
        }

        public bool IsUserInProject(int projectId, int userId)
        {
            return _db.ProjectUsers
                .Any(pu => pu.ProjectId == projectId && pu.UserId == userId);
        }

        public IEnumerable<Project> GetProjectsForUser(int userId)
        {
            return _db.Projects
                .Include(p => p.ProjectUsers)
                    .ThenInclude(pu => pu.User)
                .Include(p => p.Tasks)
                .Where(p => p.ProjectUsers.Any(pu => pu.UserId == userId))
                .ToList();
        }

        // ✅ Add a user to a project
        public bool AddUserToProject(int projectId, int userId)
        {
            var project = _db.Projects
                .Include(p => p.ProjectUsers)
                .FirstOrDefault(p => p.Id == projectId);

            if (project == null) return false;

            if (!project.ProjectUsers.Any(pu => pu.UserId == userId))
            {
                project.ProjectUsers.Add(new ProjectUser
                {
                    ProjectId = projectId,
                    UserId = userId
                });

                _db.SaveChanges();
            }

            return true;
        }

        // ✅ Remove a user from a project
        public bool RemoveUserFromProject(int projectId, int userId)
        {
            var projectUser = _db.ProjectUsers
                .FirstOrDefault(pu => pu.ProjectId == projectId && pu.UserId == userId);

            if (projectUser == null) return false;

            _db.ProjectUsers.Remove(projectUser);
            _db.SaveChanges();
            return true;
        }
    }
}


