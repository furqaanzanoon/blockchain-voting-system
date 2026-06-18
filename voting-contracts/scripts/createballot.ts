import hre from "hardhat";

async function main() {

  const ethers =
    await hre.network.getOrCreate().then(n => n.ethers);

  const ballotFactoryAddress =
    "0x5FbDB2315678afecb367f032d93F642f64180aa3";

  const factory =
    await ethers.getContractAt(
      "BallotFactory",
      ballotFactoryAddress
    );

  const tx = await factory.createBallot(
    "College Election",
    ["Alice", "Bob", "Charlie"]
  );

  await tx.wait();

  console.log("Ballot created");

  // Get ballot info
  const ballotInfo =
    await factory.getBallotInfo(0);

  console.log("Voting Contract Address:");
  console.log(ballotInfo.contractAddress);
}

main().catch(console.error);