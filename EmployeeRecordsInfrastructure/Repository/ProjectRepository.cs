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
                .ThenInclude(pu => pu.User)   // ✅ include users through join table
            .Include(p => p.Tasks)
            .FirstOrDefault(p => p.Id == id);

        public IEnumerable<Project> GetAll() => _db.Projects
            .Include(p => p.ProjectUsers)
                .ThenInclude(pu => pu.User)   // ✅ include users through join table
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

        // 🔑 Check if a user is part of a project
        public bool IsUserInProject(int projectId, int userId)
        {
            return _db.ProjectUsers
                .Any(pu => pu.ProjectId == projectId && pu.UserId == userId);
        }

        // 🔑 Get all projects for a user
        public IEnumerable<Project> GetProjectsForUser(int userId)
        {
            return _db.Projects
                .Include(p => p.ProjectUsers)
                    .ThenInclude(pu => pu.User)   // ✅ load user details too
                .Include(p => p.Tasks)
                .Where(p => p.ProjectUsers.Any(pu => pu.UserId == userId))
                .ToList();
        }
    }
}

