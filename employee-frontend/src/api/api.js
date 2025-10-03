import axios from "axios";

// 👇 List possible backend URLs (http and https)
const backendUrls = [
  "https://localhost:7219/api",
  "http://localhost:5006/api",
];

// Pick the first one that works (basic check)
const getBaseUrl = () => {
  if (process.env.NODE_ENV === "development") {
    // During dev → use localhost
    return backendUrls[0]; // prefer https
  }
  // Later, in production, you can change this to your deployed API
  return "/api";
};

const api = axios.create({
  baseURL: getBaseUrl(),
});

// Optional: attach token automatically if saved in localStorage
api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default api;


/*
   What this does:

    - axios.create → sets your backend URL one time.

    - interceptors → if user logs in, the saved token will be added automatically to every request.
*/