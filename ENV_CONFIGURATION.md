# Environment Configuration Examples

## .env Template for Hardhat

Create a `.env` file in the `Contracts/` directory:

```bash
# DO NOT COMMIT THIS FILE TO GIT!
# Add .env to .gitignore

# ========================================
# BLOCKCHAIN CONFIGURATION
# ========================================

# Testnet RPC URLs (get from Infura, Alchemy, or public endpoints)
GOERLI_RPC_URL=https://goerli.infura.io/v3/YOUR_INFURA_PROJECT_ID
SEPOLIA_RPC_URL=https://sepolia.infura.io/v3/YOUR_INFURA_PROJECT_ID
MUMBAI_RPC_URL=https://polygon-mumbai.g.alchemy.com/v2/YOUR_ALCHEMY_KEY

# Mainnet RPC URLs (for production)
MAINNET_RPC_URL=https://mainnet.infura.io/v3/YOUR_INFURA_PROJECT_ID
POLYGON_RPC_URL=https://polygon-mainnet.g.alchemy.com/v2/YOUR_ALCHEMY_KEY
ARBITRUM_RPC_URL=https://arb-mainnet.g.alchemy.com/v2/YOUR_ALCHEMY_KEY

# Wallet Private Key (NEVER use keys with real funds for development!)
# This should be a testnet-only wallet
PRIVATE_KEY=0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef

# Etherscan API Key (for contract verification)
ETHERSCAN_API_KEY=YOUR_ETHERSCAN_API_KEY
POLYGONSCAN_API_KEY=YOUR_POLYGONSCAN_API_KEY

# ========================================
# KAIRO SECURITY CONFIGURATION
# ========================================

# Kairo API Key (get from https://kairoaisec.com)
KAIRO_API_KEY=kairo_sk_live_xxxxxxxxxxxxxxxxxxxxx
KAIRO_PROJECT_ID=proj_xxxxxxxxxxxxxxxxxxxxx

# ========================================
# METADATA STORAGE CONFIGURATION
# ========================================

# IPFS Configuration (for production)
IPFS_PROJECT_ID=your_infura_ipfs_project_id
IPFS_PROJECT_SECRET=your_infura_ipfs_secret
IPFS_API_URL=https://ipfs.infura.io:5001

# Pinata IPFS (alternative)
PINATA_API_KEY=your_pinata_api_key
PINATA_SECRET_KEY=your_pinata_secret_key

# Local Metadata Server
METADATA_SERVER_PORT=3000
METADATA_SERVER_URL=http://localhost:3000

# ========================================
# DEVELOPMENT CONFIGURATION
# ========================================

# Local network
LOCALHOST_RPC_URL=http://127.0.0.1:8545
LOCALHOST_CHAIN_ID=31337

# Gas Settings
GAS_PRICE=20000000000
GAS_LIMIT=6000000

# Deployment Settings
DEPLOY_CONFIRMATIONS=2
DEPLOY_TIMEOUT=300000
```

## Unity Configuration (ScriptableObject)

Create a `BlockchainConfig.cs` ScriptableObject:

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "BlockchainConfig", menuName = "Blockchain/Config")]
public class BlockchainConfig : ScriptableObject
{
    [Header("Network Configuration")]
    public NetworkType networkType = NetworkType.Localhost;
    
    [Header("Contract Addresses")]
    public string graffitiAnchorAddress = "0x5FbDB2315678afecb367f032d93F642f64180aa3";
    
    [Header("RPC URLs")]
    public string localhostRPC = "http://localhost:8545";
    public string goerliRPC = "https://goerli.infura.io/v3/YOUR_KEY";
    public string sepoliaRPC = "https://sepolia.infura.io/v3/YOUR_KEY";
    public string mainnetRPC = "https://mainnet.infura.io/v3/YOUR_KEY";
    
    [Header("Wallet")]
    public string walletAddress = "0xf39Fd6e51aad88F6F4ce6aB8827279cffFb92266";
    
    [Header("Kairo Security")]
    public string kairoApiKey = "kairo_sk_live_xxxxx";
    
