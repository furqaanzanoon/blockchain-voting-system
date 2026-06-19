import React, { createContext, useContext, useState, useEffect, useCallback } from "react";
import { startSession, clearSession } from "../utils/session";

// ── Types ─────────────────────────────────────────────────────────────────────

export type UserRole =
  | "Voter"
  | "Admin"
  | "ElectionOfficer"
  | "Party"
  | "Candidate";

interface AuthUser {
  userId: string;
  fullName: string;
  email: string;
  role: UserRole;
}

interface AuthContextValue {
  user: AuthUser | null;
  token: string | null;
  isAuthenticated: boolean;
  login: (token: string) => void;
  logout: () => void;
}

// ── JWT helpers ───────────────────────────────────────────────────────────────

const CLAIM_URIS: Record<string, string> = {
  nameidentifier:
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
  name: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
  emailaddress:
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
  role: "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
};

function parseJwt(token: string): Record<string, string> | null {
  try {
    const payload = token.split(".")[1] ?? "";
    const normalized = payload
      .replace(/-/g, "+")
      .replace(/_/g, "/")
      .padEnd(Math.ceil(payload.length / 4) * 4, "=");
    return JSON.parse(atob(normalized));
  } catch {
    return null;
  }
}

function getClaim(
  payload: Record<string, string>,
  name: keyof typeof CLAIM_URIS
): string {
  return payload[CLAIM_URIS[name]] ?? payload[name] ?? "";
}

function userFromToken(token: string): AuthUser | null {
  const payload = parseJwt(token);
  if (!payload) return null;
  return {
    userId: getClaim(payload, "nameidentifier"),
    fullName: getClaim(payload, "name"),
    email: getClaim(payload, "emailaddress"),
    role: getClaim(payload, "role") as UserRole,
  };
}

function isTokenExpired(token: string): boolean {
  const payload = parseJwt(token);
  if (!payload || !payload.exp) return true;
  return Date.now() / 1000 > Number(payload.exp);
}

// ── Context ───────────────────────────────────────────────────────────────────

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [token, setToken] = useState<string | null>(() => {
    const stored = localStorage.getItem("token");
    if (stored && !isTokenExpired(stored)) return stored;
    localStorage.removeItem("token");
    return null;
  });

  const [user, setUser] = useState<AuthUser | null>(() => {
    const stored = localStorage.getItem("token");
    if (stored && !isTokenExpired(stored)) return userFromToken(stored);
    return null;
  });

  const login = useCallback((newToken: string) => {
    const parsedUser = userFromToken(newToken);
    if (!parsedUser) return;

    localStorage.setItem("token", newToken);
    localStorage.setItem("userId", parsedUser.userId);
    localStorage.setItem("role", parsedUser.role);
    localStorage.setItem("fullName", parsedUser.fullName);

    setToken(newToken);
    setUser(parsedUser);
    startSession();
  }, []);

  const logout = useCallback(() => {
    setToken(null);
    setUser(null);
    clearSession();
  }, []);

  // Auto-logout when token expires
  useEffect(() => {
    if (!token) return;
    const payload = parseJwt(token);
    if (!payload?.exp) return;

    const msUntilExpiry = Number(payload.exp) * 1000 - Date.now();
    if (msUntilExpiry <= 0) {
      logout();
      return;
    }

    const timer = setTimeout(() => {
      logout();
    }, msUntilExpiry);

    return () => clearTimeout(timer);
  }, [token, logout]);

  const value: AuthContextValue = {
    user,
    token,
    isAuthenticated: !!user,
    login,
    logout,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) {
    throw new Error("useAuth must be used inside <AuthProvider>");
  }
  return ctx;
}
