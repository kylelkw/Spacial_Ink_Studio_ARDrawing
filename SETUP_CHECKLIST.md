# 🎯 Setup Checklist

Use this checklist to ensure everything is properly configured.

## ✅ Prerequisites

- [ ] Unity 2022.3 LTS or later installed
- [ ] Meta XR SDK 68.0.0 installed in Unity project
- [ ] Node.js 16+ installed
- [ ] Git installed
- [ ] Quest 3S connected and developer mode enabled
- [ ] Kairo account created at https://kairoaisec.com

## ✅ Blockchain Environment Setup

### Hardhat Installation
- [ ] Navigate to `Contracts/` directory
- [ ] Run `npm install` (installs Hardhat and dependencies)
- [ ] Verify installation: `npx hardhat --version`
- [ ] Configuration file exists: `hardhat.config.js`

### Start Local Blockchain
- [ ] Open terminal in `Contracts/` directory
- [ ] Run `npx hardhat node`
- [ ] See 20 test accounts with addresses and private keys
- [ ] Note: Keep this terminal running!
- [ ] Verify: Check for "Started HTTP and WebSocket JSON-RPC server at..."

### Deploy Smart Contract
- [ ] Open NEW terminal in `Contracts/` directory
- [ ] Run `npx hardhat run scripts/deploy.js --network localhost`
- [ ] Contract deploys successfully
- [ ] **SAVE CONTRACT ADDRESS** displayed in output
- [ ] Example: `0x5FbDB2315678afecb367f032d93F642f64180aa3`

### Test Smart Contract (Optional but Recommended)
- [ ] Run `npx hardhat test`
- [ ] All 16 tests pass ✓
- [ ] No errors or failures

## ✅ Metadata Server Setup

### Install Dependencies
- [ ] In `Contracts/` directory
- [ ] Run `npm install express cors body-parser`
- [ ] (Or use `metadata-server-package.json` if separate installation)

### Start Metadata Server
- [ ] Open NEW terminal in `Contracts/` directory
- [ ] Run `node metadata-server.js`
- [ ] See "Server running on http://localhost:3000"
- [ ] Note: Keep this terminal running!
- [ ] Verify: Open browser to `http://localhost:3000/health`
- [ ] Should see `{"status":"ok",...}`

### Test Metadata Upload
- [ ] Run test command:
```bash
curl -X POST http://localhost:3000/metadata \
  -H "Content-Type: application/json" \
  -d '{"artistId":"test","x":1.5,"y":0.5,"z":2.0,"totalStrokes":1,"strokes":[]}'
```
- [ ] Receive success response with URI
- [ ] Metadata file created in `Contracts/metadata/` directory

## ✅ Kairo Security Setup

### Get API Key
- [ ] Sign up at https://kairoaisec.com
- [ ] Create a new project
- [ ] Navigate to API Settings
- [ ] Generate new API key
- [ ] **SAVE API KEY** (format: `kairo_sk_live_xxxxx`)
- [ ] Copy to secure location (password manager)

### Test Kairo API
- [ ] Run test command:
```bash
curl -X POST https://api.kairoaisec.com/v1/analyze \
  -H "Authorization: Bearer YOUR_API_KEY" \
  -H "Content-Type: application/json" \
  -d '{
    "source": {
      "type": "inline",
      "files": [{
        "path": "test.sol",
        "content": "pragma solidity ^0.8.0; contract Test {}"
      }]
    }
  }'
```
- [ ] Receive response with `"decision": "ALLOW"`
- [ ] No 401 Unauthorized errors

## ✅ Unity Configuration

### Open Project
- [ ] Open Unity Hub
- [ ] Open project at `Spacial_Ink_Studio_ARDrawing/ARDrawingQuest`
- [ ] Wait for Unity to import assets
- [ ] No import errors
- [ ] Meta XR SDK imported correctly

### Create Blockchain System GameObject
- [ ] Open main scene (SampleScene or your AR scene)
- [ ] Create Empty GameObject: `GameObject → Create Empty`
- [ ] Rename to: `BlockchainSystem`
- [ ] Position at (0, 0, 0)

### Add Components
- [ ] Select `BlockchainSystem` GameObject
- [ ] Add Component: `BlockchainManager`
- [ ] Add Component: `KairoSecurityCheck`
- [ ] Add Component: `DrawingSaveManager`

### Configure BlockchainManager Component
- [ ] **Contract Address**: Paste address from deployment step
  - Example: `0x5FbDB2315678afecb367f032d93F642f64180aa3`
