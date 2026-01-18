# Blockchain Integration Setup Guide

## Overview
This guide helps you set up the blockchain integration for your AR Drawing app using the GraffitiAnchor smart contract and Kairo security platform.

## Architecture

```
AR Drawing App (Unity/Quest)
    ↓
DrawingSaveManager.cs
    ↓
BlockchainManager.cs
    ├→ Uploads metadata to localhost server
    └→ Claims location on GraffitiAnchor contract
    
KairoSecurityCheck.cs
    ↓
Kairo API (security validation)
```

## Prerequisites

1. **Node.js** (v16+) for running local blockchain
2. **Hardhat** or **Foundry** for smart contract deployment
3. **Kairo API Key** from [https://kairoaisec.com](https://kairoaisec.com)
4. **Unity 2021.3+** with Meta XR SDK

## Step 1: Deploy Smart Contract

### Using Hardhat

```bash
# Install Hardhat
npm install --save-dev hardhat

# Initialize Hardhat project
npx hardhat init

# Copy the contract
cp Contracts/GraffitiAnchor.sol contracts/

# Create deployment script
cat > scripts/deploy.js << 'EOF'
async function main() {
  const GraffitiAnchor = await ethers.getContractFactory("GraffitiAnchor");
  const contract = await GraffitiAnchor.deploy();
  await contract.deployed();
  
  console.log("GraffitiAnchor deployed to:", contract.address);
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
EOF

# Start local blockchain
npx hardhat node

# Deploy contract (in another terminal)
npx hardhat run scripts/deploy.js --network localhost
```

### Using Foundry

```bash
# Install Foundry
curl -L https://foundry.paradigm.xyz | bash
foundryup

# Initialize project
forge init

# Copy contract
cp Contracts/GraffitiAnchor.sol src/

# Deploy to local network
anvil  # Starts local blockchain

# In another terminal
forge create GraffitiAnchor --rpc-url http://localhost:8545 --private-key 0xac0974bec39a17e36ba4a6b4d238ff944bacb478cbed5efcae784d7bf4f2ff80
```

**Save the deployed contract address!** You'll need it for Unity configuration.

## Step 2: Set Up Kairo Security

1. **Sign up at Kairo**: https://kairoaisec.com
2. **Create a project**
3. **Generate API Key** (format: `kairo_sk_live_xxxxx`)
4. **Test the API**:

```bash
curl -X POST https://api.kairoaisec.com/v1/analyze \
  -H "Authorization: Bearer YOUR_API_KEY" \
  -H "Content-Type: application/json" \
  -d @- << 'EOF'
{
  "source": {
    "type": "inline",
    "files": [{
      "path": "GraffitiAnchor.sol",
      "content": "$(cat Contracts/GraffitiAnchor.sol)"
    }]
  },
  "config": {
    "severity_threshold": "high",
    "include_suggestions": true
  }
}
EOF
```

Expected response:
```json
{
  "decision": "ALLOW",
  "decision_reason": "No security issues detected. Safe to proceed.",
  "risk_score": 0,
  "summary": { "total": 0, "critical": 0, "high": 0, "medium": 0, "low": 0 }
}
```

## Step 3: Set Up Metadata Server

Create a simple Express server to handle metadata uploads:

```bash
# Create server directory
mkdir metadata-server
cd metadata-server

# Initialize npm
npm init -y

# Install dependencies
npm install express cors body-parser

# Create server
cat > server.js << 'EOF'
const express = require('express');
const cors = require('cors');
const bodyParser = require('body-parser');
const fs = require('fs');
const path = require('path');

const app = express();
const PORT = 3000;

app.use(cors());
app.use(bodyParser.json({ limit: '50mb' }));

// Create metadata directory
const metadataDir = path.join(__dirname, 'metadata');
if (!fs.existsSync(metadataDir)) {
  fs.mkdirSync(metadataDir, { recursive: true });
}

// Upload metadata endpoint
app.post('/metadata', (req, res) => {
  try {
    const metadata = req.body;
    const timestamp = Date.now();
    const filename = `metadata_${timestamp}.json`;
    const filepath = path.join(metadataDir, filename);
    
    fs.writeFileSync(filepath, JSON.stringify(metadata, null, 2));
    
    const uri = `http://localhost:${PORT}/metadata/${filename}`;
    
    console.log(`Metadata saved: ${filename}`);
    
    res.json({
      success: true,
      uri: uri,
      filename: filename
    });
  } catch (error) {
    console.error('Error saving metadata:', error);
    res.status(500).json({ error: error.message });
  }
});

// Serve metadata files
app.use('/metadata', express.static(metadataDir));

// Health check
app.get('/health', (req, res) => {
  res.json({ status: 'ok' });
});

app.listen(PORT, () => {
  console.log(`Metadata server running on http://localhost:${PORT}`);
});
EOF

# Start server
node server.js
```

## Step 4: Configure Unity

### 4.1 Configure BlockchainManager

In Unity Editor:

1. **Create Empty GameObject**: `BlockchainSystem`
2. **Add Components**:
   - `BlockchainManager.cs`
   - `KairoSecurityCheck.cs`
   - `DrawingSaveManager.cs`

3. **Configure BlockchainManager**:
   - **Contract Address**: `0x...` (from Step 1)
   - **RPC URL**: `http://localhost:8545`
   - **Chain ID**: `1337` (or `31337` for Hardhat)
   - **Network Name**: `localhost`
   - **Wallet Address**: Use one from your local blockchain
   - **Private Key**: Corresponding private key (KEEP SECRET!)
   - **Metadata Base URL**: `http://localhost:3000/metadata`
   - **Artist ID**: Your artist name

