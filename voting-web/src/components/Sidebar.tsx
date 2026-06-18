import {
  FaVoteYea,
  FaUsers,
  FaChartBar,
  FaUniversity
} from "react-icons/fa";

interface Props {
  setPage: (page: string) => void;
}

export default function Sidebar({
  setPage
}: Props) {
  return (
    <div className="sidebar">
      <h2 className="sidebar-title">
        Blockchain Voting
      </h2>

      <button
        onClick={() =>
          setPage("elections")
        }
      >
        <FaUniversity /> Elections
      </button>

      <button
        onClick={() =>
          setPage("candidates")
        }
      >
        <FaUsers /> Candidates
      </button>

      <button
        onClick={() =>
          setPage("vote")
        }
      >
        <FaVoteYea /> Vote
      </button>

      <button
        onClick={() =>
          setPage("results")
        }
      >
        <FaChartBar /> Results
      </button>
    </div>
  );
}