using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeRecordsCore.Models;

namespace EmployeeRecordsCore.Repositories
{
    public interface ITaskRepository
    {
        Tasks? GetById(int id);
        IEnumerable<Tasks> GetByProjectId(int projectId);
        void Add(Tasks task);
        void Update(Tasks task);
        void Delete(Tasks task);
    }
}
