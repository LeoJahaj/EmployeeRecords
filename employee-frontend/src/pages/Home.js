// src/pages/Home.js
import React from "react";
import { Link } from "react-router-dom";

function Home() {
  return (
    <div className="page-container">
      <h2>🏠 Welcome</h2>
      <p>Select a section to continue:</p>
      <ul className="list">
        <li><Link to="/projects">📂 Projects</Link></li>
        <li><Link to="/tasks">📝 Tasks</Link></li>
        <li><Link to="/profile">👤 Profile</Link></li>
        <li><Link to="/users">👥 Users</Link></li>
      </ul>
    </div>
  );
}

export default Home;
