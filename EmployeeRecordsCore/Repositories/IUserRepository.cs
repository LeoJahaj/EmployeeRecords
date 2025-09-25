using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeRecordsCore.Models;

namespace EmployeeRecordsCore.Repositories
{
    public interface IUserRepository
    {
        User? GetById(int id);
        IEnumerable<User> GetAll();
        void Add(User user);
        void Update(User user);
        void Delete(User user);

        // ✅ New helper
        IEnumerable<Project> GetProjectsForUser(int userId);
    }
}

