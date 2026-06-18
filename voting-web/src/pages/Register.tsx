import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../services/api";
import { useToast } from "../context/ToastContext";

export default function Register() {
  const navigate = useNavigate();
  const { showToast } = useToast();

  const [fullName, setFullName] =
    useState("");
  const [email, setEmail] =
    useState("");
  const [password, setPassword] =
    useState("");
  const [confirmPassword, setConfirmPassword] =
    useState("");
  const [role, setRole] =
    useState(0); // 0 = Voter, 2 = ElectionOfficer, 3 = Party, 4 = Candidate
  const [partyAffiliation, setPartyAffiliation] =
    useState("Independent");
  const [parties, setParties] =
    useState<any[]>([]);
  const [otp, setOtp] =
    useState("");
  const [needsOtp, setNeedsOtp] =
    useState(false);
  const [loading, setLoading] =
    useState(false);

  useEffect(() => {
    const fetchParties = async () => {
      try {
        const res = await api.get("/users/parties");
        setParties(res.data || []);
      } catch (err) {
        console.error(err);
      }
    };
    fetchParties();
  }, []);

  const handleRegister = async (
    e: React.FormEvent
  ) => {
    e.preventDefault();

    try {
      setLoading(true);

      await api.post(
        "/auth/register",
        {
          fullName,
          email,
          password,
          confirmPassword,
          role,
          partyAffiliation: role === 4 ? partyAffiliation : null,
        }
      );

      showToast(
        "Registration successful. Check your email for the OTP.",
        "success"
      );

      setNeedsOtp(true);
    } catch (error: any) {
      showToast(
        error?.response?.data
          ?.message ||
          "Registration failed",
        "error"
      );
    } finally {
      setLoading(false);
    }
  };

  const verifyOtp = async () => {
    try {
      setLoading(true);

      await api.post(
        "/auth/verify-otp",
        {
          email,
          otp,
        }
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
    }
  };

  return (
    <div className="min-h-screen flex justify-center items-center bg-slate-950">
      <div className="w-[500px] bg-slate-900 p-8 rounded-3xl border border-slate-800 shadow-2xl">
        <h1 className="text-4xl font-bold text-center text-cyan-400 mb-8">
          Create Account
        </h1>

        <form onSubmit={handleRegister}>
          <input
            type="text"
            placeholder="Full Name"
            value={fullName}
            onChange={(e) =>
              setFullName(
                e.target.value
              )
            }
            required
            className="w-full p-4 mb-4 rounded-xl bg-slate-800 border border-slate-700 text-white"
          />

          <input
            type="email"
            placeholder="Email Address"
            value={email}
            onChange={(e) =>
              setEmail(e.target.value)
            }
            required
            className="w-full p-4 mb-4 rounded-xl bg-slate-800 border border-slate-700 text-white"
          />

          <input
            type="password"
            placeholder="Password"
            value={password}
            onChange={(e) =>
              setPassword(
                e.target.value
              )
            }
            required
            className="w-full p-4 mb-4 rounded-xl bg-slate-800 border border-slate-700 text-white"
          />

          <input
            type="password"
            placeholder="Confirm Password"
            value={confirmPassword}
            onChange={(e) =>
              setConfirmPassword(
                e.target.value
              )
            }
            required
            className="w-full p-4 mb-4 rounded-xl bg-slate-800 border border-slate-700 text-white"
          />

          <label className="block text-slate-400 text-xs font-semibold mb-2 ml-1 uppercase tracking-wider">
            Register As
          </label>
          <select
            value={role}
            onChange={(e) =>
              setRole(Number(e.target.value))
            }
            className="w-full p-4 mb-6 rounded-xl bg-slate-800 border border-slate-700 text-white cursor-pointer"
          >
            <option value={0}>Voter</option>
            <option value={3}>Party</option>
            <option value={4}>Candidate</option>
          </select>

          {role === 4 && (
            <>
              <label className="block text-slate-400 text-xs font-semibold mb-2 ml-1 uppercase tracking-wider">
                Party Affiliation
              </label>
              <select
                value={partyAffiliation}
                onChange={(e) => setPartyAffiliation(e.target.value)}
                className="w-full p-4 mb-6 rounded-xl bg-slate-800 border border-slate-700 text-white cursor-pointer animate-fade-in"
              >
                <option value="Independent">Independent</option>
                {parties.map((p) => (
                  <option key={p.userId} value={p.fullName}>
                    {p.fullName}
                  </option>
                ))}
              </select>
            </>
          )}

          <button
            type="submit"
            disabled={loading || needsOtp}
            className="w-full p-4 rounded-xl bg-green-500 text-black font-bold hover:bg-green-400"
          >
            {loading
              ? "Creating Account..."
              : "Create Account"}
          </button>
        </form>

        {needsOtp && (
          <div className="mt-6">
            <input
              type="text"
              placeholder="Email OTP"
              value={otp}
              onChange={(e) =>
                setOtp(e.target.value)
              }
              className="w-full p-4 mb-4 rounded-xl bg-slate-800 border border-slate-700 text-white"
            />

            <button
              type="button"
              onClick={verifyOtp}
              disabled={loading}
              className="w-full p-4 rounded-xl bg-cyan-500 text-black font-bold hover:bg-cyan-400"
            >
              Verify OTP
            </button>

            <button
              type="button"
              onClick={resendOtp}
              className="w-full mt-4 text-cyan-400 hover:text-cyan-300"
            >
              Resend OTP
            </button>
          </div>
        )}

        <button
          onClick={() =>
            navigate("/login")
          }
          className="w-full mt-4 text-cyan-400 hover:text-cyan-300"
        >
          Back to Login
        </button>
      </div>
    </div>
  );
}
