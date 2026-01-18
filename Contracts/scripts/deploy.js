const hre = require("hardhat");

async function main() {
  console.log("Deploying GraffitiAnchor contract...");
  
  const GraffitiAnchor = await hre.ethers.getContractFactory("GraffitiAnchor");
  const graffitiAnchor = await GraffitiAnchor.deploy();
  
  await graffitiAnchor.waitForDeployment();
  
  const address = await graffitiAnchor.getAddress();
  
  console.log("\n✅ GraffitiAnchor deployed successfully!");
  console.log("Contract address:", address);
  console.log("\n📋 Next steps:");
  console.log("1. Copy the contract address above");
  console.log("2. In Unity, paste it into BlockchainManager > Contract Address");
  console.log("3. Update RPC URL if deploying to testnet/mainnet");
  console.log("\n🔍 Verify deployment:");
  console.log(`   npx hardhat verify --network ${hre.network.name} ${address}`);
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
