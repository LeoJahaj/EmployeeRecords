using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeRecordsCore.DTOs;

namespace EmployeeRecordsCore.Interfaces
{
    public interface ITaskService
    {
        IEnumerable<TaskDto> GetTasksByProject(int projectId);
        TaskDto? GetTaskById(int id);
        TaskDto CreateTask(TaskDto taskDto);
        bool UpdateTask(int id, TaskDto taskDto);
        bool DeleteTask(int id);
    }
}

