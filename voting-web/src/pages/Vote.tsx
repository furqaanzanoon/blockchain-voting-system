import { useEffect, useState, useRef } from "react";
import { ethers } from "ethers";
import api from "../services/api";
// @ts-ignore
import * as snarkjs from "snarkjs";
// @ts-ignore
import { buildPoseidon } from "circomlibjs";
import {
  FaVoteYea,
  FaUserTie,
  FaUniversity,
  FaCheckCircle,
  FaMicrophone,
  FaMicrophoneSlash,
  FaExternalLinkAlt,
} from "react-icons/fa";
import { useToast } from "../context/ToastContext";
import { normalizeStatus } from "../utils/normalizeStatus";

interface Candidate {
  candidateId: string;
  candidateName: string;
  partyAffiliation?: string;
  description?: string;
  onChainIndex?: number;
}

interface Election {
  electionId: string;
  title: string;
  status: string | number;
  hasVoted: boolean;
  endTime: string;
  contractAddress: string;
}

interface VoteReceipt {
  txHash: string;
  blockNumber: number | null;
  votedAt: string;
  candidateName: string;
  electionTitle: string;
  contractAddress: string;
}


const normalizeElection = (e: any): Election => ({
  electionId: e.electionId ?? e.ElectionId ?? "",
  title: e.title ?? e.Title ?? "",
  status: e.status ?? e.Status ?? 0,
  hasVoted: e.hasVoted ?? e.HasVoted ?? false,
  endTime: e.endTime ?? e.EndTime ?? "",
  contractAddress: e.contractAddress ?? e.ContractAddress ?? "",
});

