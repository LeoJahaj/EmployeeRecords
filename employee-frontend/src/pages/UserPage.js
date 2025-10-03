// src/pages/UsersPage.js
import React, { useState, useEffect } from "react";
import api from "../api/api";
import { jwtDecode } from "jwt-decode";

function UsersPage() {
  const [users, setUsers] = useState([]);
  const [userId, setUserId] = useState("");
  const [user, setUser] = useState(null);
  const [projects, setProjects] = useState([]);
  const [newUser, setNewUser] = useState({
    userName: "",
    email: "",
    role: "Employee",
    password: "",
  });

  const token = localStorage.getItem("token");

  // 🔹 Decode current user role
  const getUserFromToken = () => {
    if (!token) return null;
    try {
      const decoded = jwtDecode(token);
      return {
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

  // ✅ Load all users on mount
  useEffect(() => {
    fetchAllUsers();
  }, []);

  const fetchAllUsers = async () => {
    try {
      const response = await api.get("/user", {
        headers: { Authorization: `Bearer ${token}` },
      });
      setUsers(response.data);
    } catch (err) {
      console.error("Error fetching users", err);
    }
  };

  const fetchUserById = async () => {
    if (!userId) return alert("Select a user first.");
    try {
      const response = await api.get(`/user/${userId}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      setUser(response.data);
    } catch {
      alert("Error fetching user");
    }
  };

  const fetchProjectsForUser = async () => {
    if (!userId) return alert("Select a user first.");
    try {
      const response = await api.get(`/user/${userId}/projects`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      setProjects(response.data);
    } catch {
      alert("Error fetching projects for user");
    }
  };

  const createUser = async () => {
    try {
      await api.post("/user", newUser, {
        headers: { Authorization: `Bearer ${token}` },
      });
      alert("User created successfully!");
      fetchAllUsers();
    } catch {
      alert("Error creating user");
    }
  };

  const deleteUser = async () => {
    if (!userId) return alert("Select a user first.");
    try {
      await api.delete(`/user/${userId}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      alert("User deleted successfully!");
      setUser(null);
      setProjects([]);
      fetchAllUsers();
    } catch {
      alert("Error deleting user");
    }
  };

  return (
    <div className="page-container">
      <h2>Users</h2>

      {/* 🔹 Select User */}
      <h3>Select User</h3>
      <select value={userId} onChange={(e) => setUserId(e.target.value)}>
        <option value="">-- Select a user --</option>
        {users.map((u) => (
          <option key={u.id} value={u.id}>
            {u.id} - {u.userName}
          </option>
        ))}
      </select>
      <div className="button-group">
        <button onClick={fetchUserById}>Get User</button>
        <button onClick={fetchProjectsForUser}>Get Projects</button>
        {currentUser?.role === "Administrator" && (
          <button className="danger" onClick={deleteUser}>
            Delete User
          </button>
        )}
      </div>

      {/* 🔹 User info */}
      {user && (
        <div>
          <h3>User Info</h3>
          <p><b>ID:</b> {user.id}</p>
          <p><b>Username:</b> {user.userName}</p>
          <p><b>Email:</b> {user.email}</p>
          <p><b>Role:</b> {user.role}</p>
        </div>
      )}

      {/* 🔹 Projects */}
      {projects.length > 0 && (
        <div>
          <h3>User Projects</h3>
          <ul className="list">
            {projects.map((p) => (
              <li key={p.id}>
                <strong>{p.name}</strong> - {p.description}
              </li>
            ))}
          </ul>
        </div>
      )}

      {/* 🔹 Create form (Admins only) */}
      {currentUser?.role === "Administrator" && (
        <div>
          <h3>Create New User</h3>
          <input
            type="text"
            placeholder="Username"
            value={newUser.userName}
            onChange={(e) =>
              setNewUser({ ...newUser, userName: e.target.value })
            }
          />
          <input
            type="email"
            placeholder="Email"
            value={newUser.email}
            onChange={(e) => setNewUser({ ...newUser, email: e.target.value })}
          />
          <input
            type="text"
            placeholder="Role (Administrator/Employee)"
            value={newUser.role}
            onChange={(e) => setNewUser({ ...newUser, role: e.target.value })}
          />
          <input
            type="password"
            placeholder="Password"
            value={newUser.password}
            onChange={(e) =>
              setNewUser({ ...newUser, password: e.target.value })
            }
          />
          <button onClick={createUser}>Create User</button>
        </div>
      )}
    </div>
  );
}

export default UsersPage;
