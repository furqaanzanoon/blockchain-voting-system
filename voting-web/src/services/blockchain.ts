import { ethers } from "ethers";
import VotingABI from "../abi/Voting.json";

/**
 * Returns an ethers.Contract instance for a specific Voting contract address.
 * The address is dynamic per election — fetched from the API (election.contractAddress).
 *
 * @param contractAddress - The deployed Voting contract address for the election
 */
export const getVotingContract = async (contractAddress: string) => {
  if (!contractAddress) {
    throw new Error("Contract address is required");
  }

  if (!window.ethereum) {
    throw new Error(
      "MetaMask is not installed. Please install the MetaMask extension."
    );
  }

  const provider = new ethers.BrowserProvider(window.ethereum);
  const signer = await provider.getSigner();

  return new ethers.Contract(contractAddress, VotingABI.abi, signer);
};