require("@nomicfoundation/hardhat-toolbox");
require("dotenv").config();

/** @type import('hardhat/config').HardhatUserConfig */
module.exports = {
  solidity: {
    version: "0.8.20",
    settings: {
      optimizer: {
        enabled: true,
        runs: 200
      }
    }
  },
  networks: {
    // Kairo testnet configuration
    kairoTestnet: {
      url: process.env.KAIRO_RPC_URL || "https://kairo-testnet-rpc.com",
      accounts: process.env.PRIVATE_KEY ? [process.env.PRIVATE_KEY] : [],
      chainId: parseInt(process.env.KAIRO_CHAIN_ID || "1"),
      gasPrice: "auto"
    },
    // Kairo mainnet configuration
    kairoMainnet: {
      url: process.env.KAIRO_MAINNET_RPC_URL || "https://kairo-mainnet-rpc.com",
      accounts: process.env.PRIVATE_KEY ? [process.env.PRIVATE_KEY] : [],
      chainId: parseInt(process.env.KAIRO_MAINNET_CHAIN_ID || "1"),
      gasPrice: "auto"
    },
    // Local development
    hardhat: {
      chainId: 31337
    },
    localhost: {
      url: "http://127.0.0.1:8545"
    }
  },
  etherscan: {
    apiKey: {
      kairoTestnet: process.env.KAIRO_API_KEY || "",
      kairoMainnet: process.env.KAIRO_API_KEY || ""
    },
    customChains: [
      {
        network: "kairoTestnet",
        chainId: parseInt(process.env.KAIRO_CHAIN_ID || "1"),
        urls: {
          apiURL: process.env.KAIRO_API_URL || "https://api.kairo-explorer.com/api",
          browserURL: process.env.KAIRO_EXPLORER_URL || "https://kairo-explorer.com"
        }
      }
    ]
  },
  paths: {
    sources: "./",
    tests: "./test",
    cache: "./cache",
    artifacts: "./artifacts"
  }
};
