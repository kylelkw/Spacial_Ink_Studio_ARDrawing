# 🎨 Spatial Ink Studio - AR Drawing with Blockchain

**"Making digital graffiti permanent and tradeable"**

Meta Quest AR drawing application with blockchain integration, turning spatial art into owned digital assets.

---

## 🚀 Features

### Core Drawing Features
- ✅ **Wire Brush** - 3D spatial drawing with tube renderer
- ✅ **Flat Brush** - Graffiti-style billboard rendering
- ✅ **8 Color Palette** - Curated color selection
- ✅ **Analog Size Control** - Joystick-based brush sizing
- ✅ **Erase & Undo** - Full drawing management
- ✅ **Extensible Brush System** - Easy to add new brush types

### 🔗 Blockchain Features (Kairo Track)
- ✅ **GraffitiAnchor Smart Contract** - Claim physical locations on-chain
- ✅ **Kairo Security Integration** - AI-powered smart contract validation
- ✅ **Metadata Storage** - Persistent drawing data with IPFS-ready architecture
- ✅ **Location Claiming** - Turn vandalism into owned digital assets
- ✅ **Artist Verification** - On-chain proof of authorship

---

## 📋 Project Info

- **Platform:** Meta Quest 3S
- **Engine:** Unity 2022.3 LTS  
- **Blockchain:** Ethereum-compatible (tested on localhost/Hardhat)
- **Security:** Kairo AI Security Platform
- **Development Time:** 12 hours (core) + blockchain integration

---

## 🏗️ Architecture

```
┌─────────────────────────────────────┐
│       Meta Quest 3S Headset         │
│  ┌──────────────────────────────┐   │
│  │  Unity AR Drawing App        │   │
│  │  • DrawingManager            │   │
│  │  • BlockchainManager         │   │
│  │  • KairoSecurityCheck        │   │
│  │  • DrawingSaveManager        │   │
│  └──────────┬───────────────────┘   │
└─────────────┼───────────────────────┘
              │ WiFi
              ▼
     ┌────────────────┐
     │  Localhost PC  │
     ├────────────────┤
     │ • Hardhat Node │ (Port 8545)
     │ • Metadata API │ (Port 3000)
     └────────┬───────┘
              │
              ▼
    ┌──────────────────┐
    │  Kairo Security  │
    │  API (Cloud)     │
    └──────────────────┘
              │
              ▼
    ┌──────────────────┐
    │  Smart Contract  │
    │  GraffitiAnchor  │
    │  (On-chain)      │
    └──────────────────┘
```

---

## 🎯 How It Works

### The Digital Graffiti Flow

1. **Draw in AR** - User creates spatial art with Quest controllers
2. **Press Save** - Triggers blockchain claiming process
3. **Upload Metadata** - Drawing data stored on metadata server (future: IPFS)
4. **Security Check** - Kairo validates contract before transaction
5. **Claim Location** - Smart contract records XYZ coordinates + artist ID
6. **Mint Asset** - Location becomes a tradeable digital asset on-chain

### Smart Contract: GraffitiAnchor

```solidity
struct GraffitiClaim {
    address artist;      // Ethereum address
    string artistId;     // Custom identifier
    int256 x, y, z;      // World coordinates (1e6 precision)
    uint256 timestamp;   // Claim time
    string metadataURI;  // Drawing data location
    bool isActive;       // Revocation status
}
```

**Key Functions:**
- `claimLocation()` - Claim a physical location with art
- `updateMetadata()` - Update drawing data URI
- `revokeClaim()` - Remove your graffiti
- `isLocationAvailable()` - Check if location is claimed

---

## 🛠️ Setup

### Quick Start (5 Minutes)

See **[QUICKSTART.md](QUICKSTART.md)** for fastest setup.

### Full Setup

See **[BLOCKCHAIN_SETUP.md](BLOCKCHAIN_SETUP.md)** for comprehensive guide.

### Prerequisites

