# Kairo Blockchain Integration - Quick Start

## 🚀 Quick Setup (5 Minutes)

### 1. Install Dependencies

```bash
cd Contracts

# Install Hardhat dependencies
npm install

# Install metadata server dependencies
npm install express cors body-parser
```

### 2. Start Local Blockchain

```bash
# Terminal 1 - Start Hardhat node
npx hardhat node
```

Keep this terminal running. You'll see test accounts with private keys.

### 3. Deploy Smart Contract

```bash
# Terminal 2 - Deploy contract
npx hardhat run scripts/deploy.js --network localhost
```

Copy the contract address printed in the console.

### 4. Start Metadata Server

```bash
# Terminal 3 - Start metadata server
node metadata-server.js
```

### 5. Configure Unity

In Unity Editor:

1. Create empty GameObject: "BlockchainSystem"
2. Add components:
   - BlockchainManager
   - KairoSecurityCheck
   - DrawingSaveManager

3. **BlockchainManager settings**:
   ```
   Contract Address: [paste from step 3]
   RPC URL: http://localhost:8545
   Chain ID: 31337
   Network Name: localhost
   Wallet Address: [use account #0 from step 2]
   Private Key: [use private key from step 2]
   Metadata Base URL: http://localhost:3000/metadata
   Artist ID: YourArtistName
   ```

4. **KairoSecurityCheck settings**:
   ```
   API Key: YOUR_KAIRO_API_KEY
   Severity Threshold: high
   Block On Security Issues: true
   ```

5. **DrawingSaveManager settings**:
   ```
   Drawing Manager: [drag DrawingManager]
   Blockchain Manager: [drag BlockchainManager]
   Save Local JSON: true
   Claim On Blockchain: true
   ```

### 6. Test It!

1. Run Unity scene
2. Draw something in AR
3. Press Save button
4. Check console logs for transaction hash

## 🧪 Testing Commands

### Test Contract

```bash
npx hardhat test
```

### Test Metadata Server

```bash
curl -X POST http://localhost:3000/metadata \
  -H "Content-Type: application/json" \
  -d '{"artistId":"test","x":1.5,"y":0.5,"z":2.0,"totalStrokes":1,"strokes":[]}'
```

### Check Blockchain State

```bash
npx hardhat console --network localhost

# In console:
const GraffitiAnchor = await ethers.getContractFactory("GraffitiAnchor")
const contract = await GraffitiAnchor.attach("YOUR_CONTRACT_ADDRESS")
const claims = await contract.nextClaimId()
console.log("Total claims:", claims.toString())
```

## 🔒 Security Check with Kairo

1. Get API key from https://kairoaisec.com
2. Test it:

```bash
curl -X POST https://api.kairoaisec.com/v1/analyze \
  -H "Authorization: Bearer YOUR_API_KEY" \
  -H "Content-Type: application/json" \
  -d '{
    "source": {
      "type": "inline",
      "files": [{
        "path": "GraffitiAnchor.sol",
        "content": "'"$(cat GraffitiAnchor.sol)"'"
      }]
    }
  }'
```

## 🎮 For Quest Headset

1. Find your PC's local IP:
   ```bash
   # macOS
   ipconfig getifaddr en0
   
   # Windows
   ipconfig
   ```

2. Update Unity settings:
   ```
   RPC URL: http://YOUR_IP:8545
   Metadata URL: http://YOUR_IP:3000/metadata
   ```

3. Make sure Quest is on same WiFi network

## 📁 File Structure

```
Contracts/
├── GraffitiAnchor.sol          # Smart contract
├── hardhat.config.js           # Hardhat config
├── package.json                # Hardhat dependencies
├── metadata-server.js          # Metadata server
├── metadata-server-package.json # Server dependencies
├── scripts/
│   └── deploy.js               # Deployment script
└── test/
    └── GraffitiAnchor.test.js  # Contract tests

ARDrawingQuest/Assets/DrawingSystem/Scripts/Blockchain/
├── BlockchainManager.cs        # Blockchain integration
├── KairoSecurityCheck.cs       # Kairo API client
└── DrawingSaveManager.cs       # Save + blockchain logic
```

## 🐛 Common Issues

**"Contract not deployed"**
- Make sure Hardhat node is running
- Redeploy contract: `npx hardhat run scripts/deploy.js --network localhost`

**"Cannot connect to localhost"**
- On Quest: Use PC's IP address, not localhost
- Check firewall settings

**"Kairo API error"**
- Verify API key format: `kairo_sk_live_xxxxx`
- Check key is active in Kairo dashboard

## 📚 Full Documentation

See [BLOCKCHAIN_SETUP.md](BLOCKCHAIN_SETUP.md) for complete guide.

## 🎯 Next Steps

- [ ] Deploy to testnet (Goerli/Sepolia)
- [ ] Use IPFS for metadata storage
- [ ] Add NFT minting functionality
- [ ] Implement marketplace features
