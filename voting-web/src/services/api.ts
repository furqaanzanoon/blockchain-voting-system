import axios from "axios";

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? "/api",
  withCredentials: true, // Sends the HttpOnly access_token cookie on every request
});

// NOTE: No Authorization header is set here intentionally.
// The backend authenticates via an HttpOnly cookie (access_token) which is
// sent automatically by the browser with withCredentials: true.
// Storing the JWT in localStorage and adding it to headers would be an XSS risk.

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
