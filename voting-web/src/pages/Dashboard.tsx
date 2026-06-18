import React, { useEffect, useState, Suspense } from "react";
import { useNavigate } from "react-router-dom";
import api from "../services/api";
import { useToast } from "../context/ToastContext";
import { clearSession } from "../utils/session";

import {
  FaVoteYea,
  FaUsers,
  FaChartBar,
  FaUniversity,
  FaWallet,
  FaSignOutAlt,
  FaUser,
  FaUserShield,
  FaLink,
  FaServer,
  FaKey,
  FaCheckCircle,
  FaFlag,
} from "react-icons/fa";

const Elections = React.lazy(() => import("./Elections"));
const Candidates = React.lazy(() => import("./Candidates"));
const Vote = React.lazy(() => import("./Vote"));
const Results = React.lazy(() => import("./Results"));
const ElectionOfficers = React.lazy(() => import("./ElectionOfficers"));
const Voters = React.lazy(() => import("./Voters"));
const ChangePassword = React.lazy(() => import("./ChangePassword"));
const PendingUsers = React.lazy(() => import("./PendingUsers"));
const Parties = React.lazy(() => import("./Parties"));

declare global {
  interface Window {
    ethereum?: any;
  }
}

type DashboardPage =
  | "elections"
  | "candidates"
  | "vote"
  | "results"
  | "officers"
  | "voters"
  | "change-password"
  | "parties"
  | "pending-users";

