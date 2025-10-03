// src/App.js
import React, { useState } from "react";
import { BrowserRouter as Router, Routes, Route, Navigate, Link } from "react-router-dom";
import api from "./api/api";
import ProjectsPage from "./pages/ProjectPage";
import TaskPage from "./pages/TaskPage";
import UsersPage from "./pages/UserPage";
import ProfilePage from "./pages/ProfilePage";
import Home from "./pages/Home";   // ✅ use the new Home.js
import "./App.css";

// 🔹 Header Component (Visible after login)
function Header({ onLogout }) {
  const token = localStorage.getItem("token");

  if (!token) return null; // Hide header if not logged in

  return (
    <header className="header">
      <nav>
        <Link to="/home">Home</Link> |{" "}
        <Link to="/projects">Projects</Link> |{" "}
        <Link to="/tasks">Tasks</Link> |{" "}
        <Link to="/users">Users</Link> |{" "}
        <Link to="/profile">Profile</Link> |{" "}
        <button onClick={onLogout}>Logout</button>
      </nav>
    </header>
  );
}

// 🔹 Login Page
function LoginPage({ onLogin }) {
  const [credentials, setCredentials] = useState({ userName: "", password: "" });
  const [error, setError] = useState("");

  const handleLogin = async (e) => {
    e.preventDefault();
    try {
      const response = await api.post("/user/login", credentials);
      localStorage.setItem("token", response.data.token);
      onLogin(); // Successful login
    } catch (err) {
      console.error("Login failed", err);
      setError("Invalid username or password");
    }
  };

  return (
    <div className="login-container">
      <h2>Login</h2>
      <form onSubmit={handleLogin}>
        <input
          type="text"
          placeholder="Username"
          value={credentials.userName}
          onChange={(e) => setCredentials({ ...credentials, userName: e.target.value })}
        />
        <input
          type="password"
          placeholder="Password"
          value={credentials.password}
          onChange={(e) => setCredentials({ ...credentials, password: e.target.value })}
        />
        <button type="submit">Login</button>
      </form>
      {error && <p style={{ color: "red" }}>{error}</p>}
    </div>
  );
}

// 🔹 Protected Route Wrapper
function PrivateRoute({ children }) {
  const token = localStorage.getItem("token");
  return token ? children : <Navigate to="/login" />;
}

// 🔹 Main App Component
function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(!!localStorage.getItem("token"));

  const handleLogin = () => setIsAuthenticated(true);
  const handleLogout = () => {
    localStorage.removeItem("token");
    setIsAuthenticated(false);
  };

  return (
    <Router>
      <div className="App">
        <Header onLogout={handleLogout} />
        <main className="container">
          <Routes>
            {/* Login route */}
            <Route
              path="/login"
              element={isAuthenticated ? <Navigate to="/home" /> : <LoginPage onLogin={handleLogin} />}
            />

            {/* Protected routes */}
            <Route
              path="/home"
              element={
                <PrivateRoute>
                  <Home />  {/* ✅ now uses your Home.js */}
                </PrivateRoute>
              }
            />
            <Route
              path="/projects"
              element={
                <PrivateRoute>
                  <ProjectsPage />
                </PrivateRoute>
              }
            />
            <Route
              path="/tasks"
              element={
                <PrivateRoute>
                  <TaskPage />
                </PrivateRoute>
              }
            />
            <Route
              path="/users"
              element={
                <PrivateRoute>
                  <UsersPage />
                </PrivateRoute>
              }
            />
            <Route
              path="/profile"
              element={
                <PrivateRoute>
                  <ProfilePage />
                </PrivateRoute>
              }
            />

            {/* Default route */}
            <Route path="/" element={isAuthenticated ? <Navigate to="/home" /> : <Navigate to="/login" />} />
          </Routes>
        </main>
      </div>
    </Router>
  );
}

export default App;










