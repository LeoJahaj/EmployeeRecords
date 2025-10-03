// src/pages/TaskPage.js
import React, { useEffect, useState } from "react";
import api from "../api/api";
import { jwtDecode } from "jwt-decode";

function TaskPage() {
  const [projects, setProjects] = useState([]);
  const [tasks, setTasks] = useState([]);
  const [users, setUsers] = useState([]);

  const [selectedProjectId, setSelectedProjectId] = useState("");
  const [selectedTaskId, setSelectedTaskId] = useState("");

  const [newTask, setNewTask] = useState({
    title: "",
    description: "",
    assignedToUserId: "",
  });

  const [updateTask, setUpdateTask] = useState({
    title: "",
    description: "",
    isCompleted: false,
    assignedToUserId: "",
  });

  const token = localStorage.getItem("token");

  // 🔹 Decode current user info
  const getUserFromToken = () => {
    if (!token) return null;
    try {
      const decoded = jwtDecode(token);
      return {
        userId: Number(decoded.sub),
        role:
          decoded[
            "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
          ],
      };
    } catch {
      return null;
    }
  };

  const currentUser = getUserFromToken();

  // 🔹 Load projects & users on mount
  useEffect(() => {
    fetchProjects();
    fetchUsers();
  }, []);

  const fetchProjects = async () => {
    try {
      const response = await api.get("/project", {
        headers: { Authorization: `Bearer ${token}` },
      });
      setProjects(response.data);
    } catch (err) {
      console.error("Error fetching projects", err);
    }
  };

  const fetchUsers = async () => {
    try {
      const response = await api.get("/user", {
        headers: { Authorization: `Bearer ${token}` },
      });
      setUsers(response.data);
    } catch (err) {
      console.error("Error fetching users", err);
    }
  };

  // 🔹 Load tasks for a project
  const fetchTasksByProject = async (projectId) => {
    try {
      const response = await api.get(`/task/project/${projectId}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      setTasks(response.data);
    } catch (err) {
      console.error("Error fetching tasks", err);
    }
  };

  // 🔹 Create task
  const createTask = async () => {
    if (!selectedProjectId) return alert("Select a project first.");

    try {
      const payload = {
        title: newTask.title,
        description: newTask.description,
        projectId: Number(selectedProjectId),
        assignedToUserId: Number(newTask.assignedToUserId),
        isCompleted: false,
      };
      const response = await api.post("/task", payload, {
        headers: { Authorization: `Bearer ${token}` },
      });
      alert("✅ Task created successfully!");
      setTasks([...tasks, response.data]);
    } catch (err) {
      if (err.response?.status === 403 || err.response?.status === 401) {
        alert("❌ Unauthorized: You cannot create tasks in this project.");
      } else {
        console.error("Error creating task", err);
      }
    }
  };

  // 🔹 Update task
  const updateTaskById = async () => {
    if (!selectedTaskId) return alert("Select a task first.");
    const task = tasks.find((t) => t.id === Number(selectedTaskId));
    if (!task) return alert("Task not found.");

    try {
      let payload = {
        title: updateTask.title || task.title,
        description: updateTask.description || task.description,
        isCompleted: updateTask.isCompleted,
        projectId: Number(selectedProjectId),
      };

      // 🔹 Admins can reassign, employees keep the same assignee
      if (currentUser?.role === "Administrator") {
        payload.assignedToUserId =
          Number(updateTask.assignedToUserId) || task.assignedToUserId;
      } else {
        payload.assignedToUserId = task.assignedToUserId;
      }

      await api.put(`/task/${selectedTaskId}`, payload, {
        headers: { Authorization: `Bearer ${token}` },
      });

      alert(`✅ Task ${selectedTaskId} updated successfully!`);
      fetchTasksByProject(selectedProjectId);
    } catch (err) {
      if (err.response?.status === 403 || err.response?.status === 401) {
        alert("❌ Unauthorized: You cannot update this task.");
      } else {
        console.error("Error updating task", err);
      }
    }
  };

  // 🔹 Delete task
  const deleteTask = async () => {
    if (!selectedTaskId) return alert("Select a task first.");
    try {
      await api.delete(`/task/${selectedTaskId}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      alert(`🗑️ Task ${selectedTaskId} deleted successfully!`);
      setTasks(tasks.filter((t) => t.id !== Number(selectedTaskId)));
      setSelectedTaskId("");
    } catch (err) {
      console.error("Error deleting task", err);
    }
  };

  // 🔹 Filter users based on selected project
  const usersInSelectedProject = selectedProjectId
    ? users.filter((u) =>
        projects
          .find((p) => p.id === Number(selectedProjectId))
          ?.userIds.includes(u.id)
      )
    : [];

  // 🔹 Employees: only see their own tasks in dropdown
  const tasksForDropdown =
    currentUser?.role === "Administrator"
      ? tasks
      : tasks.filter((t) => t.assignedToUserId === currentUser?.userId);

  return (
    <div className="page-container">
      <h2>📌 Tasks</h2>

      {/* 🔹 Project selection */}
      <div className="form-group">
        <h3>Select Project</h3>
        <select
          value={selectedProjectId}
          onChange={(e) => {
            setSelectedProjectId(e.target.value);
            setSelectedTaskId("");
            if (e.target.value) fetchTasksByProject(e.target.value);
          }}
        >
          <option value="">-- Select project --</option>
          {projects.map((p) => (
            <option key={p.id} value={p.id}>
              {p.id} - {p.name}
            </option>
          ))}
        </select>
      </div>

      {/* 🔹 Task selection */}
      {tasksForDropdown.length > 0 && (
        <div className="form-group">
          <h3>Select Task</h3>
          <select
            value={selectedTaskId}
            onChange={(e) => setSelectedTaskId(e.target.value)}
          >
            <option value="">-- Select task --</option>
            {tasksForDropdown.map((t) => (
              <option key={t.id} value={t.id}>
                {t.id} - {t.title}
              </option>
            ))}
          </select>
        </div>
      )}

      {/* 🔹 Create new task */}
      <div className="form-group task-form">
        <h3>Create Task</h3>
        <input
          placeholder="Title"
          value={newTask.title}
          onChange={(e) => setNewTask({ ...newTask, title: e.target.value })}
        />
        <input
          placeholder="Description"
          value={newTask.description}
          onChange={(e) =>
            setNewTask({ ...newTask, description: e.target.value })
          }
        />
        <select
          value={newTask.assignedToUserId}
          onChange={(e) =>
            setNewTask({ ...newTask, assignedToUserId: e.target.value })
          }
        >
          <option value="">-- Assign to user --</option>
          {usersInSelectedProject.map((u) => (
            <option key={u.id} value={u.id}>
              {u.id} - {u.userName}
            </option>
          ))}
        </select>
        <button onClick={createTask}>Create Task</button>
      </div>

      {/* 🔹 Update existing task */}
      <div className="form-group task-form">
        <h3>Update Task</h3>
        <input
          placeholder="New Title"
          value={updateTask.title}
          onChange={(e) =>
            setUpdateTask({ ...updateTask, title: e.target.value })
          }
        />
        <input
          placeholder="New Description"
          value={updateTask.description}
          onChange={(e) =>
            setUpdateTask({ ...updateTask, description: e.target.value })
          }
        />

        {/* ✅ Completed row */}
        <div className="completed-row">
          <label>Completed?</label>
          <input
            type="checkbox"
            checked={updateTask.isCompleted}
            onChange={(e) =>
              setUpdateTask({ ...updateTask, isCompleted: e.target.checked })
            }
          />
        </div>

        {/* 🔹 Hide reassign dropdown if employee */}
        {currentUser?.role === "Administrator" && (
          <select
            value={updateTask.assignedToUserId}
            onChange={(e) =>
              setUpdateTask({
                ...updateTask,
                assignedToUserId: e.target.value,
              })
            }
          >
            <option value="">-- Reassign to user --</option>
            {usersInSelectedProject.map((u) => (
              <option key={u.id} value={u.id}>
                {u.id} - {u.userName}
              </option>
            ))}
          </select>
        )}

        <button onClick={updateTaskById}>Update Task</button>
        <button className="danger" onClick={deleteTask}>
          Delete Task
        </button>
      </div>

      {/* 🔹 Task list */}
      <h3>All Tasks in Project</h3>
      <ul className="task-list">
        {tasks.map((t) => (
          <li key={t.id}>
            {t.id}: <strong>{t.title}</strong> - {t.description} → User{" "}
            {t.assignedToUserId} ({t.isCompleted ? "✅ Done" : "⏳ Pending"})
          </li>
        ))}
      </ul>
    </div>
  );
}

export default TaskPage;


