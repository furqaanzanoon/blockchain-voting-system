import { useEffect, useState } from "react";
import api from "../services/api";
import { FaUserCheck, FaUserTimes, FaFlag, FaUserTie } from "react-icons/fa";
import { useToast } from "../context/ToastContext";

interface PartyUser {
  userId: string;
  fullName: string;
  email: string;
  isApproved: boolean;
}

interface CandidateUser {
  userId: string;
  fullName: string;
  email: string;
  isApproved: boolean;
  partyAffiliation?: string;
}

export default function Parties() {
  const { showToast } = useToast();
  const [parties, setParties] = useState<PartyUser[]>([]);
  const [candidates, setCandidates] = useState<CandidateUser[]>([]);
  const [loading, setLoading] = useState(false);
  const [actionUserId, setActionUserId] = useState("");

  const fetchData = async () => {
    try {
      setLoading(true);
      const [partiesRes, candidatesRes] = await Promise.all([
        api.get("/users/all-parties"),
        api.get("/users/all-candidates"),
      ]);
      setParties(partiesRes.data || []);
      setCandidates(candidatesRes.data || []);
    } catch (err) {
      console.error(err);
      showToast("Failed to load registration data.", "error");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const handleApprove = async (userId: string) => {
    try {
      setActionUserId(userId);
      await api.post(`/users/${userId}/approve`);
      showToast("User account approved successfully", "success");
      
      setParties((prev) =>
        prev.map((p) => (p.userId === userId ? { ...p, isApproved: true } : p))
      );
      setCandidates((prev) =>
        prev.map((c) => (c.userId === userId ? { ...c, isApproved: true } : c))
      );
    } catch (err: any) {
      showToast(err?.response?.data?.message || "Failed to approve user", "error");
    } finally {
      setActionUserId("");
    }
  };

  const handleReject = async (userId: string) => {
    const isCandidate = candidates.some((c) => c.userId === userId);
    const confirmMessage = isCandidate
      ? "Are you sure you want to reject and delete this candidate?"
      : "Are you sure you want to reject and delete this party?";

    if (!window.confirm(confirmMessage)) return;

    try {
      setActionUserId(userId);
      await api.post(`/users/${userId}/reject`);
      showToast("User registration rejected and deleted", "info");
      
      setParties((prev) => prev.filter((p) => p.userId !== userId));
      setCandidates((prev) => prev.filter((c) => c.userId !== userId));
    } catch (err: any) {
      showToast(err?.response?.data?.message || "Failed to reject user", "error");
    } finally {
      setActionUserId("");
    }
  };

  const independentCandidates = candidates.filter(
    (c) => c.partyAffiliation === "Independent"
  );

  return (
    <div>
      <h1 className="text-4xl font-bold mb-8 text-white">Parties & Independent Candidates</h1>

      {/* Political Parties Section */}
      <div className="bg-slate-900 rounded-3xl p-6 border border-slate-800 shadow-lg mb-8">
        <div className="flex items-center gap-3 mb-6">
          <FaFlag className="text-cyan-400" size={24} />
          <h2 className="text-xl font-bold text-white">Registered Political Parties</h2>
        </div>

        {loading && parties.length === 0 ? (
          <div className="text-center py-10 text-slate-500">
            <span className="animate-spin inline-block h-6 w-6 border-2 border-cyan-400 border-t-transparent rounded-full mr-2 align-middle"></span>
            Loading parties...
          </div>
        ) : parties.length === 0 ? (
          <div className="text-center py-10 text-slate-500 font-medium">
            No political parties have been registered yet.
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className="border-b border-slate-800 text-slate-400 text-xs font-semibold uppercase tracking-wider">
                  <th className="py-4 px-4">Party Name</th>
                  <th className="py-4 px-4">Email Address</th>
                  <th className="py-4 px-4">Approval Status</th>
                  <th className="py-4 px-4 text-center">Actions</th>
                </tr>
              </thead>
              <tbody>
                {parties.map((party) => (
                  <tr
                    key={party.userId}
                    className="border-b border-slate-800/60 hover:bg-slate-800/20 transition-all duration-200"
                  >
                    <td className="py-4 px-4 font-semibold text-white">
                      {party.fullName}
                    </td>
                    <td className="py-4 px-4 text-slate-300">
                      {party.email}
                    </td>
                    <td className="py-4 px-4">
                      {party.isApproved ? (
                        <span className="px-3 py-1 rounded-full text-xs font-bold bg-green-500/10 text-green-400 border border-green-500/20">
                          Approved
                        </span>
                      ) : (
                        <span className="px-3 py-1 rounded-full text-xs font-bold bg-amber-500/10 text-amber-400 border border-amber-500/20">
                          Pending Approval
                        </span>
                      )}
                    </td>
                    <td className="py-4 px-4 text-center">
                      <div className="flex items-center justify-center gap-2">
                        {!party.isApproved && (
                          <button
                            onClick={() => handleApprove(party.userId)}
                            disabled={actionUserId !== ""}
                            className="flex items-center gap-1.5 px-3.5 py-2 rounded-xl text-xs font-bold bg-green-500 text-black hover:bg-green-400 disabled:opacity-50 transition-all"
                          >
                            <FaUserCheck />
                            {actionUserId === party.userId ? "Approving..." : "Approve"}
                          </button>
                        )}
                        <button
                          onClick={() => handleReject(party.userId)}
                          disabled={actionUserId !== ""}
                          className="flex items-center gap-1.5 px-3.5 py-2 rounded-xl text-xs font-bold bg-red-600 hover:bg-red-500 text-white disabled:opacity-50 transition-all"
                        >
                          <FaUserTimes />
                          {actionUserId === party.userId ? "Processing..." : "Reject"}
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

      {/* Independent Candidates Section */}
      <div className="bg-slate-900 rounded-3xl p-6 border border-slate-800 shadow-lg">
        <div className="flex items-center gap-3 mb-6">
          <FaUserTie className="text-cyan-400" size={24} />
          <h2 className="text-xl font-bold text-white">Registered Independent Candidates</h2>
        </div>

        {loading && independentCandidates.length === 0 ? (
          <div className="text-center py-10 text-slate-500">
            <span className="animate-spin inline-block h-6 w-6 border-2 border-cyan-400 border-t-transparent rounded-full mr-2 align-middle"></span>
            Loading independent candidates...
          </div>
        ) : independentCandidates.length === 0 ? (
          <div className="text-center py-10 text-slate-500 font-medium">
            No independent candidates have been registered yet.
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className="border-b border-slate-800 text-slate-400 text-xs font-semibold uppercase tracking-wider">
                  <th className="py-4 px-4">Candidate Name</th>
                  <th className="py-4 px-4">Email Address</th>
                  <th className="py-4 px-4">Approval Status</th>
                  <th className="py-4 px-4 text-center">Actions</th>
                </tr>
              </thead>
              <tbody>
                {independentCandidates.map((candidate) => (
                  <tr
                    key={candidate.userId}
                    className="border-b border-slate-800/60 hover:bg-slate-800/20 transition-all duration-200"
                  >
                    <td className="py-4 px-4 font-semibold text-white">
                      {candidate.fullName}
                    </td>
                    <td className="py-4 px-4 text-slate-300">
                      {candidate.email}
                    </td>
                    <td className="py-4 px-4">
                      {candidate.isApproved ? (
                        <span className="px-3 py-1 rounded-full text-xs font-bold bg-green-500/10 text-green-400 border border-green-500/20">
                          Approved
                        </span>
                      ) : (
                        <span className="px-3 py-1 rounded-full text-xs font-bold bg-amber-500/10 text-amber-400 border border-amber-500/20">
                          Pending Approval
                        </span>
                      )}
                    </td>
                    <td className="py-4 px-4 text-center">
                      <div className="flex items-center justify-center gap-2">
                        {!candidate.isApproved && (
                          <button
                            onClick={() => handleApprove(candidate.userId)}
                            disabled={actionUserId !== ""}
                            className="flex items-center gap-1.5 px-3.5 py-2 rounded-xl text-xs font-bold bg-green-500 text-black hover:bg-green-400 disabled:opacity-50 transition-all"
                          >
                            <FaUserCheck />
                            {actionUserId === candidate.userId ? "Approving..." : "Approve"}
                          </button>
                        )}
                        <button
                          onClick={() => handleReject(candidate.userId)}
                          disabled={actionUserId !== ""}
                          className="flex items-center gap-1.5 px-3.5 py-2 rounded-xl text-xs font-bold bg-red-600 hover:bg-red-500 text-white disabled:opacity-50 transition-all"
                        >
                          <FaUserTimes />
                          {actionUserId === candidate.userId ? "Processing..." : "Reject"}
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