- [ ] **RPC URL**: 
  - For Unity Editor: `http://localhost:8545`
  - For Quest: `http://YOUR_PC_IP:8545` (find with `ipconfig` or `ifconfig`)
- [ ] **Chain ID**: `31337` (Hardhat default) or `1337`
- [ ] **Network Name**: `localhost`
- [ ] **Wallet Address**: Use Account #0 from Hardhat node
  - Example: `0xf39Fd6e51aad88F6F4ce6aB8827279cffFb92266`
- [ ] **Private Key**: Use Private Key from Account #0
  - Example: `0xac0974bec39a17e36ba4a6b4d238ff944bacb478cbed5efcae784d7bf4f2ff80`
  - ⚠️ WARNING: Never use real keys with funds!
- [ ] **Metadata Base URL**:
  - For Unity Editor: `http://localhost:3000/metadata`
  - For Quest: `http://YOUR_PC_IP:3000/metadata`
- [ ] **Artist ID**: Your artist name (e.g., "SpatialInkArtist")

### Configure KairoSecurityCheck Component
- [ ] **API Key**: Paste your Kairo API key
  - Format: `kairo_sk_live_xxxxx`
- [ ] **API Base URL**: `https://api.kairoaisec.com/v1` (default)
- [ ] **Severity Threshold**: `high`
- [ ] **Include Suggestions**: ✓ (checked)
- [ ] **Block On Security Issues**: ✓ (checked)

### Configure DrawingSaveManager Component
- [ ] **Save Button**: Link to your UI Save button (if exists)
- [ ] **Status Text**: Link to your UI status text (if exists)
- [ ] **Saving Panel**: Link to your UI loading panel (if exists)
- [ ] **Drawing Manager**: Drag `DrawingManager` component/GameObject
- [ ] **Blockchain Manager**: Drag `BlockchainSystem` GameObject
- [ ] **Center Point**: Drag Main Camera transform
- [ ] **Save Local JSON**: ✓ (checked)
- [ ] **Claim On Blockchain**: ✓ (checked)
- [ ] **Check Availability**: ✓ (checked)

### Verify Component Links
- [ ] All required references are assigned (no "None" or "Missing")
- [ ] No red errors in Console
- [ ] No missing script warnings

## ✅ Testing in Unity Editor

### Play Mode Test
- [ ] Enter Play Mode in Unity Editor
- [ ] No errors in Console on start
- [ ] See initialization logs:
  - `[Blockchain] Initialized - Contract: 0x...`
  - `[Kairo] ...`
- [ ] Draw something with keyboard/mouse input (if KeyboardInputProvider is active)
- [ ] Manually trigger save (or create test button)

### Test Save Functionality
- [ ] Call `DrawingSaveManager.SaveDrawing()` from test button
- [ ] Check Console for save progress:
  - `[SaveManager] Saving drawing with X strokes`
  - `[Blockchain] Step 1/3: Uploading metadata...`
  - `[Blockchain] Metadata uploaded: http://...`
  - `[Blockchain] Step 2/3: Preparing blockchain transaction...`
  - `[Blockchain] Step 3/3: Sending transaction...`
  - `[Blockchain] ✅ Location claimed successfully!`
  - `[Blockchain] Transaction Hash: 0x...`
- [ ] No red errors
- [ ] Transaction hash displayed

### Verify Metadata Upload
- [ ] Check `Contracts/metadata/` directory
- [ ] See new file: `metadata_TIMESTAMP.json`
- [ ] Open file and verify JSON structure
- [ ] Contains stroke data, coordinates, artist ID

### Verify Blockchain State
- [ ] Open new terminal
- [ ] Run: `npx hardhat console --network localhost`
- [ ] In console:
```javascript
const GraffitiAnchor = await ethers.getContractFactory("GraffitiAnchor")
const contract = await GraffitiAnchor.attach("YOUR_CONTRACT_ADDRESS")
const claimCount = await contract.nextClaimId()
console.log("Total claims:", claimCount.toString())
```
- [ ] See claim count increased (should be > 1)
- [ ] Query specific claim:
```javascript
const claim = await contract.getClaim(1)
console.log(claim)
```
- [ ] See claim data with coordinates and metadata URI

## ✅ Testing on Quest Headset

### Network Configuration
- [ ] Quest and PC are on same WiFi network
- [ ] Find PC's local IP address:
  - macOS: `ipconfig getifaddr en0`
  - Windows: `ipconfig` (look for IPv4 Address)
  - Linux: `ifconfig` or `ip addr`
- [ ] **SAVE YOUR PC IP** (e.g., `192.168.1.100`)

