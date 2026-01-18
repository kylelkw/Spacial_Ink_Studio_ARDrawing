# Spatial Ink Studio - AR Drawing with Blockchain

Turn your Meta Quest 3 drawings into permanent, owned digital assets on the Kairo blockchain.

## 🎨 Overview

Spatial Ink Studio is an AR drawing application for Meta Quest 3 that allows users to create 3D graffiti in physical space and permanently store their artwork on the blockchain. Each artwork is tied to its real-world GPS coordinates, creating a global map of digital graffiti.

## ✨ Features

### Drawing Features
- **Wire brush** - 3D spatial drawing that follows your hand
- **Flat brush** - Graffiti-style billboard brushes
- **8 color palette** - Full color selection
- **Analog size control** - Via joystick for precise control
- **Erase & undo** - Fix mistakes easily
- **Extensible brush system** - Easy to add new brush types

### Blockchain Features (NEW! 🔗)
- **Permanent Storage** - Save artwork to Kairo blockchain
- **Location-Based** - Artwork anchored to real-world coordinates
- **IPFS Integration** - Decentralized artwork storage
- **Wallet Integration** - Connect wallet to claim ownership
- **Immutable Art** - Once saved, permanently stored and tradeable
- **Artist Portfolio** - Track all your creations on-chain

## 🚀 Quick Start

### For Players

1. **Put on Meta Quest 3**
2. **Launch Spatial Ink Studio**
3. **Draw your artwork** in 3D space
4. **Save to blockchain** (optional) to make it permanent
5. **Share your creation** - it's now owned by you forever!

### For Developers

#### Prerequisites
- Unity 2022.3 LTS or later
- Meta Quest 3
- Node.js 16+ and npm
- Kairo wallet with tokens (for blockchain features)

#### Installation

```bash
# Clone the repository
git clone [your-repo-url]
cd Spacial_Ink_Studio_ARDrawing

# Install blockchain dependencies
cd Contracts
npm install

# Configure environment
cp .env.example .env
# Edit .env with your Kairo RPC URL and private key
```

#### Deploy Smart Contract

```bash
# Test locally
npx hardhat node
npx hardhat run scripts/deploy.js --network localhost

# Or deploy to Kairo testnet
npx hardhat run scripts/deploy.js --network kairoTestnet
```

#### Configure Unity

1. Open project in Unity 2022.3 LTS
2. Create BlockchainConfig: Assets → Create → Spatial Ink → Blockchain Config
3. Set RPC URL, Chain ID, and Contract Address
4. Build for Meta Quest 3

See [BLOCKCHAIN_SETUP.md](BLOCKCHAIN_SETUP.md) for detailed setup guide.

## 📖 Documentation

- **[BLOCKCHAIN_SETUP.md](BLOCKCHAIN_SETUP.md)** - Complete blockchain integration guide
- **[Smart Contract](Contracts/GraffitiAnchor.sol)** - Contract source code and documentation
- **[Unity Scripts](Assets/Scripts/Blockchain/)** - Unity blockchain integration

## 🏗️ Project Structure

```
Spacial_Ink_Studio_ARDrawing/
├── Assets/
│   └── Scripts/
│       └── Blockchain/
│           ├── BlockchainManager.cs      # Core blockchain logic
│           ├── BlockchainConfig.cs       # Configuration
│           ├── BlockchainSaveUI.cs       # UI controller
│           └── GraffitiData.cs           # Data structures
├── Contracts/
│   ├── GraffitiAnchor.sol               # Smart contract
│   ├── hardhat.config.js                # Hardhat configuration
│   ├── package.json                     # Dependencies
│   └── scripts/
│       └── deploy.js                    # Deployment script
├── BLOCKCHAIN_SETUP.md                  # Detailed setup guide
└── README.md                            # This file
```

## 🔧 Technology Stack

### Frontend
- Unity 2022.3 LTS
- Meta XR SDK
- C# .NET

### Blockchain
- Solidity ^0.8.20
- Hardhat
- OpenZeppelin Contracts
- Kairo Network

### Storage
- IPFS (artwork data)
- Kairo Blockchain (ownership & coordinates)

## 💡 Use Cases

- **Digital Graffiti Artists** - Create permanent, owned street art
- **AR Museums** - Curate location-based exhibitions
- **Tourist Attractions** - Leave digital "signatures" at famous locations
- **Virtual Real Estate** - Claim and decorate physical spaces
- **NFT Artists** - Create location-specific experiences

## 🗺️ Development Timeline

### Original Build (12 hours)
- **Platform:** Meta Quest 3S
- **Engine:** Unity 2022.3 LTS
- **Date:** January 17, 2026

### Blockchain Integration (Kairo Track)
- Smart contract development
- Unity Web3 integration
- IPFS storage system
- UI for blockchain saves

## 🎯 The Pitch

> "Digital graffiti is ephemeral. We use Kairo to make it permanent and tradeable, turning vandalism into owned digital assets."

## 🔒 Security

- OpenZeppelin audited contracts
- ReentrancyGuard protection
- Private keys never stored in client
- Location hashing prevents manipulation

See [BLOCKCHAIN_SETUP.md](BLOCKCHAIN_SETUP.md) for security details.

## 🚀 Roadmap

- [x] Core AR drawing features
- [x] Smart contract development
- [x] Unity blockchain integration
- [x] IPFS integration framework
- [ ] WalletConnect for Quest 3
- [ ] NFT minting
- [ ] Artwork gallery & discovery
- [ ] Social features
- [ ] Marketplace

## Team Structure
- Developer 1: XR Setup & Controller Input
- Developer 2: Drawing System
- Developer 3: UI/UX Controls
- Developer 4: Integration & Testing
- **Blockchain Integration:** Kairo Track Feature

## Development Workflow
- `main` - Production-ready code
- `dev` - Integration branch
- `feature/*` - Feature branches

## 🤝 Contributing

Contributions welcome! Please:

1. Fork the repository
2. Create feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open Pull Request

## 📄 License

[Your License Here]

## 🙏 Acknowledgments

- Kairo blockchain team
- Meta Quest SDK
- OpenZeppelin
- Unity Technologies

## 📞 Contact

- **Issues:** [GitHub Issues](your-repo-url/issues)
- **Discord:** [Your Server]
- **Email:** your-email@example.com

---

**Made with ❤️ for the Kairo Blockchain Track**

*Turning ephemeral art into permanent digital assets*


