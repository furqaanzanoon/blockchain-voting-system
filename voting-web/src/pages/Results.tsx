import { useEffect, useState, useMemo, lazy, Suspense } from "react";
import api from "../services/api";

const ResultsChart = lazy(() => import("../components/ResultsChart"));

import {
  FaTrophy,
  FaVoteYea,
  FaUsers,
  FaUniversity,
} from "react-icons/fa";

interface Election {
  electionId: string;
  title: string;
}

interface Result {
  candidate: string;
  votes: number;
}

export default function Results() {
  const [results, setResults] =
    useState<Result[]>([]);
  const [elections, setElections] =
    useState<Election[]>([]);
  const [selectedElection, setSelectedElection] =
    useState("");

  useEffect(() => {
    loadElections();
  }, []);

  useEffect(() => {
    if (selectedElection) {
      loadResults(selectedElection);
    }
  }, [selectedElection]);

  const loadElections = async () => {
    try {
      const res =
        await api.get("/elections");
      const list =
        res.data.elections ?? [];

      setElections(list);

      if (list.length > 0) {
        setSelectedElection(
          list[0].electionId
        );
      }
    } catch (err) {
      console.error(err);
    }
  };

  const loadResults = async (
    electionId: string
  ) => {
    try {
      const res = await api.get(
        `/results/${electionId}`
      );

      const formatted =
        res.data.map((item: any) => ({
          candidate:
            item.candidateName,
          votes:
            item.voteCount ?? 0,
        }));

      setResults(formatted);
    } catch (err) {
      console.error(err);
      setResults([]);
    }
  };

  const totalVotes = useMemo(() => {
    return results.reduce(
      (sum, item) => sum + item.votes,
      0
    );
  }, [results]);

  const winner = useMemo(() => {
    return results.length > 0
      ? results.reduce(
          (prev, current) =>
            prev.votes > current.votes ? prev : current
        )
      : null;
  }, [results]);

  return (
    <div>
      <h1 className="text-4xl font-bold mb-8">
        Election Results
      </h1>

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
      </div>

      <div className="grid md:grid-cols-3 gap-6 mb-8">
        <div className="bg-slate-900 rounded-3xl p-6">
          <div className="flex items-center gap-3 mb-3">
            <FaVoteYea
              className="text-cyan-400"
              size={28}
            />

            <h3 className="text-xl">
              Total Votes
            </h3>
          </div>

          <p className="text-4xl font-bold">
            {totalVotes}
          </p>
        </div>

        <div className="bg-slate-900 rounded-3xl p-6">
          <div className="flex items-center gap-3 mb-3">
            <FaUsers
              className="text-cyan-400"
              size={28}
            />

            <h3 className="text-xl">
              Candidates
            </h3>
          </div>

          <p className="text-4xl font-bold">
            {results.length}
          </p>
        </div>

        <div className="bg-slate-900 rounded-3xl p-6">
          <div className="flex items-center gap-3 mb-3">
            <FaTrophy
              className="text-yellow-400"
              size={28}
            />

            <h3 className="text-xl">
              Winner
            </h3>
          </div>

          <p className="text-xl font-bold text-green-400">
            {winner
              ? winner.candidate
              : "No Votes Yet"}
          </p>
        </div>
      </div>

      {winner && (
        <div className="bg-gradient-to-r from-yellow-500 to-orange-500 text-black rounded-3xl p-8 mb-8">
          <h2 className="text-3xl font-bold mb-3">
            Winning Candidate
          </h2>

          <p className="text-2xl font-semibold">
            {winner.candidate}
          </p>

          <p className="mt-2 text-lg">
            Total Votes: {winner.votes}
          </p>
        </div>
      )}

      <div className="bg-slate-900 rounded-3xl p-6">
        <h2 className="text-2xl font-bold mb-5">
          Vote Distribution
        </h2>

        <div className="w-full h-[300px] sm:h-[400px] md:h-[450px]">
          <Suspense fallback={<div className="text-slate-400 text-center py-20">Loading chart...</div>}>
            <ResultsChart data={results} />
          </Suspense>
        </div>
      </div>
    </div>
  );
}
