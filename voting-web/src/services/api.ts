import axios from "axios";

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? "/api",
  withCredentials: true, // Sends the HttpOnly access_token cookie on every request
});

// Send JWT token in Authorization header if present, as a fallback/primary auth method.
// This ensures authentication works when hosted cross-domain online, since modern browsers
// block third-party cookies (HttpOnly access_token cookie) from being sent.
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Clear any residual client-side auth state and redirect to login
      localStorage.removeItem("token");
      localStorage.removeItem("userId");
      localStorage.removeItem("role");
      localStorage.removeItem("fullName");
      window.location.href = "/login";
    }
    return Promise.reject(error);
  }
);

export default api;
