import { useEffect, useState } from "react";
import api from "../services/api";
import { FaCalendarAlt } from "react-icons/fa";
import { formatToIST } from "../utils/formatIST";
import { useToast } from "../context/ToastContext";

interface Election {
  electionId: string;
  title: string;
  description: string;
  startTime: string;
  endTime: string;
  status: string | number;
  autoActivate: boolean;
  autoActivateFailReason: string | null;
  autoClose: boolean;
}

const normalizeStatus = (
  status: Election["status"]
) => {
  if (typeof status === "number") {
    return [
      "Draft",
      "Active",
      "Closed",
    ][status] ?? "Draft";
  }

  return status;
};

export default function Elections() {
  const role =
    localStorage.getItem("role") ||
    "Voter";

  const canManage =
    role === "Admin" ||
    role === "ElectionOfficer";

  const { showToast } = useToast();

  const [elections, setElections] =
    useState<Election[]>([]);
  const [activatingId, setActivatingId] = useState("");
  const [closingId, setClosingId] = useState("");

  const [form, setForm] = useState({
    title: "",
    description: "",
    startDate: "",
    endDate: "",
    autoActivate: false,
    autoClose: false,
  });

  const fetchElections = async () => {
    try {
      const res =
        await api.get("/elections");

      setElections(
        res.data.elections ?? []
      );
    } catch (err) {
      console.error(err);
    }
  };

  useEffect(() => {
    fetchElections();
    const interval = setInterval(fetchElections, 5000);
    return () => clearInterval(interval);
  }, []);

  const createElection = async () => {
    try {
      await api.post(
        "/elections",
        {
          title: form.title,
          description:
            form.description,
          startTime: new Date(
            form.startDate + "+05:30"
          ).toISOString(),
          endTime: new Date(
            form.endDate + "+05:30"
          ).toISOString(),
          autoActivate:
            form.autoActivate,
          autoClose:
            form.autoClose,
        }
      );

      showToast(
        "Election Created Successfully",
        "success"
      );

      setForm({
        title: "",
        description: "",
        startDate: "",
        endDate: "",
        autoActivate: false,
        autoClose: false,
      });

      fetchElections();
    } catch (err: any) {
      console.error(err);

      showToast(
        err?.response?.data
          ?.message ||
          "Failed to create election",
        "error"
      );
    }
  };

  const activateElection = async (
    electionId: string
  ) => {
    try {
      setActivatingId(electionId);

      showToast(
        "Deploying election to blockchain. This may take 1-2 minutes...",
        "info"
      );

      await api.put(
        `/elections/${electionId}/activate`,
        null,
        { timeout: 180000 } // 3 min timeout for blockchain txs
      );

      showToast("Election activated successfully!", "success");
      fetchElections();
    } catch (err: any) {
      const isTimeout =
        err?.code === "ECONNABORTED" ||
        err?.message?.includes("timeout");

      showToast(
        isTimeout
          ? "Activation is taking longer than expected. Please refresh the page to check the status."
          : err?.response?.data?.message ||
            "Failed to activate election",
        isTimeout ? "warning" : "error"
      );

      // Refresh anyway in case it eventually succeeded
      if (isTimeout) fetchElections();
    } finally {
      setActivatingId("");
    }
  };

  const closeElection = async (
    electionId: string
  ) => {
    try {
      setClosingId(electionId);

      await api.put(
        `/elections/${electionId}/close`,
        null,
        { timeout: 120000 } // 2 min timeout
      );

      showToast("Election closed", "success");
      fetchElections();
    } catch (err: any) {
      const isTimeout =
        err?.code === "ECONNABORTED" ||
        err?.message?.includes("timeout");

      showToast(
        isTimeout
          ? "Close operation is taking longer than expected. Please refresh."
          : err?.response?.data?.message ||
            "Failed to close election",
        isTimeout ? "warning" : "error"
      );
    } finally {
      setClosingId("");
    }
  };

  return (
    <div>
      <h1 className="text-4xl font-bold mb-8">
        Elections
      </h1>

      {canManage && (
        <div className="bg-slate-900 rounded-3xl p-6 mb-8 shadow-lg">
          <h2 className="text-2xl font-semibold mb-5">
            Create New Election
          </h2>

          <div className="grid md:grid-cols-2 gap-4">
            <input
              className="bg-slate-800 p-3 rounded-xl border border-slate-700"
              placeholder="Election Title"
              value={form.title}
              onChange={(e) =>
                setForm({
                  ...form,
                  title:
                    e.target.value,
                })
              }
            />

            <input
              className="bg-slate-800 p-3 rounded-xl border border-slate-700"
              placeholder="Description"
              value={form.description}
              onChange={(e) =>
                setForm({
                  ...form,
                  description:
                    e.target.value,
                })
              }
            />

            <input
              type="datetime-local"
              className="bg-slate-800 p-3 rounded-xl border border-slate-700"
              value={form.startDate}
              onChange={(e) =>
                setForm({
                  ...form,
                  startDate:
                    e.target.value,
                })
              }
            />

            <input
              type="datetime-local"
              className="bg-slate-800 p-3 rounded-xl border border-slate-700"
              value={form.endDate}
              onChange={(e) =>
                setForm({
                  ...form,
                  endDate:
                    e.target.value,
                })
              }
            />
          </div>

          <div className="flex flex-wrap gap-6 mt-4">
            <label className="flex items-center gap-3 cursor-pointer select-none">
              <input
                type="checkbox"
                checked={form.autoActivate}
                onChange={(e) =>
                  setForm({
                    ...form,
                    autoActivate:
                      e.target.checked,
                  })
                }
                className="w-5 h-5 accent-cyan-500 rounded"
              />
              <span className="text-slate-300 font-medium">
                Auto Activate at Start
              </span>
            </label>

            <label className="flex items-center gap-3 cursor-pointer select-none">
              <input
                type="checkbox"
                checked={form.autoClose}
                onChange={(e) =>
                  setForm({
                    ...form,
                    autoClose:
                      e.target.checked,
                  })
                }
                className="w-5 h-5 accent-cyan-500 rounded"
              />
              <span className="text-slate-300 font-medium">
                Auto Close at End
              </span>
            </label>
          </div>

          <button
            onClick={createElection}
            className="mt-5 bg-cyan-500 hover:bg-cyan-400 text-black font-bold px-6 py-3 rounded-xl"
          >
            Create Election
          </button>
        </div>
      )}

      <h2 className="text-2xl font-bold mb-5">
        Available Elections
      </h2>

      <div className="grid md:grid-cols-2 xl:grid-cols-3 gap-6">
        {elections
          .filter((election) => {
            const status =
              normalizeStatus(
                election.status
              );
            return (
              !(
                status === "Closed" &&
                !canManage
              ) &&
              !(
                status === "Draft" &&
                new Date(
                  typeof election.endTime === "string" && !/Z|[+-]\d{2}:\d{2}$/.test(election.endTime)
                    ? election.endTime + "Z"
                    : election.endTime
                ) < new Date()
              )
            );
          })
          .map((election) => {
          const status =
            normalizeStatus(
              election.status
            );

          return (
            <div
              key={election.electionId}
              className="bg-slate-900 rounded-3xl p-6 shadow-lg hover:scale-105 transition duration-300"
            >
              <h3 className="text-xl font-bold mb-2 text-cyan-400">
                {election.title}
              </h3>

              <p className="text-slate-400 mb-4">
                {election.description}
              </p>

              <div className="flex items-center gap-2 mb-2 text-slate-300">
                <FaCalendarAlt />
                Start:{" "}
                {formatToIST(election.startTime)}
              </div>

              <div className="flex items-center gap-2 mb-4 text-slate-300">
                <FaCalendarAlt />
                End:{" "}
                {formatToIST(election.endTime)}
              </div>

              <div className="flex justify-between items-center mb-4">
                <span
                  className={`px-3 py-1 rounded-full text-black text-sm font-bold ${
                    status === "Active"
                      ? "bg-green-500"
                      : status ===
                        "Closed"
                      ? "bg-red-500"
                      : "bg-yellow-500"
                  }`}
                >
                  {status}
                </span>

                <div className="flex flex-col items-end gap-1">
                  {election.autoActivate && status === "Draft" && (
                    <span className="text-amber-400 text-xs font-semibold">
                      ⚡ Auto Activate
                    </span>
                  )}
                  {election.autoClose && status !== "Closed" && (
                    <span className="text-amber-400 text-xs font-semibold">
                      ⚡ Auto Close
                    </span>
                  )}
                  {!election.autoActivate && status === "Draft" && (
                    <span className="text-cyan-400 text-xs font-semibold">
                      Blockchain Ready
                    </span>
                  )}
                </div>
              </div>

              {election.autoActivateFailReason && (
                <div className="bg-red-900/40 border border-red-500/50 text-red-300 text-sm rounded-xl px-4 py-2 mb-4">
                  ⚠️ Auto-activate failed: {election.autoActivateFailReason}
                </div>
              )}

              {canManage && status !== "Closed" && (
                <div className="w-full">
                  {status === "Draft" && (
                    <button
                      onClick={() =>
                        activateElection(
                          election.electionId
                        )
                      }
                      disabled={Boolean(activatingId)}
                      className={`py-2 rounded-xl font-bold w-full transition-all ${
                        activatingId === election.electionId
                          ? "bg-green-500/60 text-black/60 cursor-wait animate-pulse"
                          : activatingId
                          ? "bg-slate-600 text-slate-400 cursor-not-allowed"
                          : "bg-green-500 hover:bg-green-400 text-black"
                      }`}
                    >
                      {activatingId === election.electionId
                        ? "⏳ Deploying to Blockchain..."
                        : "Activate"}
                    </button>
                  )}

                  {status === "Active" && (
                    <button
                      onClick={() =>
                        closeElection(
                          election.electionId
                        )
                      }
                      disabled={Boolean(closingId)}
                      className={`py-2 rounded-xl font-bold w-full transition-all ${
                        closingId === election.electionId
                          ? "bg-red-600/60 text-white/60 cursor-wait animate-pulse"
                          : closingId
                          ? "bg-slate-600 text-slate-400 cursor-not-allowed"
                          : "bg-red-600 hover:bg-red-500 text-white"
                      }`}
                    >
                      {closingId === election.electionId
                        ? "⏳ Closing..."
                        : "Close"}
                    </button>
                  )}
                </div>
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
}