export default function Vote() {
  const [candidates, setCandidates] =
    useState<Candidate[]>([]);
  const [elections, setElections] =
    useState<Election[]>([]);
  const [selectedElection, setSelectedElection] =
    useState("");
  const [loadingCandidateId, setLoadingCandidateId] =
    useState("");

  const { showToast } = useToast();
  const [sendingOtpCandidateId, setSendingOtpCandidateId] =
    useState("");
  const [pendingVoteCandidateId, setPendingVoteCandidateId] =
    useState("");
  const [otp, setOtp] = useState("");
  const [activeVoiceCandidateId, setActiveVoiceCandidateId] = useState("");
  const [voteReceipt, setVoteReceipt] = useState<VoteReceipt | null>(null);
  const [loadingReceipt, setLoadingReceipt] = useState(false);

  const handleVoiceCancel = () => {
    setActiveVoiceCandidateId("");
    setPendingVoteCandidateId("");
    setOtp("");
  };

  const handleSelectCandidate = (candidateId: string) => {
    setActiveVoiceCandidateId(candidateId);
    setTimeout(() => {
      const el = document.getElementById(`candidate-card-${candidateId}`);
      if (el) {
        el.scrollIntoView({ behavior: "smooth", block: "center" });
      }
    }, 100);
  };

  const [isListening, setIsListening] = useState(false);
  const [recognition, setRecognition] = useState<any>(null);

  const handleVoiceCommandRef = useRef<any>(null);

  useEffect(() => {
    handleVoiceCommandRef.current = handleVoiceCommand;
  }, [candidates]);

  useEffect(() => {
    const SpeechRecognition =
      (window as any).SpeechRecognition || (window as any).webkitSpeechRecognition;

    let rec: any = null;
    if (SpeechRecognition) {
      rec = new SpeechRecognition();
      rec.continuous = false;
      rec.interimResults = false;
      rec.lang = "en-IN";

      rec.onstart = () => {
        setIsListening(true);
      };

      rec.onerror = (e: any) => {
        console.error("Speech recognition error", e);
        setIsListening(false);
        if (e.error === "not-allowed") {
          showToast("Microphone access denied. Please check your browser permissions.", "error");
        }
      };

      rec.onend = () => {
        setIsListening(false);
      };

      rec.onresult = (event: any) => {
        const resultText = event.results[0][0].transcript.trim();
        if (handleVoiceCommandRef.current) {
          handleVoiceCommandRef.current(resultText);
        }
      };

      setRecognition(rec);
    }

    return () => {
      if (rec) {
        try {
          rec.abort();
        } catch (err) {
          console.error(err);
        }
      }
    };
  }, []);

  const handleVoiceCommand = (command: string) => {
    const cleanCommand = command.toLowerCase().trim();
    
    const isVotePhrase = 
      cleanCommand.includes("vote") || 
      cleanCommand.includes("वोट") || 
      cleanCommand.includes("mera") || 
      cleanCommand.includes("मेरा") || 
      cleanCommand.includes("ko") || 
      cleanCommand.includes("को");

    if (!isVotePhrase) {
      showToast(`Command not recognized: "${command}". Please speak using a standard format like "Mera vote [Candidate Name] ko".`, "warning");
      return;
    }

    let namePart = cleanCommand;
    const stopWords = ["mera", "मेरा", "vote", "वोट", "ko", "को", "for", "select"];
    for (const word of stopWords) {
      namePart = namePart.replace(new RegExp(`\\b${word}\\b`, "g"), "");
    }
    namePart = namePart.trim();

    const devanagariMap: { [key: string]: string } = {
      "हनान": "hanan",
      "रवि": "ravi",
      "अमित": "amit",
      "राहुल": "rahul",
      "प्रिया": "priya",
      "विजय": "vijay",
    };

    for (const [hindiName, engName] of Object.entries(devanagariMap)) {
      if (namePart.includes(hindiName)) {
        namePart = namePart.replace(hindiName, engName);
      }
    }

    let matchedCandidate = candidates.find(c => {
      const name = c.candidateName.toLowerCase();
      return name.includes(namePart) || namePart.includes(name);
    });

    if (!matchedCandidate && namePart.length > 2) {
      matchedCandidate = candidates.find(c => {
        const nameParts = c.candidateName.toLowerCase().split(/\s+/);
        return nameParts.some(part => part.includes(namePart) || namePart.includes(part));
      });
    }

    if (matchedCandidate) {
      showToast(`Mera vote ${matchedCandidate.candidateName} ko: Casting vote...`, "success");
      
      handleSelectCandidate(matchedCandidate.candidateId);

      requestVoteOtp(matchedCandidate.candidateId);
    } else {
      showToast(`Candidate "${namePart}" not found.`, "error");
    }
  };

  const toggleVoiceListening = () => {
    if (!recognition) {
      showToast("Speech recognition is not supported in this browser. Please use Chrome or Edge.", "warning");
      return;
    }

    if (isListening) {
      recognition.stop();
    } else {
      try {
        recognition.start();
      } catch (err) {
        console.error(err);
      }
    }
  };

  useEffect(() => {
    loadElections();
  }, []);

  useEffect(() => {
    if (selectedElection) {
      loadCandidates(
        selectedElection
      );
    }
  }, [selectedElection]);

  const loadElections = async () => {
    try {
      const res =
        await api.get("/elections");
      const allElections: Election[] =
        (res.data.elections ?? []).map(normalizeElection);

      // Only show active elections (not closed, not draft) and whose closing time has not passed
      const list = allElections.filter((e) => {
        const isStatusActive = normalizeStatus(e.status) === "Active";
        const rawEndTime = e.endTime;
        const endTimeStr = typeof rawEndTime === "string" && !/Z|[+-]\d{2}:\d{2}$/.test(rawEndTime)
          ? rawEndTime + "Z"
          : rawEndTime;
        const isNotEnded = new Date(endTimeStr) > new Date();
        return isStatusActive && isNotEnded;
      });

      setElections(list);

      if (list.length > 0) {
        setSelectedElection(
          list[0].electionId
        );
      } else {
        setSelectedElection("");
        setCandidates([]);
      }
    } catch (err) {
      console.error(err);
    }
  };

  const loadCandidates = async (
    electionId: string
  ) => {
    try {
      const res = await api.get(
        `/elections/${electionId}/candidates`
      );

      setCandidates(res.data);
    } catch (err) {
      console.error(err);
      setCandidates([]);
    }
  };

  // Check if the currently selected election has been voted
  const currentElection = elections.find(
    (e) => e.electionId === selectedElection
  );
  const hasVotedInSelected = currentElection?.hasVoted ?? false;

  // Fetch vote receipt when selecting an election the user has voted in
  useEffect(() => {
    if (hasVotedInSelected && selectedElection) {
      fetchVoteReceipt(selectedElection);
    } else {
      setVoteReceipt(null);
    }
  }, [selectedElection, hasVotedInSelected]);

  const fetchVoteReceipt = async (electionId: string) => {
    try {
      setLoadingReceipt(true);
      const res = await api.get(`/vote/receipt/${electionId}`);
      setVoteReceipt(res.data);
    } catch {
      setVoteReceipt(null);
    } finally {
      setLoadingReceipt(false);
    }
  };

  const requestVoteOtp = async (
    candidateId: string
  ) => {
    try {
      setSendingOtpCandidateId(
        candidateId
      );

      await api.post(
        "/vote/send-otp",
        {
          electionId:
            selectedElection,
          candidateId,
        }
      );

      setPendingVoteCandidateId(
        candidateId
      );
      setOtp("");

      showToast(
        "OTP sent to your registered email.",
        "info"
      );
    } catch (err: any) {
      console.error(err);

      const message =
        err?.response?.data
          ?.message ||
        "Failed to send vote OTP";

      showToast(
        message ===
          "Not registered for election"
          ? "You are not registered for this election. Ask an admin or election officer to register you before voting."
          : message,
        "error"
      );
    } finally {
      setSendingOtpCandidateId("");
    }
  };

  const castVote = async () => {
    if (!pendingVoteCandidateId) {
      return;
    }

    try {
      if (!window.ethereum) {
        showToast(
          "Please install MetaMask to vote.",
          "warning"
        );
        return;
      }

      setLoadingCandidateId(
        pendingVoteCandidateId
      );

      const currentElection = elections.find(e => e.electionId === selectedElection);
      if (!currentElection) {
        showToast("Election not found", "error");
        return;
      }
      if (!currentElection.contractAddress) {
        showToast("Election not deployed on chain", "error");
        return;
      }

      const currentCandidate = candidates.find(c => c.candidateId === pendingVoteCandidateId);
      if (!currentCandidate) {
        showToast("Candidate not found", "error");
        return;
      }
      if (currentCandidate.onChainIndex === undefined || currentCandidate.onChainIndex === null) {
        showToast("Candidate not mapped on-chain", "error");
        return;
      }

      let provider = new ethers.BrowserProvider(window.ethereum);
      let signer = await provider.getSigner();
      let network = await provider.getNetwork();
      let chainId = Number(network.chainId);

      const targetChainId = 11155111; // Sepolia
      const targetChainIdHex = "0x" + targetChainId.toString(16);

      if (chainId !== targetChainId) {
        try {
          await window.ethereum.request({
            method: "wallet_switchEthereumChain",
            params: [{ chainId: targetChainIdHex }],
          });
          provider = new ethers.BrowserProvider(window.ethereum);
          signer = await provider.getSigner();
          network = await provider.getNetwork();
          chainId = Number(network.chainId);
        } catch (switchError: any) {
          // This error code indicates that the chain has not been added to MetaMask.
          if (switchError.code === 4902) {
            try {
              await window.ethereum.request({
                method: "wallet_addEthereumChain",
                params: [
                  {
                    chainId: targetChainIdHex,
                    chainName: "Sepolia Test Network",
                    rpcUrls: ["https://ethereum-sepolia-rpc.publicnode.com"],
                    nativeCurrency: {
                      name: "Sepolia Ether",
                      symbol: "ETH",
                      decimals: 18,
                    },
                    blockExplorerUrls: ["https://sepolia.etherscan.io"],
                  },
                ],
              });
              provider = new ethers.BrowserProvider(window.ethereum);
              signer = await provider.getSigner();
              network = await provider.getNetwork();
              chainId = Number(network.chainId);
            } catch (addError) {
              showToast("Failed to add Sepolia network to MetaMask.", "error");
              return;
            }
          } else {
            showToast("Failed to switch network to Sepolia.", "error");
            return;
          }
        }
      }

      if (chainId !== targetChainId) {
        showToast("Please connect to the Sepolia network to cast your vote.", "error");
        return;
      }

      const voterAddress = await signer.getAddress();

      // Fetch voter nonce from backend
      const nonceRes = await api.get(`/vote/nonce/${selectedElection}`);
      const registeredAddress = nonceRes.data.registeredAddress;

      if (registeredAddress && voterAddress.toLowerCase() !== registeredAddress.toLowerCase()) {
        showToast(
          `Your active MetaMask account (${voterAddress.slice(0, 6)}...${voterAddress.slice(-4)}) does not match your registered voting address (${registeredAddress.slice(0, 6)}...${registeredAddress.slice(-4)}). Please switch MetaMask to your registered account.`,
          "error"
        );
        return;
      }

      // Construct ZK proof inputs and signals using Poseidon and SnarkJS
      showToast("Generating Zero-Knowledge proof locally in your browser. Please wait...", "info");

      // @ts-ignore
      const poseidon = await buildPoseidon();

      // Convert voter address to field elements / bigints
      const addressBigInt = BigInt(voterAddress);

      // Derive secret and salt deterministically from the wallet address
      const secret = poseidon.F.toObject(poseidon([addressBigInt, BigInt(1)])).toString();
      const salt = poseidon.F.toObject(poseidon([addressBigInt, BigInt(2)])).toString();

      // Calculate identity commitment: leaf = Poseidon(secret, salt)
      const leafBigInt = poseidon.F.toObject(poseidon([BigInt(secret), BigInt(salt)]));
      const leafStr = leafBigInt.toString();

      // Create a local Merkle tree of depth 3 (8 leaves)
      const leaves: string[] = [leafStr];
      for (let i = 1; i < 8; i++) {
        const dummyLeaf = poseidon.F.toObject(poseidon([BigInt(i), BigInt(i)])).toString();
        leaves.push(dummyLeaf);
      }

      // Compute tree levels
      const level0 = leaves.map(x => BigInt(x));
      
      const level1: bigint[] = [];
      for (let i = 0; i < 8; i += 2) {
        const hash = poseidon.F.toObject(poseidon([level0[i], level0[i+1]]));
        level1.push(hash);
      }

      const level2: bigint[] = [];
      for (let i = 0; i < 4; i += 2) {
        const hash = poseidon.F.toObject(poseidon([level1[i], level1[i+1]]));
        level2.push(hash);
      }

      const rootBigInt = poseidon.F.toObject(poseidon([level2[0], level2[1]]));
      const merkleRoot = rootBigInt.toString();

      // Compute sibling path for leaf index 0 (indices = ["0", "0", "0"])
      const pathElements = [
        level0[1].toString(),
        level1[1].toString(),
        level2[1].toString()
      ];
      const pathIndices = ["0", "0", "0"];

      // Nullifier: Poseidon(secret, ballotId)
      const ballotIdBigInt = BigInt(ethers.solidityPackedKeccak256(["string"], [selectedElection])) % BigInt("21888242871839275222246405745257275088548364400416034343698204186575808495617");
      const ballotId = ballotIdBigInt.toString();

      const nullifierHash = poseidon.F.toObject(poseidon([BigInt(secret), ballotIdBigInt])).toString();

      // Vote commitment: Poseidon(candidateId, secret)
      const candidateIdBigInt = BigInt(currentCandidate.onChainIndex ?? 0);
      const voteCommitment = poseidon.F.toObject(poseidon([candidateIdBigInt, BigInt(secret)])).toString();

      // Construct circuit inputs
      const inputs = {
        merkleRoot,
        nullifierHash,
        ballotId,
        voteCommitment,
        secret,
        salt,
        pathElements,
        pathIndices,
        candidateId: candidateIdBigInt.toString()
      };

      console.log("Proving with inputs:", inputs);

      // Generate the real Groth16 proof using snarkjs
      // @ts-ignore
      const { proof: fullProof, publicSignals } = await snarkjs.groth16.fullProve(
        inputs,
        "/vote.wasm",
        "/vote_final.zkey"
      );

      console.log("Proof generated!", fullProof, publicSignals);

      // Reformat fullProof to match expected API structure:
      const proof = {
        a: {
          x: fullProof.pi_a[0],
          y: fullProof.pi_a[1]
        },
        b: {
          x: [fullProof.pi_b[0][1], fullProof.pi_b[0][0]],
          y: [fullProof.pi_b[1][1], fullProof.pi_b[1][0]]
        },
        c: {
          x: fullProof.pi_c[0],
          y: fullProof.pi_c[1]
        }
      };

      // Submit the vote via the ZK-SNARK pipeline
      const voteRes = await api.post(
        "/vote/zk",
        {
          electionId: selectedElection,
          candidateId: pendingVoteCandidateId,
          otp,
          proof,
          publicSignals: {
            merkleRoot,
            nullifierHash,
            ballotId,
            voteCommitment
          }
        }
      );

      // Extract receipt data from the response
      const responseData = voteRes.data;
      if (responseData?.txHash) {
        setVoteReceipt({
          txHash: responseData.txHash,
          blockNumber: responseData.blockNumber,
          votedAt: new Date().toISOString(),
          candidateName: currentCandidate?.candidateName ?? "",
          electionTitle: currentElection?.title ?? "",
          contractAddress: currentElection?.contractAddress ?? "",
        });
      }

      showToast(
        "Vote cast successfully! You can verify it on the blockchain.",
        "success"
      );

      // Mark this election as voted in local state
      setElections((prev) =>
        prev.map((e) =>
          e.electionId === selectedElection
            ? { ...e, hasVoted: true }
            : e
        )
      );

      handleVoiceCancel();
    } catch (err: any) {
      console.error(err);

      // Check if user rejected the MetaMask signature request
      const isUserRejection =
        err?.code === 4001 ||
        err?.code === "ACTION_REJECTED" ||
        err?.message?.includes("user rejected") ||
        err?.message?.includes("User denied");

      showToast(
        isUserRejection
          ? "Signature request was cancelled."
          : err?.response?.data
              ?.message ||
            err?.shortMessage ||
            err?.message ||
            "Vote failed",
        isUserRejection ? "warning" : "error"
      );
    } finally {
      setLoadingCandidateId("");
    }
  };

  const submitVerifiedVote = async (
    event: React.FormEvent
  ) => {
    event.preventDefault();
    try {
      await castVote();
    } catch {
      // Error already handled in castVote
    }
  };

  return (
    <div>
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-8">
        <h1 className="text-4xl font-bold">
          Cast Your Vote
        </h1>

        <div className="flex items-center gap-3.5 bg-slate-900 border border-slate-750 rounded-2xl px-5 py-3 shadow-lg">
          <button
            onClick={toggleVoiceListening}
            className={`w-11 h-11 rounded-full flex items-center justify-center transition-all duration-300 relative ${
              isListening
                ? "bg-rose-500 text-white animate-pulse shadow-lg shadow-rose-500/25"
                : "bg-slate-800 text-slate-400 hover:bg-slate-700 hover:text-white"
            }`}
            title="Vote using voice (Hindi/English)"
          >
            {isListening ? (
              <>
                <span className="absolute -inset-1 rounded-full bg-rose-500/20 animate-ping"></span>
                <FaMicrophone size={18} />
              </>
            ) : (
              <FaMicrophoneSlash size={18} />
            )}
          </button>
          <span className="text-sm font-semibold text-slate-300">
            {isListening ? "Listening..." : "Vote by Voice"}
          </span>
        </div>
      </div>

      <div className="bg-slate-900 rounded-3xl p-6 mb-8">
        <div className="flex items-center gap-3 mb-4">
          <FaUniversity
            className="text-cyan-400"
            size={24}
          />

          <h2 className="text-xl font-bold">
            Select Election
          </h2>
        </div>

        <select
          value={selectedElection}
          onChange={(e) =>
            setSelectedElection(
              e.target.value
            )
          }
          className="w-full bg-slate-800 border border-slate-700 rounded-xl p-3"
        >
          {elections.length === 0 && (
            <option value="">
              No active elections
            </option>
          )}
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
                {election.hasVoted
                  ? " (Voted)"
                  : ""}
              </option>
            )
          )}
        </select>
      </div>

      {hasVotedInSelected && (
        <div className="bg-green-500/10 border border-green-500/30 rounded-3xl p-6 mb-8">
          <div className="flex items-center gap-4 mb-4">
            <FaCheckCircle
              className="text-green-400"
              size={28}
            />
            <div>
              <h3 className="text-lg font-bold text-green-400">
                You have voted successfully in this election
              </h3>
              <p className="text-slate-400 text-sm">
                Your vote has been recorded on the blockchain.
              </p>
            </div>
          </div>

          {loadingReceipt && (
            <div className="text-slate-400 text-sm animate-pulse mt-2">
              Loading blockchain receipt...
            </div>
          )}

          {voteReceipt && (
            <div className="mt-4 bg-slate-900/60 rounded-2xl p-5 border border-slate-700">
              <h4 className="text-sm font-bold text-cyan-400 uppercase tracking-wider mb-3">
                🔗 Blockchain Vote Receipt
              </h4>

              <div className="space-y-2 text-sm">
                <div className="flex flex-col sm:flex-row sm:items-center gap-1">
                  <span className="text-slate-400 font-medium min-w-[120px]">Transaction:</span>
                  <div className="flex items-center gap-2 min-w-0">
                    <code className="text-green-400 text-xs break-all">
                      {voteReceipt.txHash}
                    </code>
                    <a
                      href={`https://sepolia.etherscan.io/tx/${voteReceipt.txHash}`}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="text-cyan-400 hover:text-cyan-300 flex-shrink-0 transition-colors"
                      title="View on Etherscan"
                    >
                      <FaExternalLinkAlt size={12} />
                    </a>
                  </div>
                </div>

                {voteReceipt.blockNumber && (
                  <div className="flex flex-col sm:flex-row sm:items-center gap-1">
                    <span className="text-slate-400 font-medium min-w-[120px]">Block:</span>
                    <a
                      href={`https://sepolia.etherscan.io/block/${voteReceipt.blockNumber}`}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="text-cyan-400 hover:text-cyan-300 transition-colors"
                    >
                      #{voteReceipt.blockNumber}
                    </a>
                  </div>
                )}

                {voteReceipt.candidateName && (
                  <div className="flex flex-col sm:flex-row sm:items-center gap-1">
                    <span className="text-slate-400 font-medium min-w-[120px]">Voted For:</span>
                    <span className="text-white font-semibold">{voteReceipt.candidateName}</span>
                  </div>
                )}

                {voteReceipt.votedAt && (
                  <div className="flex flex-col sm:flex-row sm:items-center gap-1">
                    <span className="text-slate-400 font-medium min-w-[120px]">Voted At:</span>
                    <span className="text-slate-300">
                      {new Date(voteReceipt.votedAt).toLocaleString("en-IN", { timeZone: "Asia/Kolkata" })}
                    </span>
                  </div>
                )}

                {voteReceipt.contractAddress && (
                  <div className="flex flex-col sm:flex-row sm:items-center gap-1">
                    <span className="text-slate-400 font-medium min-w-[120px]">Contract:</span>
                    <div className="flex items-center gap-2 min-w-0">
                      <code className="text-slate-300 text-xs break-all">
                        {voteReceipt.contractAddress}
                      </code>
                      <a
                        href={`https://sepolia.etherscan.io/address/${voteReceipt.contractAddress}`}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="text-cyan-400 hover:text-cyan-300 flex-shrink-0 transition-colors"
                        title="View contract on Etherscan"
                      >
                        <FaExternalLinkAlt size={12} />
                      </a>
                    </div>
                  </div>
                )}
              </div>

              <a
                href={`https://sepolia.etherscan.io/tx/${voteReceipt.txHash}`}
                target="_blank"
                rel="noopener noreferrer"
                className="mt-4 inline-flex items-center gap-2 bg-cyan-500/15 border border-cyan-500/30 text-cyan-400 hover:bg-cyan-500/25 font-semibold px-4 py-2 rounded-xl text-sm transition-colors"
              >
                <FaExternalLinkAlt size={12} />
                Verify on Etherscan
              </a>
            </div>
          )}
        </div>
      )}

      <div className="grid md:grid-cols-2 xl:grid-cols-3 gap-6">
        {candidates.map(
          (candidate) => {
            const isVoiceSelected = activeVoiceCandidateId === candidate.candidateId;
            return (
              <div
                key={candidate.candidateId}
                id={`candidate-card-${candidate.candidateId}`}
                className={`bg-slate-900 rounded-3xl p-6 shadow-lg transition-all duration-300 relative ${
                  isVoiceSelected
                    ? "ring-4 ring-cyan-400 bg-slate-850 scale-[1.03] shadow-cyan-500/10"
                    : "hover:scale-105"
                }`}
              >
                {isVoiceSelected && (
                  <span className="absolute top-4 right-4 bg-cyan-500 text-black text-[10px] font-bold px-2.5 py-1 rounded-full uppercase tracking-wider animate-pulse">
                    Voice Selected
                  </span>
                )}
                <div className="flex items-center gap-3 mb-4">
                  <FaUserTie
                    size={32}
                    className="text-cyan-400"
                  />

                  <div>
                    <h3 className="text-xl font-bold">
                      {
                        candidate.candidateName
                      }
                    </h3>

                    <p className="text-slate-400">
                      {candidate.partyAffiliation ||
                        "Independent"}
                    </p>
                  </div>
                </div>

                <p className="mb-4 text-slate-300">
                  {candidate.description ||
                    "No description provided."}
                </p>

                {hasVotedInSelected ? (
                  <div className="w-full bg-green-500/15 border border-green-500/30 text-green-400 font-bold py-3 rounded-xl flex items-center justify-center gap-2">
                    <FaCheckCircle />
                    Voted Successfully
                  </div>
                ) : (
                  <button
                    onClick={() =>
                      requestVoteOtp(
                        candidate.candidateId
                      )
                    }
                    disabled={
                      loadingCandidateId ===
                        candidate.candidateId ||
                      sendingOtpCandidateId ===
                      candidate.candidateId
                    }
                    className="w-full bg-cyan-500 hover:bg-cyan-400 text-black font-bold py-3 rounded-xl flex items-center justify-center gap-2"
                  >
                    <FaVoteYea />
                    {loadingCandidateId ===
                    candidate.candidateId
                      ? "Voting..."
                      : sendingOtpCandidateId ===
                        candidate.candidateId
                      ? "Sending OTP..."
                      : "Vote Now"}
                  </button>
                )}
              </div>
            );
          }
        )}
      </div>

      {pendingVoteCandidateId && (
        <div className="fixed inset-0 bg-black/70 flex items-center justify-center p-6 z-50">
          <div className="w-full max-w-md bg-slate-900 rounded-3xl p-6 border border-slate-700 shadow-2xl">
            <h2 className="text-2xl font-bold mb-4 text-cyan-400">
              Verify Vote OTP
            </h2>

            <form onSubmit={submitVerifiedVote}>
              <input
                type="text"
                required
                placeholder="Enter OTP"
                value={otp}
                onChange={(e) =>
                  setOtp(e.target.value)
                }
                className="w-full bg-slate-800 border border-slate-700 rounded-xl p-3 mb-4"
              />

              <div className="grid grid-cols-2 gap-3">
                <button
                  type="button"
                  onClick={handleVoiceCancel}
                  disabled={
                    Boolean(
                      loadingCandidateId
                    )
                  }
                  className="bg-slate-700 hover:bg-slate-600 text-white font-bold py-3 rounded-xl"
                >
                  Cancel
                </button>

                <button
                  type="submit"
                  disabled={
                    Boolean(
                      loadingCandidateId
                    )
                  }
                  className="bg-cyan-500 hover:bg-cyan-400 disabled:bg-slate-600 disabled:text-slate-300 text-black font-bold py-3 rounded-xl"
                >
                  {loadingCandidateId
                    ? "Voting..."
                    : "Verify & Vote"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {candidates.length === 0 && (
        <div className="bg-slate-900 rounded-3xl p-10 text-center text-slate-400 mt-6">
          No candidates found for this election.
        </div>
      )}
    </div>
  );
}

