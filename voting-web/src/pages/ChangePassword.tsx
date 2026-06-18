import { useState } from "react";
import api from "../services/api";
import { useToast } from "../context/ToastContext";
import { FaLock } from "react-icons/fa";

export default function ChangePassword() {
  const { showToast } = useToast();
  const [loading, setLoading] = useState(false);
  const [form, setForm] = useState({
    currentPassword: "",
    newPassword: "",
    confirmPassword: "",
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (form.newPassword !== form.confirmPassword) {
      showToast("New passwords do not match", "warning");
      return;
    }

    try {
      setLoading(true);
      const res = await api.post("/users/change-password", {
        currentPassword: form.currentPassword,
        newPassword: form.newPassword,
      });

      showToast(res.data.message || "Password changed successfully", "success");
      setForm({
        currentPassword: "",
        newPassword: "",
        confirmPassword: "",
      });
    } catch (err: any) {
      showToast(
        err?.response?.data?.message || "Failed to change password",
        "error"
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-md mx-auto">
      <h1 className="text-4xl font-bold mb-8">Change Password</h1>

      <div className="bg-slate-900 rounded-3xl p-6 shadow-lg border border-slate-800">
        <div className="flex items-center gap-3 mb-6">
          <FaLock className="text-cyan-400" size={24} />
          <h2 className="text-2xl font-semibold">Update Credentials</h2>
        </div>

        <form onSubmit={handleSubmit} className="space-y-5">
          <div>
            <label className="block text-slate-300 mb-2 font-medium">
              Current Password
            </label>
            <input
              type="password"
              value={form.currentPassword}
              onChange={(e) =>
                setForm({ ...form, currentPassword: e.target.value })
              }
              required
              className="w-full bg-slate-800 p-3 rounded-xl border border-slate-700 text-white focus:outline-none focus:border-cyan-400"
              placeholder="Enter current password"
            />
          </div>

          <div>
            <label className="block text-slate-300 mb-2 font-medium">
              New Password
            </label>
            <input
              type="password"
              value={form.newPassword}
              onChange={(e) =>
                setForm({ ...form, newPassword: e.target.value })
              }
              required
              className="w-full bg-slate-800 p-3 rounded-xl border border-slate-700 text-white focus:outline-none focus:border-cyan-400"
              placeholder="Enter new password"
            />
          </div>

          <div>
            <label className="block text-slate-300 mb-2 font-medium">
              Confirm New Password
            </label>
            <input
              type="password"
              value={form.confirmPassword}
              onChange={(e) =>
                setForm({ ...form, confirmPassword: e.target.value })
              }
              required
              className="w-full bg-slate-800 p-3 rounded-xl border border-slate-700 text-white focus:outline-none focus:border-cyan-400"
              placeholder="Confirm new password"
            />
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full bg-cyan-500 hover:bg-cyan-400 text-black font-bold py-3 rounded-xl transition duration-300 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {loading ? "Updating..." : "Change Password"}
          </button>
        </form>
      </div>
    </div>
  );
}
