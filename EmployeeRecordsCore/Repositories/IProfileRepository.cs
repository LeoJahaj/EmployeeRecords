using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeRecordsCore.Models;

namespace EmployeeRecordsCore.Repositories
{
    public interface IProfileRepository
    {
        Profile? GetByUserId(int userId);
        void Add(Profile profile);
        void Update(Profile profile);
    }
}
