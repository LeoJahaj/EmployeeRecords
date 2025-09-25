using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeRecordsCore.DTOs;

namespace EmployeeRecordsCore.Interfaces
{
    public interface IUserService
    {
        string? Login(LoginDto loginDto);
        UserDto CreateUser(UserDto userDto);
        UserDto? GetUserById(int id);
        bool DeleteUser(int id);
        IEnumerable<ProjectDto> GetProjectsForUser(int userId);
    }
}

