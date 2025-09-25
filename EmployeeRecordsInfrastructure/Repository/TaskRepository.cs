using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeRecordsCore.Models;
using EmployeeRecordsCore.Repositories;
using EmployeeRecordsInfrastructure.Data;

namespace EmployeeRecordsInfrastructure.Repository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _db;

        public TaskRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public Tasks? GetById(int id) => _db.Tasks.FirstOrDefault(t => t.Id == id);

        public IEnumerable<Tasks> GetByProjectId(int projectId) =>
            _db.Tasks.Where(t => t.ProjectId == projectId).ToList();

        public void Add(Tasks task)
        {
            _db.Tasks.Add(task);
            _db.SaveChanges();
        }

        public void Update(Tasks task)
        {
            _db.Tasks.Update(task);
            _db.SaveChanges();
        }

        public void Delete(Tasks task)
        {
            _db.Tasks.Remove(task);
            _db.SaveChanges();
        }
    }
}

