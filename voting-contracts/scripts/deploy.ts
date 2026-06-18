import hre from "hardhat";
import fs from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

async function main() {
  const delay = (ms: number) => new Promise((resolve) => setTimeout(resolve, ms));

  const ethers = await hre.network
    .create()
    .then(n => n.ethers);

  const [deployer] =
    await ethers.getSigners();

  console.log(
    "Deploying contracts with:",
    deployer.address
  );

  // =====================================================
  // Deploy AccessControl
  // =====================================================

  const AccessControl =
    await ethers.getContractFactory(
      "AccessControl"
    );

  const accessControl =
    await AccessControl.deploy();

  await accessControl.waitForDeployment();

  const accessControlAddress =
    await accessControl.getAddress();

  console.log(
    "AccessControl deployed to:",
    accessControlAddress
  );

  await delay(5000);

  // =====================================================
  // Grant ADMIN role to deployer
  // =====================================================

  const ADMIN_ROLE =
    await accessControl.ADMIN();

  const grantTx = await accessControl.grantRole(
    ADMIN_ROLE,
    deployer.address
  );
  await grantTx.wait();

  console.log(
    "ADMIN role granted to:",
    deployer.address
  );

  await delay(5000);

  // =====================================================
  // Deploy ZKVerifier
  // =====================================================

  const ZKVerifier =
    await ethers.getContractFactory(
      "ZKVerifier"
    );

  const zkVerifier =
    await ZKVerifier.deploy();

  await zkVerifier.waitForDeployment();

  const zkVerifierAddress =
    await zkVerifier.getAddress();

  console.log(
    "ZKVerifier deployed to:",
    zkVerifierAddress
  );

  await delay(5000);

  // =====================================================
  // Deploy VoterRegistry
  // =====================================================

  const VoterRegistry =
    await ethers.getContractFactory(
      "VoterRegistry"
    );

  const voterRegistry =
    await VoterRegistry.deploy(
      accessControlAddress
    );

  await voterRegistry.waitForDeployment();

  const registryAddress =
    await voterRegistry.getAddress();

  console.log(
    "VoterRegistry deployed to:",
    registryAddress
  );

  await delay(5000);

  // =====================================================
  // Deploy BallotFactory
  // =====================================================

  const BallotFactory =
    await ethers.getContractFactory(
      "BallotFactory"
    );

  const ballotFactory =
    await BallotFactory.deploy(
      registryAddress,
      zkVerifierAddress
    );

  await ballotFactory.waitForDeployment();

  const ballotFactoryAddress =
    await ballotFactory.getAddress();

  console.log(
    "BallotFactory deployed to:",
    ballotFactoryAddress
  );

  await delay(5000);

  // =====================================================
  // Deploy ResultAggregator
  // =====================================================

  const ResultAggregator =
    await ethers.getContractFactory(
      "ResultAggregator"
    );

  const aggregator =
    await ResultAggregator.deploy(
      ballotFactoryAddress
    );

  await aggregator.waitForDeployment();

  const aggregatorAddress =
    await aggregator.getAddress();

  console.log(
    "ResultAggregator deployed to:",
    aggregatorAddress
  );

  await delay(5000);

  // =====================================================
  // Final Summary
  // =====================================================

  console.log("\n========== DEPLOYMENT SUMMARY ==========");

  console.log("AccessControl:", accessControlAddress);
  console.log("ZKVerifier:", zkVerifierAddress);
  console.log("VoterRegistry:", registryAddress);
  console.log("BallotFactory:", ballotFactoryAddress);
  console.log("ResultAggregator:", aggregatorAddress);

  console.log("========================================");

  // =====================================================
  // Update appsettings.json in the backend project
  // =====================================================

  const appsettingsPath = path.resolve(
    __dirname,
    "../../VotingAPI/VotingAPI/appsettings.json"
  );

  try {
    const raw = fs.readFileSync(appsettingsPath, "utf-8");
    const appsettings = JSON.parse(raw);

    appsettings.BlockchainSettings = {
      ...appsettings.BlockchainSettings,
      AccessControlAddress: accessControlAddress,
      ZKVerifierAddress: zkVerifierAddress,
      VoterRegistryAddress: registryAddress,
      BallotFactoryAddress: ballotFactoryAddress,
      ResultAggregatorAddress: aggregatorAddress,
    };

    fs.writeFileSync(
      appsettingsPath,
      JSON.stringify(appsettings, null, 2) + "\n",
      "utf-8"
    );

    console.log(
      "\n✅ appsettings.json updated at:",
      appsettingsPath
    );
  } catch (err) {
    console.error(
      "\n❌ Failed to update appsettings.json:",
      err
    );
  }
}

main().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});