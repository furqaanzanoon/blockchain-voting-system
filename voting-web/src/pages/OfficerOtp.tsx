import { useEffect, useState } from "react";
import {
  useLocation,
  useNavigate,
} from "react-router-dom";
import api from "../services/api";
import { useToast } from "../context/ToastContext";

interface OfficerOtpState {
  email?: string;
}

export default function OfficerOtp() {
  const navigate = useNavigate();
  const location = useLocation();
  const { showToast } = useToast();
  const state =
    location.state as OfficerOtpState | null;

  const [email, setEmail] = useState(
    state?.email ||
      localStorage.getItem(
        "pendingOfficerEmail"
      ) ||
      ""
  );
  const [otp, setOtp] = useState("");
  const [loading, setLoading] =
    useState(false);

  useEffect(() => {
    if (email) {
      localStorage.setItem(
        "pendingOfficerEmail",
        email
      );
    }
  }, [email]);

  const verifyOtp = async (
    event: React.FormEvent
  ) => {
    event.preventDefault();

    try {
      setLoading(true);

      await api.post(
        "/auth/verify-otp",
        {
          email,
          otp,
        }
      );

      localStorage.removeItem(
        "pendingOfficerEmail"
      );

      showToast(
        "Account verified. You can now log in.",
        "success"
      );

      navigate("/login");
    } catch (error: any) {
      showToast(
        error?.response?.data
          ?.message ||
          "OTP verification failed",
        "error"
      );
    } finally {
      setLoading(false);
    }
  };

  const resendOtp = async () => {
    try {
      setLoading(true);

      await api.post(
        "/auth/resend-otp",
        { email }
      );

      showToast("OTP resent.", "success");
    } catch (error: any) {
      showToast(
        error?.response?.data
          ?.message ||
          "Failed to resend OTP",
        "error"
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex justify-center items-center bg-slate-950">
      <div className="w-[460px] bg-slate-900 p-8 rounded-3xl border border-slate-800 shadow-2xl">
        <h1 className="text-4xl font-bold text-center text-cyan-400 mb-8">
          Verify Officer
        </h1>

        <form onSubmit={verifyOtp}>
          <input
            type="email"
            placeholder="Email Address"
            value={email}
            onChange={(event) =>
              setEmail(event.target.value)
            }
            required
            className="w-full p-4 mb-4 rounded-xl bg-slate-800 border border-slate-700 text-white"
          />

          <input
            type="text"
            placeholder="Email OTP"
            value={otp}
            onChange={(event) =>
              setOtp(event.target.value)
            }
            required
            className="w-full p-4 mb-4 rounded-xl bg-slate-800 border border-slate-700 text-white"
          />

          <button
            type="submit"
            disabled={loading}
            className="w-full p-4 rounded-xl bg-cyan-500 text-black font-bold hover:bg-cyan-400 disabled:bg-slate-600 disabled:text-slate-300"
          >
            {loading
              ? "Verifying..."
              : "Verify OTP"}
          </button>
        </form>

        <button
          type="button"
          onClick={resendOtp}
          disabled={loading || !email}
          className="w-full mt-4 text-cyan-400 hover:text-cyan-300 disabled:text-slate-500"
        >
          Resend OTP
        </button>

        <button
          type="button"
          onClick={() =>
            navigate("/login")
          }
          className="w-full mt-4 text-slate-400 hover:text-slate-300"
        >
          Back to Login
        </button>
      </div>
    </div>
  );
}
