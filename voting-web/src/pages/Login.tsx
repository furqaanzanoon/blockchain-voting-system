import { useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../services/api";
import { useToast } from "../context/ToastContext";
import { startSession } from "../utils/session";

type LoginRole =
  | "Voter"
  | "Admin"
  | "ElectionOfficer"
  | "Party"
  | "Candidate";

const claimUris: Record<string, string> = {
  nameidentifier:
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
  name: "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
  emailaddress:
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
  role: "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
};

const parseJwtPayload = (token: string) => {
  const payload = token.split(".")[1] ?? "";
  const normalized = payload
    .replace(/-/g, "+")
    .replace(/_/g, "/")
    .padEnd(
      Math.ceil(payload.length / 4) * 4,
      "="
    );

  return JSON.parse(atob(normalized));
};

const getClaim = (
  payload: Record<string, string>,
  name: keyof typeof claimUris
) => payload[claimUris[name]] ?? payload[name] ?? "";

export default function Login() {
  const navigate = useNavigate();
  const { showToast } = useToast();

  const [email, setEmail] =
    useState("");
  const [password, setPassword] =
    useState("");
  const [role, setRole] =
    useState<LoginRole>("Voter");
  const [loading, setLoading] =
    useState(false);

  // OTP state
  const [needsOtp, setNeedsOtp] =
    useState(false);
  const [otp, setOtp] = useState("");

  // Password reset state
  const [isForgotPassword, setIsForgotPassword] = useState(false);
  const [isResetPassword, setIsResetPassword] = useState(false);
  const [resetEmail, setResetEmail] = useState("");
  const [resetOtp, setResetOtp] = useState("");
  const [newPassword, setNewPassword] = useState("");

  const handleLogin = async (
    e: React.FormEvent
  ) => {
    e.preventDefault();

    try {
      setLoading(true);

      const roleMap = {
        Voter: 0,
        Admin: 1,
        ElectionOfficer: 2,
        Party: 3,
        Candidate: 4,
      };

      const res = await api.post("/auth/login", {
        email,
        password,
        role: roleMap[role],
      });

      if (res.data.requiresOtp) {
        setNeedsOtp(true);
        showToast(res.data.message, "info");
        return;
      }

      // Fallback in case backend doesn't require OTP
      handleTokenResponse(res.data);
    } catch (error: any) {
      const message =
        error?.response?.data?.message ||
        "Login failed";

      if (
        role === "ElectionOfficer" &&
        message
          .toLowerCase()
          .includes("not verified")
      ) {
        localStorage.setItem(
          "pendingOfficerEmail",
          email
        );

        navigate(
          "/officer-otp",
          {
            state: { email },
          }
        );
        return;
      }

      showToast(message, "error");
    } finally {
      setLoading(false);
    }
  };

  const handleVerifyOtp = async (
    e: React.FormEvent
  ) => {
    e.preventDefault();

    try {
      setLoading(true);

      const res = await api.post(
        "/auth/verify-login-otp",
        { email, otp }
      );

      handleTokenResponse(res.data);
    } catch (error: any) {
      showToast(
        error?.response?.data?.message ||
          "OTP verification failed",
        "error"
      );
    } finally {
      setLoading(false);
    }
  };

  const handleResendOtp = async () => {
    try {
      setLoading(true);

      const roleMap = {
        Voter: 0,
        Admin: 1,
        ElectionOfficer: 2,
        Party: 3,
        Candidate: 4,
      };

      await api.post("/auth/login", {
        email,
        password,
        role: roleMap[role],
      });

      showToast("OTP resent to your email.", "success");
    } catch (error: any) {
      showToast(
        error?.response?.data?.message ||
          "Failed to resend OTP",
        "error"
      );
    } finally {
      setLoading(false);
    }
  };

  const handleTokenResponse = (
    data: any
  ) => {
    const token =
      typeof data === "string"
        ? data
        : data.token;

    const payload =
      parseJwtPayload(token);

    const fullName =
      getClaim(payload, "name");

    const userId =
      getClaim(
        payload,
        "nameidentifier"
      );

    const userRole =
      getClaim(payload, "role") ||
      role;

    localStorage.setItem(
      "token",
      token
    );

    localStorage.setItem(
      "userId",
      userId
    );

    localStorage.setItem(
      "role",
      userRole
    );

    localStorage.setItem(
      "fullName",
      fullName || email
    );

    const partyAffiliation = payload["PartyAffiliation"] || "";
    localStorage.setItem("partyAffiliation", partyAffiliation);

    startSession();

    showToast("Login Successful", "success");

    navigate("/");
  };

  const handleForgotPassword = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      setLoading(true);
      const res = await api.post("/auth/forgot-password", {
        email: resetEmail,
      });
      showToast(res.data.message || "OTP sent successfully.", "success");
      setIsForgotPassword(false);
      setIsResetPassword(true);
    } catch (error: any) {
      showToast(
        error?.response?.data?.message || "Failed to send reset OTP.",
        "error"
      );
    } finally {
      setLoading(false);
    }
  };

  const handleResetPassword = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      setLoading(true);
      const res = await api.post("/auth/reset-password", {
        email: resetEmail,
        otp: resetOtp,
        newPassword: newPassword,
      });
      showToast(res.data.message || "Password reset successfully.", "success");
      setIsResetPassword(false);
      setResetEmail("");
      setResetOtp("");
      setNewPassword("");
    } catch (error: any) {
      showToast(
        error?.response?.data?.message || "Failed to reset password.",
        "error"
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex justify-center items-center bg-slate-950 px-4">
      <div className="w-full max-w-[450px] bg-slate-900 p-6 sm:p-8 rounded-3xl shadow-2xl border border-slate-800">
        <h1 className="text-3xl sm:text-4xl font-bold text-center text-cyan-400 mb-8">
          Blockchain Voting
        </h1>

        {isForgotPassword && (
          <form onSubmit={handleForgotPassword}>
            <div className="mb-4">
              <label className="block text-white mb-2">
                Forgot Password
              </label>
              <p className="text-slate-400 text-sm mb-4">
                Enter your email address to receive a password reset OTP code.
              </p>
              <input
                type="email"
                placeholder="Email Address"
                value={resetEmail}
                onChange={(e) => setResetEmail(e.target.value)}
                required
                className="w-full p-3 rounded-xl bg-slate-800 border border-slate-700 text-white focus:outline-none focus:border-cyan-400"
              />
            </div>

            <button
              type="submit"
              disabled={loading}
              className="w-full p-3 rounded-xl bg-cyan-500 text-black font-bold hover:bg-cyan-400 transition"
            >
              {loading ? "Sending..." : "Send Reset OTP"}
            </button>

            <div className="text-center mt-4">
              <button
                type="button"
                onClick={() => {
                  setIsForgotPassword(false);
                  setResetEmail("");
                }}
                className="text-slate-400 hover:text-slate-300 text-sm"
              >
                Back to Login
              </button>
            </div>
          </form>
        )}

        {isResetPassword && (
          <form onSubmit={handleResetPassword}>
            <div className="text-center mb-6">
              <h2 className="text-xl font-semibold text-white mb-1">
                Reset Password
              </h2>
              <p className="text-slate-400 text-sm">
                Enter the code sent to <span className="text-cyan-400">{resetEmail}</span> and your new password.
              </p>
            </div>

            <div className="mb-4">
              <label className="block text-white mb-2">
                OTP Code
              </label>
              <input
                type="text"
                placeholder="6-digit OTP"
                value={resetOtp}
                onChange={(e) => setResetOtp(e.target.value.replace(/\D/g, "").slice(0, 6))}
                maxLength={6}
                required
                className="w-full p-3 rounded-xl bg-slate-800 border border-slate-700 text-white focus:outline-none focus:border-cyan-400 text-center tracking-[0.5em] font-mono text-2xl"
              />
            </div>

            <div className="mb-5">
              <label className="block text-white mb-2">
                New Password
              </label>
              <input
                type="password"
                placeholder="Enter new password"
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
                required
                className="w-full p-3 rounded-xl bg-slate-800 border border-slate-700 text-white focus:outline-none focus:border-cyan-400"
              />
            </div>

            <button
              type="submit"
              disabled={loading || resetOtp.length !== 6}
              className="w-full p-3 rounded-xl bg-cyan-500 text-black font-bold hover:bg-cyan-400 transition disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {loading ? "Resetting..." : "Reset Password"}
            </button>

            <div className="flex justify-between mt-4">
              <button
                type="button"
                onClick={handleForgotPassword}
                disabled={loading}
                className="text-cyan-400 hover:text-cyan-300 text-sm"
              >
                Resend OTP
              </button>

              <button
                type="button"
                onClick={() => {
                  setIsResetPassword(false);
                  setResetEmail("");
                  setResetOtp("");
                  setNewPassword("");
                }}
                className="text-slate-400 hover:text-slate-300 text-sm"
              >
                Cancel
              </button>
            </div>
          </form>
        )}

        {!needsOtp && !isForgotPassword && !isResetPassword && (
          <form onSubmit={handleLogin}>
            <div className="mb-4">
              <label className="block text-white mb-2">
                Email
              </label>

              <input
                type="email"
                value={email}
                onChange={(e) =>
                  setEmail(e.target.value)
                }
                required
                className="w-full p-3 rounded-xl bg-slate-800 border border-slate-700 text-white focus:outline-none focus:border-cyan-400"
              />
            </div>

            <div className="mb-5">
              <label className="block text-white mb-2">
                Password
              </label>

              <input
                type="password"
                value={password}
                onChange={(e) =>
                  setPassword(
                    e.target.value
                  )
                }
                required
                className="w-full p-3 rounded-xl bg-slate-800 border border-slate-700 text-white focus:outline-none focus:border-cyan-400"
              />
            </div>

            <div className="mb-5">
              <label className="block text-white mb-2">
                Role
              </label>

              <select
                value={role}
                onChange={(e) =>
                  setRole(
                    e.target
                      .value as LoginRole
                  )
                }
                className="w-full p-3 rounded-xl bg-slate-800 border border-slate-700 text-white focus:outline-none focus:border-cyan-400"
              >
                <option value="Voter">
                  Voter
                </option>

                <option value="Admin">
                  Admin
                </option>

                <option value="ElectionOfficer">
                  Election Officer
                </option>

                <option value="Party">
                  Party
                </option>

                <option value="Candidate">
                  Candidate
                </option>
              </select>
            </div>

            <button
              type="submit"
              disabled={loading}
              className="w-full p-3 rounded-xl bg-cyan-500 text-black font-bold hover:bg-cyan-400 transition"
            >
              {loading
                ? "Logging in..."
                : "Login"}
            </button>

            <div className="text-right mt-3">
              <button
                type="button"
                onClick={() => setIsForgotPassword(true)}
                className="text-cyan-400 hover:text-cyan-300 text-sm"
              >
                Forgot Password?
              </button>
            </div>
          </form>
        )}

        {needsOtp && !isForgotPassword && !isResetPassword && (
          <form onSubmit={handleVerifyOtp}>
            <div className="text-center mb-6">
              <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-cyan-500/10 border border-cyan-500/30 mb-4">
                <svg className="w-8 h-8 text-cyan-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
                </svg>
              </div>

              <h2 className="text-xl font-semibold text-white mb-1">
                OTP Verification
              </h2>
              <p className="text-slate-400 text-sm">
                Enter the 6-digit code sent to{" "}
                <span className="text-cyan-400">
                  {email}
                </span>
              </p>
            </div>

            <div className="mb-5">
              <input
                type="text"
                value={otp}
                onChange={(e) =>
                  setOtp(
                    e.target.value
                      .replace(/\D/g, "")
                      .slice(0, 6)
                  )
                }
                placeholder="Enter 6-digit OTP"
                maxLength={6}
                required
                className="w-full p-4 rounded-xl bg-slate-800 border border-slate-700 text-white text-center text-2xl tracking-[0.5em] font-mono focus:outline-none focus:border-cyan-400"
              />
            </div>

            <button
              type="submit"
              disabled={loading || otp.length !== 6}
              className="w-full p-3 rounded-xl bg-cyan-500 text-black font-bold hover:bg-cyan-400 transition disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {loading
                ? "Verifying..."
                : "Verify OTP"}
            </button>

            <div className="flex justify-between mt-4">
              <button
                type="button"
                onClick={handleResendOtp}
                disabled={loading}
                className="text-cyan-400 hover:text-cyan-300 text-sm"
              >
                Resend OTP
              </button>

              <button
                type="button"
                onClick={() => {
                  setNeedsOtp(false);
                  setOtp("");
                }}
                className="text-slate-400 hover:text-slate-300 text-sm"
              >
                Back to Login
              </button>
            </div>
          </form>
        )}

        <button
          onClick={() =>
            navigate("/register")
          }
          className="w-full mt-4 text-green-400 hover:text-green-300"
        >
          Create Account
        </button>
      </div>
    </div>
  );
}