    [Header("Metadata")]
    public string metadataServerURL = "http://localhost:3000/metadata";
    public string ipfsGateway = "https://ipfs.io/ipfs/";
    
    [Header("Artist Info")]
    public string artistId = "SpatialInkArtist";
    
    public enum NetworkType
    {
        Localhost,
        Goerli,
        Sepolia,
        Mainnet,
        Polygon,
        Arbitrum
    }
    
    public string GetRpcUrl()
    {
        switch (networkType)
        {
            case NetworkType.Localhost: return localhostRPC;
            case NetworkType.Goerli: return goerliRPC;
            case NetworkType.Sepolia: return sepoliaRPC;
            case NetworkType.Mainnet: return mainnetRPC;
            default: return localhostRPC;
        }
    }
    
    public int GetChainId()
    {
        switch (networkType)
        {
            case NetworkType.Localhost: return 31337;
            case NetworkType.Goerli: return 5;
            case NetworkType.Sepolia: return 11155111;
            case NetworkType.Mainnet: return 1;
            case NetworkType.Polygon: return 137;
            case NetworkType.Arbitrum: return 42161;
            default: return 31337;
        }
    }
}
```

## Hardhat Config with Environment Variables

Update `hardhat.config.js` to use environment variables:

```javascript
require("@nomicfoundation/hardhat-toolbox");
require("dotenv").config();

const PRIVATE_KEY = process.env.PRIVATE_KEY || "0xac0974bec39a17e36ba4a6b4d238ff944bacb478cbed5efcae784d7bf4f2ff80";
const GOERLI_RPC_URL = process.env.GOERLI_RPC_URL || "";
const SEPOLIA_RPC_URL = process.env.SEPOLIA_RPC_URL || "";
const MAINNET_RPC_URL = process.env.MAINNET_RPC_URL || "";
const ETHERSCAN_API_KEY = process.env.ETHERSCAN_API_KEY || "";

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
    localhost: {
      url: "http://127.0.0.1:8545",
      chainId: 31337
    },
    goerli: {
      url: GOERLI_RPC_URL,
      accounts: [PRIVATE_KEY],
      chainId: 5
    },
    sepolia: {
      url: SEPOLIA_RPC_URL,
      accounts: [PRIVATE_KEY],
      chainId: 11155111
    },
    mainnet: {
      url: MAINNET_RPC_URL,
      accounts: [PRIVATE_KEY],
      chainId: 1
    }
  },
  etherscan: {
    apiKey: ETHERSCAN_API_KEY
  }
};
```

## Metadata Server with Environment Variables

Update `metadata-server.js`:

```javascript
require('dotenv').config();

const express = require('express');
const cors = require('cors');
const bodyParser = require('body-parser');

const app = express();
const PORT = process.env.METADATA_SERVER_PORT || 3000;
const MAX_FILE_SIZE = process.env.MAX_FILE_SIZE || '50mb';

app.use(cors());
app.use(bodyParser.json({ limit: MAX_FILE_SIZE }));

// ... rest of server code ...

app.listen(PORT, () => {
  console.log(`Server running on port ${PORT}`);
});
```

## Unity Editor Config (PlayerPrefs Alternative)

For sensitive data that shouldn't be in ScriptableObjects:

```csharp
using UnityEngine;

public static class SecureConfig
{
    // Use this for development only - not production!
    public static string GetPrivateKey()
    {
        // In production, use secure storage like:
        // - Unity Keychain (iOS/macOS)
        // - Android Keystore
        // - Windows Credential Manager
        
        return PlayerPrefs.GetString("PRIVATE_KEY", "");
    }
    
    public static void SetPrivateKey(string key)
    {
        PlayerPrefs.SetString("PRIVATE_KEY", key);
        PlayerPrefs.Save();
    }
    
    public static string GetKairoApiKey()
    {
        return PlayerPrefs.GetString("KAIRO_API_KEY", "");
    }
    
    public static void SetKairoApiKey(string key)
    {
        PlayerPrefs.SetString("KAIRO_API_KEY", key);
        PlayerPrefs.Save();
    }
    