### Update Unity Settings for Quest
- [ ] Select `BlockchainSystem` GameObject
- [ ] **BlockchainManager → RPC URL**: `http://YOUR_PC_IP:8545`
- [ ] **BlockchainManager → Metadata Base URL**: `http://YOUR_PC_IP:3000/metadata`
- [ ] Keep all other settings the same

### Build Settings
- [ ] File → Build Settings
- [ ] Platform: Android
- [ ] Switch Platform (if needed)
- [ ] Texture Compression: ASTC
- [ ] Run Device: Your Quest device
- [ ] Build And Run

### Test on Quest
- [ ] App launches on Quest
- [ ] No crashes
- [ ] Draw in AR with controllers
- [ ] Trigger save (via button or controller input)
- [ ] Check PC terminals for activity:
  - Metadata server logs: `✅ Metadata saved: ...`
  - Hardhat node logs: Transaction mined
- [ ] Optional: Use Quest browser to check `http://YOUR_PC_IP:3000/metadata`

### Verify Quest Save
- [ ] On PC, check `Contracts/metadata/` for new files
- [ ] On PC, use Hardhat console to verify blockchain state
- [ ] Check claim count increased
- [ ] Verify claim data matches drawing

## ✅ Final Verification

### Check All Services Running
- [ ] Terminal 1: Hardhat node running (no errors)
- [ ] Terminal 2: Metadata server running (no errors)
- [ ] Unity: No errors in Console
- [ ] Quest: App running smoothly

### Test Complete Flow
- [ ] Create drawing in AR
- [ ] Press Save button
- [ ] See "Uploading metadata..." status
- [ ] See "Claiming location on blockchain..." status
- [ ] See "Success!" message with transaction hash
- [ ] Verify in Hardhat console
- [ ] Verify metadata file exists

### Security Verification
- [ ] Kairo API key is valid
- [ ] No API errors in console
- [ ] Optional: Run manual security check:
```csharp
// In Unity script or console
string contractCode = System.IO.File.ReadAllText("path/to/GraffitiAnchor.sol");
kairoSecurityCheck.AnalyzeContract(contractCode, "GraffitiAnchor.sol", (result) => {
    Debug.Log($"Security: {result.decision}, Risk: {result.risk_score}");
});
```
- [ ] See `"decision": "ALLOW"` or `"WARN"`

## ✅ Common Issues Resolved

### Issue: "Cannot connect to localhost"
- [ ] ✓ Using PC IP instead of localhost for Quest
- [ ] ✓ PC firewall allows connections on ports 8545 and 3000
- [ ] ✓ Quest and PC on same network

### Issue: "Contract not deployed"
- [ ] ✓ Hardhat node is running
- [ ] ✓ Contract was deployed without errors
- [ ] ✓ Contract address is correct in Unity
- [ ] ✓ Tried redeploying: `npx hardhat run scripts/deploy.js --network localhost`

### Issue: "Metadata upload failed"
- [ ] ✓ Metadata server is running on port 3000
- [ ] ✓ URL is correct in Unity
- [ ] ✓ No firewall blocking port 3000
- [ ] ✓ Tested with curl command successfully

### Issue: "Kairo API error 401"
- [ ] ✓ API key format is correct: `kairo_sk_live_xxxxx`
- [ ] ✓ No extra spaces or quotes around API key
- [ ] ✓ API key is active in Kairo dashboard
- [ ] ✓ Tested with curl command successfully

## ✅ Documentation Review

- [ ] Read [QUICKSTART.md](QUICKSTART.md)
- [ ] Read [BLOCKCHAIN_SETUP.md](BLOCKCHAIN_SETUP.md)
- [ ] Review [ARCHITECTURE.md](ARCHITECTURE.md)
- [ ] Understand [IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)
- [ ] Checked [PROJECT_README.md](PROJECT_README.md)

## ✅ Ready for Development

- [ ] All services running without errors
- [ ] Successfully saved at least one drawing
- [ ] Blockchain state verified
- [ ] Metadata storage working
- [ ] Quest build and deployment working
- [ ] Understanding of complete flow
- [ ] Ready to add features or deploy to production

---

## 🎉 Completion Status

Total items: 100+
Completed: ____ / 100+

**When all items are checked, you're ready to:**
- [ ] Demo the app
- [ ] Deploy to testnet
- [ ] Add new features
- [ ] Share with others

---

**Need Help?**
- Review troubleshooting in [BLOCKCHAIN_SETUP.md](BLOCKCHAIN_SETUP.md)
- Check Unity Console for errors
- Verify all terminals are running
- Test each component independently
