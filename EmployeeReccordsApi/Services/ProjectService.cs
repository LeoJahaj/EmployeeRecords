using EmployeeRecordsCore.DTOs;
using EmployeeRecordsCore.Interfaces;
using EmployeeRecordsCore.Models;
using EmployeeRecordsCore.Repositories;
using Microsoft.EntityFrameworkCore; // 👈 for Include
using System.Linq;

namespace EmployeeRecordsApi.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public IEnumerable<ProjectDto> GetAllProjects()
        {
            return _projectRepository.GetAll().Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                UserIds = p.ProjectUsers.Select(pu => pu.UserId).ToList()
            });
        }

        public ProjectDto? GetProjectById(int id)
        {
            var project = _projectRepository.GetById(id);
            if (project == null) return null;

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                UserIds = project.ProjectUsers.Select(pu => pu.UserId).ToList()
            };
        }

        public ProjectDto CreateProject(ProjectDto projectDto)
        {
            var project = new Project
            {
                Name = projectDto.Name,
                Description = projectDto.Description,
                ProjectUsers = projectDto.UserIds.Select(uid => new ProjectUser
                {
                    UserId = uid
                }).ToList()
            };

            _projectRepository.Add(project);
            projectDto.Id = project.Id;
            return projectDto;
        }

        public bool UpdateProject(int id, ProjectDto projectDto)
        {
            var project = _projectRepository.GetById(id);
            if (project == null) return false;

            project.Name = projectDto.Name;
            project.Description = projectDto.Description;

            // Replace project membership
            project.ProjectUsers = projectDto.UserIds.Select(uid => new ProjectUser
            {
                ProjectId = id,
                UserId = uid
            }).ToList();

            _projectRepository.Update(project);
            return true;
        }

        public bool DeleteProject(int id)
        {
            var project = _projectRepository.GetByIdWithTasks(id);
            if (project == null) return false;

            // 🚫 Block only if unfinished tasks exist
            if (project.Tasks != null && project.Tasks.Any(t => !t.IsCompleted))
            {
                throw new InvalidOperationException("Cannot delete project with unfinished tasks.");
            }

            _projectRepository.Delete(project);
            return true;
        }


        public bool IsUserInProject(int projectId, int userId)
        {
            var project = _projectRepository.GetById(projectId);
            if (project == null) return false;

            return project.ProjectUsers.Any(pu => pu.UserId == userId);
        }

        public IEnumerable<ProjectDto> GetProjectsForUser(int userId)
        {
            var projects = _projectRepository.GetAll()
                .Where(p => p.ProjectUsers.Any(pu => pu.UserId == userId));

            return projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                UserIds = p.ProjectUsers.Select(pu => pu.UserId).ToList()
            });
        }

        public bool AddUserToProject(int projectId, int userId)
        {
            var project = _projectRepository.GetById(projectId);
            if (project == null) return false;

            if (!project.ProjectUsers.Any(pu => pu.UserId == userId))
            {
                project.ProjectUsers.Add(new ProjectUser { ProjectId = projectId, UserId = userId });
                _projectRepository.Update(project);
            }
            return true;
        }

        public bool RemoveUserFromProject(int projectId, int userId)
        {
            var project = _projectRepository.GetById(projectId);
            if (project == null) return false;

            var pu = project.ProjectUsers.FirstOrDefault(pu => pu.UserId == userId);
            if (pu == null) return false;

            project.ProjectUsers.Remove(pu);
            _projectRepository.Update(project);
            return true;
        }
    }
}




