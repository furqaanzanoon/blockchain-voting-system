import { useEffect, useState } from "react";
import api from "../services/api";
import { FaUserCheck } from "react-icons/fa";
import { useToast } from "../context/ToastContext";

interface Election {
  electionId: string;
  title: string;
  status: string | number;
}

const normalizeStatus = (status: string | number) => {
  if (typeof status === "number") {
    return ["Draft", "Active", "Closed"][status] ?? "Draft";
  }
  return status;
};

interface User {
  userId: string;
  fullName: string;
  email: string;
  ethAddress?: string;
}

interface RegisteredVoter {
  userId: string;
  fullName: string;
  email: string;
  ethAddress?: string;
  hasVoted: boolean;
  registeredAt: string;
}

const normalizeUser = (user: any): User => ({
  userId:
    user.userId ??
    user.UserId ??
    user.id ??
    user.Id ??
    "",
  fullName:
    user.fullName ??
    user.FullName ??
    "",
  email:
    user.email ??
    user.Email ??
    "",
  ethAddress:
    user.ethAddress ??
    user.EthAddress,
});

const normalizeRegisteredVoter = (
  voter: any
): RegisteredVoter => ({
  userId:
    voter.userId ??
    voter.UserId ??
    "",
  fullName:
    voter.fullName ??
    voter.FullName ??
    "",
  email:
    voter.email ??
    voter.Email ??
    "",
  ethAddress:
    voter.ethAddress ??
    voter.EthAddress,
  hasVoted:
    voter.hasVoted ??
    voter.HasVoted ??
    false,
  registeredAt:
    voter.registeredAt ??
    voter.RegisteredAt ??
    "",
});

export default function Voters() {
  const [elections, setElections] =
    useState<Election[]>([]);
  const [users, setUsers] = useState<
    User[]
  >([]);
  const [registeredVoters, setRegisteredVoters] =
    useState<RegisteredVoter[]>([]);
  const [selectedElection, setSelectedElection] =
    useState("");
  const [selectedUser, setSelectedUser] =
    useState("");
  const [loading, setLoading] =
    useState(false);

  const { showToast } = useToast();

  useEffect(() => {
    loadElections();
    loadUsers();
  }, []);

  useEffect(() => {
    if (selectedElection) {
      loadRegisteredVoters(
        selectedElection
      );
    }
  }, [selectedElection]);

  const loadElections = async () => {
    try {
      const res =
        await api.get("/elections");
      const allElections: Election[] =
        res.data.elections ?? [];
      const list = allElections.filter(
        (e) => normalizeStatus(e.status) !== "Closed"
      );

      setElections(list);

      if (list.length > 0) {
        setSelectedElection(
          list[0].electionId
        );
      } else {
        setSelectedElection("");
        setRegisteredVoters([]);
      }
    } catch (error) {
      console.error(error);
    }
  };

  const loadUsers = async () => {
    try {
      const res = await api.get("/users");
      const list =
        (res.data.message ?? []).map(
          normalizeUser
        );

      setUsers(list);

      const firstUser = list.find(
        (user: User) => user.userId
      );

      if (firstUser) {
        setSelectedUser(
          firstUser.userId
        );
      }
    } catch (error) {
      console.error(error);
    }
  };

  const loadRegisteredVoters = async (
    electionId: string
  ) => {
    try {
      const res = await api.get(
        `/voters/${electionId}`
      );

      setRegisteredVoters(
        res.data.map(
          normalizeRegisteredVoter
        )
      );
    } catch (error) {
      console.error(error);
      setRegisteredVoters([]);
    }
  };

  const registerVoter = async () => {
    if (!selectedElection || !selectedUser) {
      showToast(
        "Select an election and voter first. If the voter dropdown is filled but this still appears, restart the API so it returns voter UserId values.",
        "warning"
      );
      return;
    }

    try {
      setLoading(true);

      await api.post("/voters/register", {
        electionId: selectedElection,
        userId: selectedUser,
      });

      showToast(
        "Voter registered to election successfully",
        "success"
      );

      loadRegisteredVoters(
        selectedElection
      );
    } catch (error: any) {
      showToast(
        error?.response?.data
          ?.message ||
          "Failed to register voter",
        "error"
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <h1 className="text-4xl font-bold mb-8">
        Voters
      </h1>

      <div className="bg-slate-900 rounded-3xl p-6 mb-8">
        <div className="flex items-center gap-3 mb-5">
          <FaUserCheck className="text-cyan-400" />

          <h2 className="text-2xl font-semibold">
            Register Voter
          </h2>
        </div>

        <div className="grid md:grid-cols-2 gap-4">
          <select
            value={selectedElection}
            onChange={(event) =>
              setSelectedElection(
                event.target.value
              )
            }
            className="bg-slate-800 p-3 rounded-xl border border-slate-700"
          >
            {elections.map(
              (election) => (
                <option
                  key={
                    election.electionId
                  }
                  value={
                    election.electionId
                  }
                >
                  {election.title}
                </option>
              )
            )}
          </select>

          <select
            value={selectedUser}
            onChange={(event) =>
              setSelectedUser(
                event.target.value
              )
            }
            className="bg-slate-800 p-3 rounded-xl border border-slate-700"
          >
            {users.map((user) => (
              <option
                key={user.userId}
                value={user.userId}
                disabled={!user.userId}
              >
                {user.fullName || "Unnamed voter"}
                {user.email
                  ? ` - ${user.email}`
                  : ""}
                {!user.userId
                  ? " - restart API"
                  : ""}
              </option>
            ))}
          </select>
        </div>

        <button
          onClick={registerVoter}
          disabled={loading}
          className="mt-5 bg-cyan-500 hover:bg-cyan-400 disabled:bg-slate-600 disabled:text-slate-300 text-black font-bold px-6 py-3 rounded-xl"
        >
          {loading
            ? "Registering..."
            : "Register Voter"}
        </button>
      </div>

      <div className="bg-slate-900 rounded-3xl p-6">
        <h2 className="text-2xl font-semibold mb-5">
          Registered Voters
        </h2>

        <div className="space-y-3">
          {registeredVoters.map((voter) => (
            <div
              key={voter.userId}
              className="bg-slate-800 rounded-xl p-4 flex flex-col md:flex-row md:items-center md:justify-between gap-2"
            >
              <div>
                <p className="font-bold">
                  {voter.fullName}
                </p>

                <p className="text-slate-400 text-sm">
                  {voter.email}
                </p>
              </div>

              <span
                className={`text-sm font-bold ${
                  voter.hasVoted
                    ? "text-green-400"
                    : "text-cyan-400"
                }`}
              >
                {voter.hasVoted
                  ? "Voted"
                  : "Not Voted"}
              </span>
            </div>
          ))}

          {registeredVoters.length ===
            0 && (
            <p className="text-slate-400">
              No voters registered for this election.
            </p>
          )}
        </div>
      </div>
    </div>
  );
}