4. **Configure KairoSecurityCheck**:
   - **API Key**: `kairo_sk_live_xxxxx` (from Step 2)
   - **Severity Threshold**: `high`
   - **Block On Security Issues**: `true`

5. **Configure DrawingSaveManager**:
   - **Drawing Manager**: Link to your DrawingManager
   - **Blockchain Manager**: Link to BlockchainManager
   - **Save Local JSON**: `true`
   - **Claim On Blockchain**: `true`

### 4.2 Add Save Button to UI

```csharp
// In your UI script
using UnityEngine;
using UnityEngine.UI;

public class DrawingUI : MonoBehaviour
{
    public Button saveButton;
    public DrawingSaveManager saveManager;
    
    void Start()
    {
        saveButton.onClick.AddListener(() => {
            saveManager.SaveDrawing();
        });
    }
}
```

## Step 5: Test the Integration

### 5.1 Run Security Check

```csharp
// Test Kairo security in Unity
void TestSecurity()
{
    string contractCode = File.ReadAllText("Contracts/GraffitiAnchor.sol");
    
    KairoSecurityCheck kairo = GetComponent<KairoSecurityCheck>();
    kairo.AnalyzeContract(contractCode, "GraffitiAnchor.sol", (result) =>
    {
        Debug.Log($"Security Decision: {result.decision}");
        Debug.Log($"Risk Score: {result.risk_score}");
    });
}
```

### 5.2 Test Metadata Upload

```bash
# Test metadata server
curl -X POST http://localhost:3000/metadata \
  -H "Content-Type: application/json" \
  -d '{
    "artistId": "TestArtist",
    "x": 1.5,
    "y": 0.5,
    "z": 2.0,
    "totalStrokes": 1,
    "strokes": [
      {
        "points": [[1.0, 0.0, 0.0], [2.0, 1.0, 0.0]],
        "color": "#FF0000",
        "size": 0.02,
        "brushType": "FlatBrush"
      }
    ]
  }'
```

### 5.3 Test Full Flow

1. **Start all services**:
   ```bash
   # Terminal 1: Blockchain
   npx hardhat node
   
   # Terminal 2: Metadata server
   cd metadata-server
   node server.js
   ```

2. **Run Unity app on Quest**:
   - Connect Quest to same network as your PC
   - Update RPC URL and Metadata URL to use your PC's IP:
     - `http://192.168.1.X:8545` (blockchain)
     - `http://192.168.1.X:3000/metadata` (metadata)

3. **Draw something in AR**

4. **Press Save button**
   - Watch console logs
   - Check metadata server logs
   - Verify blockchain transaction

## Step 6: Verify Deployment

### Check Blockchain State

```javascript
// Using Hardhat console
npx hardhat console --network localhost

const GraffitiAnchor = await ethers.getContractFactory("GraffitiAnchor");
const contract = await GraffitiAnchor.attach("YOUR_CONTRACT_ADDRESS");

// Get total claims
const nextId = await contract.nextClaimId();
console.log("Total claims:", nextId.toNumber() - 1);

// Get specific claim
const claim = await contract.getClaim(1);
console.log("Claim 1:", claim);

// Get artist claims
const claims = await contract.getArtistClaims("ARTIST_ADDRESS");
console.log("Artist claims:", claims);
```

### Check Metadata

```bash
# List all metadata files
curl http://localhost:3000/metadata/

# View specific metadata
curl http://localhost:3000/metadata/metadata_1234567890.json
```

## Troubleshooting

### Unity can't connect to localhost

**Problem**: Quest can't reach `localhost:8545`

**Solution**: Use your PC's local IP address:
```csharp
// In BlockchainManager
public string rpcUrl = "http://192.168.1.100:8545";
public string metadataBaseUrl = "http://192.168.1.100:3000/metadata";
```

### Kairo API returns 401 Unauthorized

**Problem**: Invalid API key

**Solution**: 
1. Check API key format: `kairo_sk_live_xxxxx`
2. Verify key is active in Kairo dashboard
3. Check for extra spaces or quotes

### Transaction fails

**Problem**: Blockchain transaction fails

**Solution**:
1. Check wallet has funds (use Hardhat's default accounts)
2. Verify contract address is correct
3. Check contract is deployed: `cast call YOUR_ADDRESS "owner()" --rpc-url http://localhost:8545`

### Metadata upload fails

**Problem**: 413 Payload Too Large

**Solution**: Increase body parser limit in server.js:
```javascript
app.use(bodyParser.json({ limit: '100mb' }));
```

## Production Deployment

### Deploy to Testnet (Goerli/Sepolia)

```bash
# Get testnet ETH from faucet
# https://goerlifaucet.com/

# Deploy with Hardhat
npx hardhat run scripts/deploy.js --network goerli
```

### Use IPFS for Metadata

```bash
# Install IPFS
npm install ipfs-http-client

# Update metadata upload to use IPFS
# See: https://docs.ipfs.tech/how-to/
```

### Secure API Keys

**Never commit API keys or private keys!**

Use environment variables:
```csharp
// In Unity, load from secure storage
string apiKey = Environment.GetEnvironmentVariable("KAIRO_API_KEY");
```

## Resources

- **Kairo Docs**: https://docs.kairoaisec.com
- **Hardhat Docs**: https://hardhat.org/docs
- **Foundry Docs**: https://book.getfoundry.sh/
- **Meta XR SDK**: https://developer.oculus.com/documentation/

## Support

For issues or questions:
1. Check Unity console logs
2. Check blockchain node logs
3. Check metadata server logs
4. Verify Kairo API status

---

**Security Reminder**: Always run Kairo security checks before deploying to mainnet!