- Unity 2022.3+ with Meta XR SDK 68.0.0
- Node.js 16+
- Hardhat or Foundry
- Kairo API Key ([Get one here](https://kairoaisec.com))

### Installation

```bash
# 1. Clone repository
git clone <repo-url>
cd Spacial_Ink_Studio_ARDrawing

# 2. Install smart contract dependencies
cd Contracts
npm install

# 3. Start local blockchain
npx hardhat node

# 4. Deploy contract (new terminal)
npx hardhat run scripts/deploy.js --network localhost

# 5. Start metadata server (new terminal)
node metadata-server.js

# 6. Open Unity project
# Configure BlockchainManager with contract address from step 4
```

---

## 🗂️ Project Structure

```
Spacial_Ink_Studio_ARDrawing/
├── ARDrawingQuest/                    # Unity project
│   └── Assets/
│       └── DrawingSystem/
│           └── Scripts/
│               ├── Core/              # Drawing engine
│               ├── Brushes/           # Brush implementations
│               ├── Colors/            # Color system
│               └── Blockchain/        # Blockchain integration ⭐
│                   ├── BlockchainManager.cs
│                   ├── KairoSecurityCheck.cs
│                   └── DrawingSaveManager.cs
│
├── Contracts/                         # Smart contracts ⭐
│   ├── GraffitiAnchor.sol            # Main contract
│   ├── hardhat.config.js             # Hardhat setup
│   ├── metadata-server.js            # Metadata API
│   ├── scripts/
│   │   └── deploy.js                 # Deployment
│   └── test/
│       └── GraffitiAnchor.test.js    # Contract tests
│
├── BLOCKCHAIN_SETUP.md               # Full setup guide
├── QUICKSTART.md                     # Quick start guide
└── README.md                         # This file
```

---

## 🔒 Security with Kairo

Every contract interaction is validated by Kairo's AI security platform:

```csharp
// Automatic security check before deployment
kairoSecurity.AnalyzeContract(contractCode, "GraffitiAnchor.sol", (result) =>
{
    if (result.decision == SecurityDecision.ALLOW) {
        // Safe to deploy ✅
    } else {
        // Security issues found ❌
        // Risk Score: result.risk_score
        // Issues: result.summary
    }
});
```

**Kairo checks for:**
- Critical vulnerabilities
- High-risk patterns  
- Medium/low severity issues
- Best practice violations

---

## 🧪 Testing

### Test Smart Contract

```bash
cd Contracts
npx hardhat test
```

### Test Metadata Server

```bash
curl -X POST http://localhost:3000/metadata \
  -H "Content-Type: application/json" \
  -d '{"artistId":"TestArtist","x":1.5,"y":0.5,"z":2.0,"totalStrokes":1,"strokes":[]}'
```

### Test Kairo Integration

```bash
curl -X POST https://api.kairoaisec.com/v1/analyze \
  -H "Authorization: Bearer YOUR_API_KEY" \
  -H "Content-Type: application/json" \
  -d @Contracts/GraffitiAnchor.sol
```

---

## 🎮 Usage

### In Unity Editor
1. Assign components in BlockchainSystem GameObject
2. Configure contract address and API keys
3. Run scene in editor or deploy to Quest

### On Quest Headset
1. Draw with right controller trigger
2. Use joystick to adjust size
3. Press Save button to claim on-chain
4. View transaction in console/logs

### Query Blockchain

```javascript
// Hardhat console
npx hardhat console --network localhost

const contract = await ethers.getContractAt("GraffitiAnchor", "CONTRACT_ADDRESS")

// Get claim by ID
const claim = await contract.getClaim(1)

// Get all artist claims
const claims = await contract.getArtistClaims("ARTIST_ADDRESS")

// Check location availability
const available = await contract.isLocationAvailable(x, y, z)
```

---

## 🌐 Production Deployment

### Deploy to Testnet

```bash
# Set environment variables
export GOERLI_RPC_URL="https://goerli.infura.io/v3/YOUR_KEY"
export PRIVATE_KEY="your-private-key"

# Deploy
npx hardhat run scripts/deploy.js --network goerli

# Verify on Etherscan
npx hardhat verify --network goerli CONTRACT_ADDRESS
```

### Use IPFS for Metadata

```javascript
// Replace metadata-server.js with IPFS client
const { create } = require('ipfs-http-client');
const ipfs = create({ url: 'https://ipfs.infura.io:5001' });

const { cid } = await ipfs.add(JSON.stringify(metadata));
const uri = `ipfs://${cid}`;
```

---

## 📊 Demo Stats

After running the app:

```bash
# Check metadata server stats
curl http://localhost:3000/stats

# Check blockchain claims
npx hardhat console --network localhost
> const contract = await ethers.getContractAt("GraffitiAnchor", "ADDRESS")
> const count = await contract.nextClaimId()
> console.log("Total claims:", count.toString())
```

---

## 🚧 Roadmap

- [ ] NFT minting for claims
- [ ] Marketplace for trading locations
- [ ] IPFS integration for decentralized storage
- [ ] Multi-chain support (Polygon, Arbitrum)
- [ ] AR visualization of claimed locations
- [ ] Social features (follow artists, galleries)
- [ ] Mobile app (iOS/Android AR)

---

## 🤝 Contributing

This is a hackathon project, but contributions are welcome!

1. Fork the repository
2. Create feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open Pull Request

---

## 📄 License

MIT License - feel free to use for your own projects!

---

## 🙏 Acknowledgments

- **Meta XR SDK** - Quest development framework
- **Kairo** - AI-powered smart contract security
- **Hardhat** - Ethereum development environment
- **Unity** - Game engine and AR platform

---

## 📞 Support

For issues or questions:
- Check [BLOCKCHAIN_SETUP.md](BLOCKCHAIN_SETUP.md) troubleshooting
- Review Unity console logs
- Check Hardhat node output
- Verify Kairo API status

---

**Built with ❤️ for the Kairo Blockchain Track**

*"Turning digital vandalism into permanent art"* 🎨⛓️
