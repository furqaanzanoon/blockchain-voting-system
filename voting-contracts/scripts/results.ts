import hre from "hardhat";

async function main() {

  const ethers =
    await hre.network.getOrCreate().then(n => n.ethers);

  // ResultAggregator deployed address
  const aggregatorAddress =
    "0xe7f1725E7734CE288F8367e1Bb143E90bb3F0512";

  // Connect to ResultAggregator contract
  const aggregator =
    await ethers.getContractAt(
      "ResultAggregator",
      aggregatorAddress
    );

  // Get result for ballot ID 0
  const result =
    await aggregator.getBallotResult(0);

  console.log("\n===== BALLOT RESULT =====");

  console.log("Ballot ID:", result.ballotId.toString());

  console.log("Title:", result.title);

  console.log(
    "Voting Contract:",
    result.contractAddress
  );

  console.log(
    "Total Votes:",
    result.totalVotes.toString()
  );

  console.log(
    "Voting Open:",
    result.votingOpen
  );

  console.log("\n===== CANDIDATES =====");

  for (const candidate of result.candidates) {

    console.log(
      `Candidate ${candidate.candidateId}`
    );

    console.log(
      `Name: ${candidate.name}`
    );

    console.log(
      `Votes: ${candidate.voteCount}`
    );

    console.log(
      `Percentage: ${Number(candidate.percentage) / 100}%`
    );

    console.log("-------------------");
  }

  console.log("\n===== WINNERS =====");

  for (const winnerId of result.winnerIds) {

    const winner =
      result.candidates[Number(winnerId)];

    console.log(
      `Winner: ${winner.name}`
    );

    console.log(
      `Votes: ${winner.voteCount}`
    );
  }
}

main().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});