export default function Dashboard() {
  const navigate = useNavigate();
  const { showToast } = useToast();

  const [page, setPage] =
    useState<DashboardPage>(
      "elections"
    );
  const [visitedPages, setVisitedPages] = useState<Set<string>>(new Set(["elections"]));

  useEffect(() => {
    setVisitedPages(prev => {
      if (prev.has(page)) return prev;
      const next = new Set(prev);
      next.add(page);
      return next;
    });
  }, [page]);
  const [wallet, setWallet] = useState("");
  const walletRef = React.useRef(wallet);

  useEffect(() => {
    walletRef.current = wallet;
  }, [wallet]);
  const [userName, setUserName] =
    useState("");
  const [role, setRole] =
    useState("");
  const [electionsCount, setElectionsCount] =
    useState(0);
  const [candidatesCount, setCandidatesCount] =
    useState(0);
  const [votersCount, setVotersCount] =
    useState(0);
  const [blockchainStatus, setBlockchainStatus] =
    useState<{ ok: boolean; label: string; sub: string }>({
      ok: false,
      label: "Checking...",
      sub: "Detecting Wallet",
    });
  const [apiStatus, setApiStatus] =
    useState<{ ok: boolean; label: string; sub: string }>({
      ok: false,
      label: "Checking...",
      sub: "Connecting to Backend",
    });

  useEffect(() => {
    const token =
      localStorage.getItem("token");

    if (!token) {
      navigate("/login");
      return;
    }

    const storedRole =
      localStorage.getItem("role") ||
      "Voter";

    setRole(storedRole);
    setUserName(
      localStorage.getItem("fullName") ||
        "User"
    );

    const storedWallet =
      localStorage.getItem("walletAddress") ||
      "";
    setWallet(storedWallet);
  }, [navigate]);

  useEffect(() => {
    if (role) {
      loadStats();
    }
  }, [role]);

  const loadStats = async () => {
    try {
      const elections =
        await api.get("/elections");
      const electionList =
        elections.data.elections ?? [];

      const canManage =
        role === "Admin" ||
        role === "ElectionOfficer";

      const visibleElections =
        electionList.filter(
          (e: any) => {
            const s =
              typeof e.status ===
              "number"
                ? [
                    "Draft",
                    "Active",
                    "Closed",
                  ][e.status] ??
                  "Draft"
                : e.status;

            if (canManage)
              return true;

            return s === "Active";
          }
        );

      setElectionsCount(
        visibleElections.length
      );

      const draftOrActiveElections =
        visibleElections.filter(
          (e: any) => {
            const s =
              typeof e.status ===
              "number"
                ? [
                    "Draft",
                    "Active",
                    "Closed",
                  ][e.status] ??
                  "Draft"
                : e.status;
            return (
              s === "Draft" ||
              s === "Active"
            );
          }
        );

      setCandidatesCount(
        draftOrActiveElections.reduce(
          (sum: number, election: any) =>
            sum + (election.candidateCount ?? 0),
          0
        )
      );

      // Fetch total registered voters for non-voters
      if (role && role !== "Voter") {
        const usersRes = await api.get("/users");
        const usersList = usersRes.data.message ?? [];
        setVotersCount(usersList.length);
      }
    } catch (error) {
      console.error(error);
    }
  };

  // Check blockchain (MetaMask) connectivity
  useEffect(() => {
    const checkBlockchain = async () => {
      try {
        if (!window.ethereum) {
          setBlockchainStatus({
            ok: false,
            label: "No Wallet",
            sub: "Install MetaMask",
          });
          return;
        }

        const accounts: string[] = await window.ethereum.request({
          method: "eth_accounts",
        });

        if (accounts && accounts.length > 0) {
          const address = accounts[0];
          setBlockchainStatus({
            ok: true,
            label: "Connected",
            sub: `${address.slice(0, 6)}...${address.slice(-4)}`,
          });
          if (walletRef.current !== address) {
            setWallet(address);
            localStorage.setItem("walletAddress", address);
          }
        } else {
          setBlockchainStatus({
            ok: true,
            label: "Ready",
            sub: "Wallet Not Connected",
          });
          if (walletRef.current !== "") {
            setWallet("");
            localStorage.removeItem("walletAddress");
          }
        }
      } catch {
        setBlockchainStatus({
          ok: false,
          label: "Error",
          sub: "Wallet Check Failed",
        });
      }
    };

    checkBlockchain();

    if (window.ethereum) {
      const handleAccounts = (accounts: string[]) => {
        if (accounts.length > 0) {
          const address = accounts[0];
          setWallet(address);
          localStorage.setItem("walletAddress", address);
          setBlockchainStatus({
            ok: true,
            label: "Connected",
            sub: `${address.slice(0, 6)}...${address.slice(-4)}`,
          });
        } else {
          setWallet("");
          localStorage.removeItem("walletAddress");
          setBlockchainStatus({
            ok: true,
            label: "Ready",
            sub: "Wallet Not Connected",
          });
        }
      };

      window.ethereum.on("accountsChanged", handleAccounts);
      return () => {
        window.ethereum.removeListener("accountsChanged", handleAccounts);
      };
    }
  }, []);

  // Check API health
  useEffect(() => {
    const checkApi = async () => {
      try {
        await api.get("/elections");
        setApiStatus({
          ok: true,
          label: "Online",
          sub: "ASP.NET Backend",
        });
      } catch {
        setApiStatus({
          ok: false,
          label: "Offline",
          sub: "Cannot Reach Backend",
        });
      }
    };

    checkApi();
  }, []);

  const connect = async () => {
    try {
      if (!window.ethereum) {
        showToast(
          "MetaMask is not installed. Please install MetaMask.",
          "warning"
        );
        return;
      }

      const accounts =
        await window.ethereum.request({
          method:
            "eth_requestAccounts",
        });

      if (
        accounts &&
        accounts.length > 0
      ) {
        const address = accounts[0];
        setWallet(address);
        localStorage.setItem(
          "walletAddress",
          address
        );

        if (role === "Voter") {
          await api.post(
            `/users/connect-wallet?ethAddress=${encodeURIComponent(
              address
            )}`
          );
        }

        showToast(
          "Wallet Connected Successfully",
          "success"
        );
      }
    } catch (error: any) {
      console.error(error);
      showToast(
        error?.response?.data
          ?.message ||
          "Failed to connect wallet",
        "error"
      );
    }
  };

  const disconnectWallet = async () => {
    try {
      // Revoke MetaMask permissions so it fully disconnects
      if (window.ethereum) {
        await window.ethereum.request({
          method: "wallet_revokePermissions",
          params: [
            { eth_accounts: {} },
          ],
        });
      }
    } catch (err) {
      // Some wallets don't support revokePermissions, that's okay
      console.warn(
        "wallet_revokePermissions not supported:",
        err
      );
    }

    setWallet("");
    localStorage.removeItem(
      "walletAddress"
    );
    showToast("Wallet Disconnected", "info");
  };

  const logout = () => {
    clearSession();
    navigate("/login");
  };

  return (
    <div className="flex min-h-screen bg-slate-950 text-white">
      <div className="w-72 bg-slate-900 border-r border-slate-800 p-6">
        <h1 className="text-5xl font-bold mb-6 text-cyan-400">
          BVS
        </h1>

        <div className="bg-slate-800 rounded-xl p-4 mb-6">
          <div className="flex items-center gap-3">
            <FaUser className="text-cyan-400" />

            <div>
              <p className="text-sm text-slate-400">
                Logged in as
              </p>

              <p className="font-semibold">
                {userName}
              </p>

              <p className="text-xs text-cyan-400">
                {role}
              </p>
            </div>
          </div>
        </div>

        <div className="space-y-3">
          <button
            onClick={() =>
              setPage("elections")
            }
            className="w-full flex items-center gap-3 p-3 rounded-xl bg-slate-800 hover:bg-cyan-500 hover:text-black"
          >
            <FaUniversity />
            Elections
          </button>

          <button
            onClick={() =>
              setPage("candidates")
            }
            className="w-full flex items-center gap-3 p-3 rounded-xl bg-slate-800 hover:bg-cyan-500 hover:text-black"
          >
            <FaUsers />
            Candidates
          </button>

          {(role === "Admin" ||
            role ===
              "ElectionOfficer") && (
            <button
              onClick={() =>
                setPage("voters")
              }
              className="w-full flex items-center gap-3 p-3 rounded-xl bg-slate-800 hover:bg-cyan-500 hover:text-black"
            >
              <FaUsers />
              Voters
            </button>
          )}
          {(role === "Admin" ||
            role ===
              "ElectionOfficer") && (
            <button
              onClick={() =>
                setPage("parties")
              }
              className="w-full flex items-center gap-3 p-3 rounded-xl bg-slate-800 hover:bg-cyan-500 hover:text-black"
            >
              <FaFlag />
              Parties
            </button>
          )}
          {(role === "Admin" ||
            role === "ElectionOfficer" ||
            role === "Party") && (
            <button
              onClick={() =>
                setPage("pending-users")
              }
              className="w-full flex items-center gap-3 p-3 rounded-xl bg-slate-800 hover:bg-cyan-500 hover:text-black"
            >
              <FaCheckCircle />
              Verification
            </button>
          )}

          {role === "Voter" && (
            <button
              onClick={() =>
                setPage("vote")
              }
              className="w-full flex items-center gap-3 p-3 rounded-xl bg-slate-800 hover:bg-cyan-500 hover:text-black"
            >
              <FaVoteYea />
              Voting
            </button>
          )}

          <button
            onClick={() =>
              setPage("results")
            }
            className="w-full flex items-center gap-3 p-3 rounded-xl bg-slate-800 hover:bg-cyan-500 hover:text-black"
          >
            <FaChartBar />
            Results
          </button>

          {role === "Admin" && (
            <button
              onClick={() =>
                setPage("officers")
              }
              className="w-full flex items-center gap-3 p-3 rounded-xl bg-slate-800 hover:bg-cyan-500 hover:text-black"
            >
              <FaUserShield />
              Officers
            </button>
          )}

          {role === "Voter" && (
            <>
              {!wallet ? (
                <button
                  onClick={connect}
                  className="w-full flex items-center gap-3 p-3 rounded-xl bg-cyan-500 text-black font-bold"
                >
                  <FaWallet />
                  Connect Wallet
                </button>
              ) : (
                <button
                  onClick={
                    disconnectWallet
                  }
                  className="w-full flex items-center gap-3 p-3 rounded-xl bg-orange-500 text-black font-bold"
                >
                  <FaWallet />
                  Disconnect Wallet
                </button>
              )}
            </>
          )}

          <button
            onClick={() =>
              setPage("change-password")
            }
            className="w-full flex items-center gap-3 p-3 rounded-xl bg-slate-800 hover:bg-cyan-500 hover:text-black"
          >
            <FaKey />
            Change Password
          </button>

          <button
            onClick={logout}
            className="w-full flex items-center gap-3 p-3 rounded-xl bg-red-600 hover:bg-red-500 text-white font-bold"
          >
            <FaSignOutAlt />
            Logout
          </button>

          {role === "Voter" && wallet && (
            <div className="mt-4 bg-slate-800 p-3 rounded-xl">
              <p className="text-green-400 text-xs font-bold">
                Wallet Connected
              </p>

              <p className="text-slate-300 text-xs break-all mt-1">
                {wallet}
              </p>
            </div>
          )}
        </div>
      </div>

      <div className="flex-1 p-8">
        <div className="grid gap-5 mb-8 md:grid-cols-2 xl:grid-cols-4">
          <div className="bg-slate-900 rounded-2xl p-5">
            <h3 className="text-slate-400">
              Elections
            </h3>

            <p className="text-3xl font-bold text-cyan-400">
              {electionsCount}
            </p>

            <p className="text-xs text-slate-500">
              Elections Available
            </p>
          </div>

          <div className="bg-slate-900 rounded-2xl p-5">
            <h3 className="text-slate-400">
              Candidates
            </h3>

            <p className="text-3xl font-bold text-cyan-400">
              {candidatesCount}
            </p>

            <p className="text-xs text-slate-500">
              Registered Candidates
            </p>
          </div>

          {role === "Voter" ? (
            <div className="bg-slate-900 rounded-2xl p-5">
              <div className="flex items-center gap-2">
                <FaLink className={blockchainStatus.ok ? "text-green-400" : "text-red-400"} />
                <h3 className="text-slate-400">
                  Blockchain
                </h3>
              </div>

              <p className={`text-xl font-bold mt-2 ${blockchainStatus.ok ? "text-green-400" : "text-red-400"}`}>
                {blockchainStatus.label}
              </p>

              <p className="text-xs text-slate-500">
                {blockchainStatus.sub}
              </p>
            </div>
          ) : (
            <div className="bg-slate-900 rounded-2xl p-5">
              <h3 className="text-slate-400">
                Voters
              </h3>

              <p className="text-3xl font-bold text-cyan-400">
                {votersCount}
              </p>

              <p className="text-xs text-slate-500">
                Registered Voters
              </p>
            </div>
          )}

          <div className="bg-slate-900 rounded-2xl p-5">
            <div className="flex items-center gap-2">
              <FaServer className={apiStatus.ok ? "text-green-400" : "text-red-400"} />
              <h3 className="text-slate-400">
                API
              </h3>
            </div>

            <p className={`text-xl font-bold mt-2 ${apiStatus.ok ? "text-green-400" : "text-red-400"}`}>
              {apiStatus.label}
            </p>

            <p className="text-xs text-slate-500">
              {apiStatus.sub}
            </p>
          </div>
        </div>

        <Suspense fallback={<div className="text-sky-400 p-4">Loading tab...</div>}>
          <div style={{ display: page === "elections" ? "block" : "none" }}>
            {visitedPages.has("elections") && <Elections />}
          </div>
          <div style={{ display: page === "candidates" ? "block" : "none" }}>
            {visitedPages.has("candidates") && <Candidates />}
          </div>
          <div style={{ display: page === "voters" ? "block" : "none" }}>
            {visitedPages.has("voters") && <Voters />}
          </div>
          {role === "Voter" && (
            <div style={{ display: page === "vote" ? "block" : "none" }}>
              {visitedPages.has("vote") && <Vote />}
            </div>
          )}
          <div style={{ display: page === "results" ? "block" : "none" }}>
            {visitedPages.has("results") && <Results />}
          </div>
          <div style={{ display: page === "officers" ? "block" : "none" }}>
            {visitedPages.has("officers") && <ElectionOfficers />}
          </div>
          <div style={{ display: page === "parties" ? "block" : "none" }}>
            {visitedPages.has("parties") && <Parties />}
          </div>
          <div style={{ display: page === "pending-users" ? "block" : "none" }}>
            {visitedPages.has("pending-users") && <PendingUsers />}
          </div>
          <div style={{ display: page === "change-password" ? "block" : "none" }}>
            {visitedPages.has("change-password") && <ChangePassword />}
          </div>
        </Suspense>
      </div>
    </div>
  );
}
