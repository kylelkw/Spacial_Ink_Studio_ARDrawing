const hre = require("hardhat");

async function main() {
  console.log("Deploying GraffitiAnchor contract...");

  // Get the contract factory
  const GraffitiAnchor = await hre.ethers.getContractFactory("GraffitiAnchor");
  
  // Deploy the contract
  const graffitiAnchor = await GraffitiAnchor.deploy();
  
  await graffitiAnchor.waitForDeployment();
  
  const address = await graffitiAnchor.getAddress();
  
  console.log("GraffitiAnchor deployed to:", address);
  console.log("\n=================================");
  console.log("Deployment Summary:");
  console.log("=================================");
  console.log("Contract Address:", address);
  console.log("Network:", hre.network.name);
  console.log("Deployer:", (await hre.ethers.getSigners())[0].address);
  console.log("\n⚠️  IMPORTANT: Save this contract address!");
  console.log("Update your Unity BlockchainConfig with this address:");
  console.log(`contractAddress = "${address}"`);
  console.log("=================================\n");
  
  // Wait for block confirmations
  if (hre.network.name !== "hardhat" && hre.network.name !== "localhost") {
    console.log("Waiting for block confirmations...");
    await graffitiAnchor.deploymentTransaction().wait(5);
    console.log("Confirmed!");
    
    // Verify on block explorer
    console.log("\nVerifying contract on block explorer...");
    try {
      await hre.run("verify:verify", {
        address: address,
        constructorArguments: [],
      });
      console.log("Contract verified!");
    } catch (error) {
      console.log("Verification failed:", error.message);
      console.log("You can verify manually later with:");
      console.log(`npx hardhat verify --network ${hre.network.name} ${address}`);
    }
  }
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
