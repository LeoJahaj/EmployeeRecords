// src/pages/ProfilePage.js
import React, { useState } from "react";
import api from "../api/api";
import { jwtDecode } from "jwt-decode";

function ProfilePage() {
  const [profile, setProfile] = useState(null);
  const [updateData, setUpdateData] = useState({
    fullName: "",
    bio: "",
    profilePictureUrl: "",
  });

  const token = localStorage.getItem("token");

  // 🔹 Decode JWT to get userId + role
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

  // 🔹 Fetch profile
  const fetchProfile = async () => {
    try {
      const res = await api.get(`/profile/${currentUser.userId}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      setProfile(res.data);
      setUpdateData({
        fullName: res.data.fullName,
        bio: res.data.bio,
        profilePictureUrl: res.data.profilePictureUrl,
      });
    } catch (err) {
      console.error("Error fetching profile", err);
    }
  };

  // 🔹 Update profile
  const updateProfile = async () => {
    try {
      await api.put(`/profile/${currentUser.userId}`, updateData, {
        headers: { Authorization: `Bearer ${token}` },
      });
      alert("Profile updated successfully!");
      fetchProfile();
    } catch (err) {
      console.error("Error updating profile", err);
    }
  };

  return (
    <div className="page-container">
      <h2>My Profile</h2>

      <div className="button-group">
        <button onClick={fetchProfile}>Load My Profile</button>
      </div>

      {/* 🔹 Display profile info */}
      {profile && (
  <div className="profile-card">
    <h3>Current Profile</h3>
    <p>
      <b>Full Name:</b> {profile.fullName}
    </p>
    <p>
      <b>Bio:</b> {profile.bio}
    </p>
    {profile.profilePictureUrl && (
      <img
        src={profile.profilePictureUrl}
        alt="Profile"
        width="120"
        style={{ borderRadius: "50%", marginTop: "10px" }}
      />
    )}
  </div>
)}


      {/* 🔹 Update form (only editable by the logged-in user) */}
      <h3>Update Profile</h3>
      <input
        placeholder="Full Name"
        value={updateData.fullName}
        onChange={(e) =>
          setUpdateData({ ...updateData, fullName: e.target.value })
        }
      />
      <input
        placeholder="Bio"
        value={updateData.bio}
        onChange={(e) => setUpdateData({ ...updateData, bio: e.target.value })}
      />
      <input
        placeholder="Profile Picture URL"
        value={updateData.profilePictureUrl}
        onChange={(e) =>
          setUpdateData({ ...updateData, profilePictureUrl: e.target.value })
        }
      />

      <div className="button-group">
        <button onClick={updateProfile}>Update</button>
      </div>
    </div>
  );
}

export default ProfilePage;