    public static void ClearAllKeys()
    {
        PlayerPrefs.DeleteKey("PRIVATE_KEY");
        PlayerPrefs.DeleteKey("KAIRO_API_KEY");
        PlayerPrefs.Save();
    }
}
```

## Docker Compose Setup (Optional)

For running all services together:

```yaml
version: '3.8'

services:
  hardhat:
    image: node:18
    working_dir: /app
    volumes:
      - ./Contracts:/app
    command: npx hardhat node --hostname 0.0.0.0
    ports:
      - "8545:8545"
    networks:
      - blockchain-network

  metadata-server:
    image: node:18
    working_dir: /app
    volumes:
      - ./Contracts:/app
    command: node metadata-server.js
    ports:
      - "3000:3000"
    environment:
      - METADATA_SERVER_PORT=3000
    networks:
      - blockchain-network

networks:
  blockchain-network:
    driver: bridge
```

## GitHub Actions CI/CD

Example workflow for testing:

```yaml
name: Smart Contract Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'
      
      - name: Install dependencies
        working-directory: ./Contracts
        run: npm install
      
      - name: Run Hardhat tests
        working-directory: ./Contracts
        run: npx hardhat test
      
      - name: Run Kairo security check
        working-directory: ./Contracts
        env:
          KAIRO_API_KEY: ${{ secrets.KAIRO_API_KEY }}
        run: |
          curl -X POST https://api.kairoaisec.com/v1/analyze \
            -H "Authorization: Bearer $KAIRO_API_KEY" \
            -H "Content-Type: application/json" \
            -d @test-payload.json
```

## Quest Environment Variables

For Quest builds, you can't use environment variables directly. Instead:

1. **Use Remote Config** (Unity Gaming Services)
2. **Use a config server** that returns settings
3. **Build-time configuration** in Unity

Example config server:

```javascript
// config-server.js
const express = require('express');
const app = express();

app.get('/config', (req, res) => {
  res.json({
    rpcUrl: process.env.RPC_URL || 'http://192.168.1.100:8545',
    metadataUrl: process.env.METADATA_URL || 'http://192.168.1.100:3000/metadata',
    contractAddress: process.env.CONTRACT_ADDRESS || '0x...'
  });
});

app.listen(4000, () => console.log('Config server on port 4000'));
```

Then in Unity:

```csharp
IEnumerator LoadConfig()
{
    using (UnityWebRequest www = UnityWebRequest.Get("http://192.168.1.100:4000/config"))
    {
        yield return www.SendWebRequest();
        
        if (www.result == UnityWebRequest.Result.Success)
        {
            var config = JsonUtility.FromJson<RemoteConfig>(www.downloadHandler.text);
            blockchainManager.rpcUrl = config.rpcUrl;
            blockchainManager.contractAddress = config.contractAddress;
        }
    }
}
```

## Security Notes

### ⚠️ IMPORTANT

1. **NEVER commit private keys** to git
2. **NEVER commit API keys** to git
3. **NEVER use production keys** in development
4. **ALWAYS use testnet** for testing
5. **ALWAYS add `.env` to `.gitignore`**

### Secure Key Management

For production:

1. **Use Hardware Wallets** (Ledger, Trezor) for deployments
2. **Use AWS Secrets Manager** or similar for API keys
3. **Use environment variables** on servers
4. **Implement proper access control**
5. **Rotate keys regularly**

---

## Getting Environment Values

### Get Infura Project ID
1. Go to https://infura.io
2. Sign up / Log in
3. Create new project
4. Copy Project ID from settings

### Get Alchemy API Key
1. Go to https://alchemy.com
2. Sign up / Log in
3. Create new app
4. Copy API Key

### Get Kairo API Key
1. Go to https://kairoaisec.com
2. Sign up / Log in
3. Create new project
4. Navigate to API Settings
5. Generate new API key

### Get Etherscan API Key
1. Go to https://etherscan.io
2. Sign up / Log in
3. My Account → API Keys
4. Create new API key

---

This configuration system allows you to easily switch between development, testnet, and production environments!
