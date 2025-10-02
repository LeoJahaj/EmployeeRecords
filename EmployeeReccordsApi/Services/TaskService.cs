using EmployeeRecordsCore.DTOs;
using EmployeeRecordsCore.Interfaces;
using EmployeeRecordsCore.Models;
using EmployeeRecordsCore.Repositories;

namespace EmployeeRecordsApi.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public IEnumerable<TaskDto> GetTasksByProject(int projectId)
        {
            var tasks = _taskRepository.GetByProjectId(projectId);

            return tasks.Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsCompleted = t.IsCompleted,
                ProjectId = t.ProjectId,
                AssignedToUserId = t.AssignedToUserId
            });
        }

        public TaskDto? GetTaskById(int id)
        {
            Tasks task = _taskRepository.GetById(id);
            Console.WriteLine("the task"+task.Title);

            if (task == null) return null;

            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                ProjectId = task.ProjectId,
                AssignedToUserId = task.AssignedToUserId
            };
        }

        public TaskDto CreateTask(TaskDto taskDto)
        {
            var task = new Tasks
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                IsCompleted = taskDto.IsCompleted,
                ProjectId = taskDto.ProjectId,
                AssignedToUserId = taskDto.AssignedToUserId
            };

            _taskRepository.Add(task);

            // Update DTO with the generated Id
            taskDto.Id = task.Id;
            return taskDto;
        }

        public bool UpdateTask(int id, TaskDto taskDto)
        {
            var task = _taskRepository.GetById(id);
            if (task == null) return false;

            task.Title = taskDto.Title;
            task.Description = taskDto.Description;
            task.IsCompleted = taskDto.IsCompleted;
            task.ProjectId = taskDto.ProjectId;
            task.AssignedToUserId = taskDto.AssignedToUserId;

            _taskRepository.Update(task);
            return true;
        }

        public bool DeleteTask(int id)
        {
            var task = _taskRepository.GetById(id);
            if (task == null) return false;

            _taskRepository.Delete(task);
            return true;
        }
    }
}


