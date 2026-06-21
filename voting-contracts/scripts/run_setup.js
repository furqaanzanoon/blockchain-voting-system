import { execSync } from "child_process";
import fs from "fs";
import path from "path";

function runCmd(cmd) {
  console.log(`Running: ${cmd}`);
  execSync(cmd, { stdio: "inherit" });
}

async function main() {
  console.log("Starting ZK-SNARK trusted setup ceremony...");
  
  // 1. Create a new Powers of Tau ceremony (power 13, supports up to 8192 constraints)
  runCmd("npx snarkjs powersoftau new bn128 13 pot13_0000.ptau -v");
  
  // 2. Contribute to the ceremony
  runCmd("npx snarkjs powersoftau contribute pot13_0000.ptau pot13_0001.ptau --name=\"First Contribution\" -v -e=\"random entropy context text\"");
  
  // 3. Prepare Phase 2 (this finishes the global setup stage)
  runCmd("npx snarkjs powersoftau prepare phase2 pot13_0001.ptau pot13_final.ptau -v");
  
  // 4. Run Groth16 setup for our circuit
  runCmd("npx snarkjs groth16 setup circuits/vote.r1cs pot13_final.ptau circuits/vote_0000.zkey");
  
  // 5. Contribute to the zkey
  runCmd("npx snarkjs zkey contribute circuits/vote_0000.zkey circuits/vote_final.zkey --name=\"Verifier Contributor\" -v -e=\"voter entropy text\"");
  
  // 6. Export verification key to JSON
  runCmd("npx snarkjs zkey export verificationkey circuits/vote_final.zkey circuits/verification_key.json");
  
  // 7. Export Solidity verifier contract to ZKVerifier.sol
  runCmd("npx snarkjs zkey export solidityverifier circuits/vote_final.zkey contracts/security/ZKVerifier.sol");
  
  console.log("Setup complete! Cleaning up temporary ceremony files...");
  
  // Clean up intermediate files
  fs.unlinkSync("pot13_0000.ptau");
  fs.unlinkSync("pot13_0001.ptau");
  fs.unlinkSync("pot13_final.ptau");
  fs.unlinkSync("circuits/vote_0000.zkey");
  
  console.log("Solidity verifier generated at: contracts/security/ZKVerifier.sol");
  console.log("Verification key generated at: circuits/verification_key.json");
  console.log("Proving key generated at: circuits/vote_final.zkey");
}

main().catch(console.error);
