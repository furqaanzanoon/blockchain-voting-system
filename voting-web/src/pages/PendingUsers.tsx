import { useEffect, useState } from "react";
import api from "../services/api";
import { FaUserCheck, FaUserTimes, FaUsers } from "react-icons/fa";
import { useToast } from "../context/ToastContext";

interface PendingUser {
  userId: string;
  fullName: string;
  email: string;
  role: number;
  partyAffiliation?: string;
}

const roleMap: { [key: number]: string } = {
  0: "Voter",
  1: "Admin",
  2: "Election Officer",
  3: "Party",
  4: "Candidate",
};

export default function PendingUsers() {
  const { showToast } = useToast();
  const [pendingUsers, setPendingUsers] = useState<PendingUser[]>([]);
  const [loading, setLoading] = useState(false);
  const [actionUserId, setActionUserId] = useState("");

  const fetchPendingUsers = async () => {
    try {
      setLoading(true);
      const res = await api.get("/users/pending");
      setPendingUsers(res.data);
    } catch (err) {
      console.error(err);
      showToast("Failed to load pending users.", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPendingUsers();
  }, []);

  const handleApprove = async (userId: string) => {
    try {
      setActionUserId(userId);
      await api.post(`/users/${userId}/approve`);
      showToast("User account approved successfully", "success");
      setPendingUsers((prev) => prev.filter((u) => u.userId !== userId));
    } catch (err: any) {
      showToast(err?.response?.data?.message || "Failed to approve user", "error");
    } finally {
      setActionUserId("");
    }
  };

  const handleReject = async (userId: string) => {
    if (!window.confirm("Are you sure you want to reject and delete this registration?")) return;

    try {
      setActionUserId(userId);
      await api.post(`/users/${userId}/reject`);
      showToast("User registration rejected and deleted", "info");
      setPendingUsers((prev) => prev.filter((u) => u.userId !== userId));
    } catch (err: any) {
      showToast(err?.response?.data?.message || "Failed to reject user", "error");
    } finally {
      setActionUserId("");
    }
  };

  return (
    <div>
      <h1 className="text-4xl font-bold mb-8">User Verification</h1>

      <div className="bg-slate-900 rounded-3xl p-6 border border-slate-800 shadow-lg">
        <div className="flex items-center gap-3 mb-6">
          <FaUsers className="text-cyan-400" size={24} />
          <h2 className="text-xl font-bold">Pending Registrations</h2>
        </div>

        {loading && pendingUsers.length === 0 ? (
          <div className="text-center py-10 text-slate-500">
            <span className="animate-spin inline-block h-6 w-6 border-2 border-cyan-400 border-t-transparent rounded-full mr-2 align-middle"></span>
            Loading pending approvals...
          </div>
        ) : pendingUsers.length === 0 ? (
          <div className="text-center py-10 text-slate-500 font-medium">
            No registrations are currently pending approval.
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className="border-b border-slate-800 text-slate-400 text-xs font-semibold uppercase tracking-wider">
                  <th className="py-4 px-4">Full Name</th>
                  <th className="py-4 px-4">Email Address</th>
                  <th className="py-4 px-4">Requested Role</th>
                  <th className="py-4 px-4 text-center">Actions</th>
                </tr>
              </thead>
              <tbody>
                {pendingUsers.map((user) => (
                  <tr
                    key={user.userId}
                    className="border-b border-slate-800/60 hover:bg-slate-800/10 transition duration-150 text-sm"
                  >
                    <td className="py-4 px-4 font-semibold text-slate-200">{user.fullName}</td>
                    <td className="py-4 px-4 text-slate-400 font-mono">{user.email}</td>
                    <td className="py-4 px-4">
                      <span
                        className={`inline-block px-3 py-1 rounded-full text-xs font-bold ${
                          user.role === 2
                            ? "bg-purple-500/10 text-purple-400 border border-purple-500/25"
                            : user.role === 4
                            ? "bg-pink-500/10 text-pink-400 border border-pink-500/25"
                            : "bg-cyan-500/10 text-cyan-400 border border-cyan-500/25"
                        }`}
                      >
                        {user.role === 4
                          ? `Candidate (${user.partyAffiliation || "Independent"})`
                          : roleMap[user.role] || "Unknown"}
                      </span>
                    </td>
                    <td className="py-4 px-4 text-center">
                      <div className="flex items-center justify-center gap-3">
                        <button
                          onClick={() => handleApprove(user.userId)}
                          disabled={actionUserId !== ""}
                          className="bg-green-500 hover:bg-green-400 disabled:bg-slate-700 text-black font-bold p-2.5 rounded-xl transition flex items-center gap-1.5 text-xs shadow-md shadow-green-500/5"
                          title="Approve User"
                        >
                          <FaUserCheck size={14} />
                          <span className="hidden sm:inline">Approve</span>
                        </button>
                        <button
                          onClick={() => handleReject(user.userId)}
                          disabled={actionUserId !== ""}
                          className="bg-rose-500/10 hover:bg-rose-500/20 text-rose-400 disabled:text-slate-500 border border-rose-500/25 disabled:border-slate-800 disabled:bg-transparent font-bold p-2.5 rounded-xl transition flex items-center gap-1.5 text-xs"
                          title="Reject User"
                        >
                          <FaUserTimes size={14} />
                          <span className="hidden sm:inline">Reject</span>
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}
