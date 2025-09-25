using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeRecordsCore.DTOs;

namespace EmployeeRecordsCore.Interfaces
{
    public interface IProfileService
    {
        ProfileDto? GetProfileByUserId(int userId);
        bool UpdateProfile(int userId, ProfileDto profileDto);
    }
}

