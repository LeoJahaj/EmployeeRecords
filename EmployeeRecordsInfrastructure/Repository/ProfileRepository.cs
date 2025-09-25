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
    public class ProfileRepository : IProfileRepository
    {
        private readonly ApplicationDbContext _db;

        public ProfileRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public Profile? GetByUserId(int userId) =>
            _db.Profiles.FirstOrDefault(p => p.UserId == userId);

        public void Add(Profile profile)
        {
            _db.Profiles.Add(profile);
            _db.SaveChanges();
        }

        public void Update(Profile profile)
        {
            _db.Profiles.Update(profile);
            _db.SaveChanges();
        }
    }
}

