import { ethers } from "ethers";

async function main() {
  const provider = new ethers.JsonRpcProvider("https://ethereum-sepolia-rpc.publicnode.com");
  const contractAddress = "0xc3CC8F273264FeEA9b0878b946f07c557d91De0A";
  
  const code = await provider.getCode(contractAddress);
  console.log("Contract code length:", code.length);
  if (code === "0x" || code === "0x0") {
    console.log("❌ NO contract deployed at this address!");
    return;
  }
  
  const abi = [
    "function owner() view returns (address)",
    "function votingOpen() view returns (bool)",
    "function DOMAIN_SEPARATOR() view returns (bytes32)",
    "function VOTE_TYPEHASH() view returns (bytes32)",
    "function getCandidateCount() view returns (uint256)"
  ];
  
  const contract = new ethers.Contract(contractAddress, abi, provider);
  
  try {
    const owner = await contract.owner();
    console.log("✅ owner():", owner);
  } catch (err: any) {
    console.log("❌ owner() failed:", err.message || err);
  }

  try {
    const open = await contract.votingOpen();
    console.log("✅ votingOpen():", open);
  } catch (err: any) {
    console.log("❌ votingOpen() failed:", err.message || err);
  }

  try {
    const ds = await contract.DOMAIN_SEPARATOR();
    console.log("✅ DOMAIN_SEPARATOR():", ds);
  } catch (err: any) {
    console.log("❌ DOMAIN_SEPARATOR() failed:", err.message || err);
  }

  try {
    const th = await contract.VOTE_TYPEHASH();
    console.log("✅ VOTE_TYPEHASH():", th);
  } catch (err: any) {
    console.log("❌ VOTE_TYPEHASH() failed:", err.message || err);
  }

  try {
    const cc = await contract.getCandidateCount();
    console.log("✅ getCandidateCount():", cc.toString());
  } catch (err: any) {
    console.log("❌ getCandidateCount() failed:", err.message || err);
  }
}

main().catch(console.error);
