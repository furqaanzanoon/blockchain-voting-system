import { ethers } from "ethers";

async function main() {
  const provider = new ethers.JsonRpcProvider("https://ethereum-sepolia-rpc.publicnode.com");
  const wallet = new ethers.Wallet("0x2366e27581ff612c77d0e5fb703961cc0a83f22a6592f582441e601a4fc8fe55", provider);
  
  const balance = await provider.getBalance(wallet.address);
  console.log(`Admin Wallet Address: ${wallet.address}`);
  console.log(`Admin Wallet Balance: ${ethers.formatEther(balance)} ETH`);
}

main().catch(console.error);
