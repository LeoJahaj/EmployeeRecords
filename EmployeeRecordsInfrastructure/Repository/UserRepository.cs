using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeRecordsCore.Models;
using EmployeeRecordsCore.Repositories;
using EmployeeRecordsInfrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeRecordsInfrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;

        public UserRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public User? GetById(int id) => _db.Users
            .Include(u => u.Profile)
            .Include(u => u.ProjectUsers)           // ✅ load join table
                .ThenInclude(pu => pu.Project)      // ✅ load related projects
            .FirstOrDefault(u => u.Id == id);

        public IEnumerable<User> GetAll() => _db.Users
            .Include(u => u.Profile)
            .Include(u => u.ProjectUsers)           // ✅ load join table
                .ThenInclude(pu => pu.Project)      // ✅ load related projects
            .ToList();

        // ✅ Get all projects for a user
        public IEnumerable<Project> GetProjectsForUser(int userId)
        {
            return _db.ProjectUsers
                .Include(pu => pu.Project)
                .Where(pu => pu.UserId == userId)
                .Select(pu => pu.Project)
                .ToList();
        }

        public void Add(User user)
        {
            _db.Users.Add(user);
            _db.SaveChanges();
        }

        public void Update(User user)
        {
            _db.Users.Update(user);
            _db.SaveChanges();
        }

        public void Delete(User user)
        {
            _db.Users.Remove(user);
            _db.SaveChanges();
        }
    }
}

