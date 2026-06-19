import React, { Suspense } from "react";
import {
  BrowserRouter,
  Routes,
  Route
} from "react-router-dom";
import { AuthProvider } from "./context/AuthContext";

const Dashboard = React.lazy(() => import("./pages/Dashboard"));
const Login = React.lazy(() => import("./pages/Login"));
const Register = React.lazy(() => import("./pages/Register"));
const OfficerOtp = React.lazy(() => import("./pages/OfficerOtp"));

// Simple 404 page
function NotFound() {
  return (
    <div style={{
      display: 'flex',
      flexDirection: 'column',
      justifyContent: 'center',
      alignItems: 'center',
      height: '100vh',
      backgroundColor: '#0f172a',
      color: '#94a3b8',
      fontFamily: 'sans-serif',
      gap: '1rem'
    }}>
      <h1 style={{ fontSize: '4rem', margin: 0, color: '#38bdf8' }}>404</h1>
      <p style={{ fontSize: '1.25rem', margin: 0 }}>Page not found</p>
      <a
        href="/"
        style={{ color: '#38bdf8', textDecoration: 'underline', fontSize: '1rem' }}
      >
        Return to Dashboard
      </a>
    </div>
  );
}

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Suspense fallback={
          <div style={{
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center',
            height: '100vh',
            backgroundColor: '#0f172a',
            color: '#38bdf8',
            fontFamily: 'sans-serif',
            fontSize: '1.25rem'
          }}>
            Loading...
          </div>
        }>
          <Routes>

            <Route
              path="/"
              element={<Dashboard />}
            />

            <Route
              path="/login"
              element={<Login />}
            />

            <Route
              path="/register"
              element={<Register />}
            />

            <Route
              path="/officer-otp"
              element={<OfficerOtp />}
            />

            {/* 404 catch-all — must be last */}
            <Route path="*" element={<NotFound />} />

          </Routes>
        </Suspense>
      </AuthProvider>
    </BrowserRouter>
  );
}
