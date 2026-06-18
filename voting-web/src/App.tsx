import React, { Suspense } from "react";
import {
  BrowserRouter,
  Routes,
  Route
} from "react-router-dom";

const Dashboard = React.lazy(() => import("./pages/Dashboard"));
const Login = React.lazy(() => import("./pages/Login"));
const Register = React.lazy(() => import("./pages/Register"));
const OfficerOtp = React.lazy(() => import("./pages/OfficerOtp"));

export default function App() {
  return (
    <BrowserRouter>
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

        </Routes>
      </Suspense>
    </BrowserRouter>
  );
}
