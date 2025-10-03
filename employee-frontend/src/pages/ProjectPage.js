import React, { useEffect, useState } from "react";
import api from "../api/api";
import { jwtDecode } from "jwt-decode";

function ProjectsPage() {
  const [projects, setProjects] = useState([]);
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);

  const [newProject, setNewProject] = useState({ name: "", description: "" });
  const [selectedProjectId, setSelectedProjectId] = useState("");
  const [selectedUserId, setSelectedUserId] = useState("");
  const [message, setMessage] = useState("");

  const token = localStorage.getItem("token");

  const getUserFromToken = () => {
    if (!token) return null;
    try {
      const decoded = jwtDecode(token);
      return {
        userId: Number(decoded.sub),
        role: decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"],
      };
    } catch {
      return null;
    }
  };

  const currentUser = getUserFromToken();

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
    } finally {
      setLoading(false);
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

  const createProject = async () => {
    try {
      await api.post("/project", newProject, {
        headers: { Authorization: `Bearer ${token}` },
      });
      setMessage("Project created!");
      fetchProjects();
    } catch (err) {
      console.error("Error creating project", err);
    }
  };

  const updateProject = async () => {
    if (!selectedProjectId) return alert("Select a project first.");
    try {
      const updatedProject = {
        id: Number(selectedProjectId),
        name: newProject.name,
        description: newProject.description,
        startDate: new Date().toISOString(),
        endDate: null,
      };

      await api.put(`/project/${selectedProjectId}`, updatedProject, {
        headers: { Authorization: `Bearer ${token}` },
      });

      setMessage(`Project ${selectedProjectId} updated!`);
      fetchProjects();
    } catch (err) {
      console.error("Error updating project", err);
    }
  };

  const deleteProject = async () => {
    if (!selectedProjectId) return alert("Select a project first.");
    try {
      await api.delete(`/project/${selectedProjectId}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      setMessage(`Project ${selectedProjectId} deleted!`);
      fetchProjects();
    } catch (err) {
      if (err.response?.data?.message) {
        setMessage(`Error: ${err.response.data.message}`);
      } else {
        setMessage("Error deleting project.");
      }
      console.error("Error deleting project", err);
    }
  };

  const addUserToProject = async () => {
    if (!selectedProjectId || !selectedUserId)
      return alert("Select project and user.");
    try {
      const response = await api.post(
        `/project/${selectedProjectId}/user/${selectedUserId}`,
        {},
        { headers: { Authorization: `Bearer ${token}` } }
      );
      setMessage(
        response.data.message ||
          `User ${selectedUserId} added to project ${selectedProjectId}`
      );
      fetchProjects();
    } catch (err) {
      console.error("Error adding user to project", err);
    }
  };

  const removeUserFromProject = async () => {
    if (!selectedProjectId || !selectedUserId)
      return alert("Select project and user.");
    try {
      const response = await api.delete(
        `/project/${selectedProjectId}/user/${selectedUserId}`,
        { headers: { Authorization: `Bearer ${token}` } }
      );
      setMessage(
        response.data.message ||
          `User ${selectedUserId} removed from project ${selectedProjectId}`
      );
      fetchProjects();
    } catch (err) {
      console.error("Error removing user from project", err);
    }
  };

  if (loading) return <p>Loading projects...</p>;

  return (
    <div className="page-container">
      <h2>Projects</h2>
      <p className="message">{message}</p>

      {currentUser?.role === "Administrator" && (
        <>
          <h3>Select Project</h3>
          <select
            value={selectedProjectId}
            onChange={(e) => setSelectedProjectId(e.target.value)}
          >
            <option value="">-- Select a project --</option>
            {projects.map((p) => (
              <option key={p.id} value={p.id}>
                {p.id} - {p.name}
              </option>
            ))}
          </select>

          <h3>Create / Update Project</h3>
          <input
            placeholder="Name"
            value={newProject.name}
            onChange={(e) =>
              setNewProject({ ...newProject, name: e.target.value })
            }
          />
          <input
            placeholder="Description"
            value={newProject.description}
            onChange={(e) =>
              setNewProject({ ...newProject, description: e.target.value })
            }
          />
          <div className="button-group">
            <button onClick={createProject}>Create</button>
            <button onClick={updateProject}>Update</button>
            <button className="danger" onClick={deleteProject}>
              Delete
            </button>
          </div>

          <h3>Project / User Actions</h3>
          <select
            value={selectedUserId}
            onChange={(e) => setSelectedUserId(e.target.value)}
          >
            <option value="">-- Select user --</option>
            {users.map((u) => (
              <option key={u.id} value={u.id}>
                {u.id} - {u.userName}
              </option>
            ))}
          </select>
          <div className="button-group">
            <button onClick={addUserToProject}>Add User</button>
            <button className="danger" onClick={removeUserFromProject}>
              Remove User
            </button>
          </div>
        </>
      )}

      <h3>All Projects</h3>
      <ul className="list">
        {projects.map((p) => (
          <li key={p.id}>
            <strong>{p.name}</strong> - {p.description} (Users:{" "}
            {p.userIds?.join(", ") || "None"})
          </li>
        ))}
      </ul>
    </div>
  );
}

export default ProjectsPage;
