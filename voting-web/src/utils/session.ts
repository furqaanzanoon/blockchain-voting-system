export const initSession = () => {
  const isSessionActive = document.cookie
    .split("; ")
    .some((row) => row.startsWith("session_active="));

  if (!isSessionActive) {
    // Browser session has expired (browser was closed)
    localStorage.removeItem("token");
    localStorage.removeItem("userId");
    localStorage.removeItem("role");
    localStorage.removeItem("fullName");
    
    // Initialize session cookie for the current browser session
    document.cookie = "session_active=true; path=/; SameSite=Strict";
  }
};

export const startSession = () => {
  document.cookie = "session_active=true; path=/; SameSite=Strict";
};

export const clearSession = () => {
  document.cookie = "session_active=; path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT; SameSite=Strict";
  localStorage.removeItem("token");
  localStorage.removeItem("userId");
  localStorage.removeItem("role");
  localStorage.removeItem("fullName");
  localStorage.removeItem("walletAddress");
};
