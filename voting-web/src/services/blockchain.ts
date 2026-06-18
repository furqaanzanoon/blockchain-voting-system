import { ethers } from "ethers";
import VotingABI from "../abi/Voting.json";

const VOTING_ADDRESS =
  "0xCf7Ed3AccA5a467e9e704C703E8D87F634fB0Fc9";

export const getVotingContract =
  async () => {
    const provider =
      new ethers.BrowserProvider(
        (window as any).ethereum
      );

    const signer =
      await provider.getSigner();

    return new ethers.Contract(
      VOTING_ADDRESS,
      VotingABI.abi,
      signer
    );
  };