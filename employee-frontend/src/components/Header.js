// src/components/Header.js
import React from "react";
import { Link } from "react-router-dom";

function Header() {
  return (
    <header style={{ padding: "10px", background: "#eee" }}>
      <nav>
        <Link to="/login">Login</Link> |{" "}
        <Link to="/projects">Projects</Link> |{" "}
        <Link to="/tasks">Tasks</Link> |{" "}
        <Link to="/users">Users</Link> |{" "}
        <Link to="/profile">Profile</Link>
      </nav>
    </header>
  );
}

export default Header;
