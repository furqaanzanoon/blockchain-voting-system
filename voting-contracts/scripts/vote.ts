import hre from "hardhat";

async function main() {

    const ethers =
        await hre.network.getOrCreate().then(n => n.ethers);

    // BallotFactory
    const factory =
        await ethers.getContractAt(
            "BallotFactory",
            "0xDc64a140Aa3E981100a9becA4E685f962f0cF6C9"
        );

    // Ballot 0
    const ballotInfo =
        await factory.getBallotInfo(0);

    const votingAddress =
        ballotInfo.contractAddress;

    console.log("Voting Address:");
    console.log(votingAddress);

    // Signer
    const [voter] =
        await ethers.getSigners();

    console.log("Voter Wallet:");
    console.log(voter.address);

    // Voting contract
    const voting =
        await ethers.getContractAt(
            "Voting",
            votingAddress
        );

    const tx =
        await voting.vote(
            voter.address,
            0
        );

    console.log("Tx:");
    console.log(tx.hash);

    const receipt =
        await tx.wait();

    console.log("Block:");
    console.log(receipt?.blockNumber);
}

main().catch(console.error);