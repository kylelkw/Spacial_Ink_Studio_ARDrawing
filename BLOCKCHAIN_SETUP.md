# Kairo Blockchain Integration for Spatial Ink Studio

## Overview

This blockchain integration allows users to permanently store their AR graffiti artwork on the Kairo blockchain, turning ephemeral digital art into owned, tradeable digital assets.

## Features

- **On-Chain Location Claiming**: Store artwork XYZ coordinates and artist ID on Kairo blockchain
- **IPFS Storage**: Full artwork data stored on IPFS with hash reference on-chain
- **Ownership Verification**: Each artwork is tied to the creator's wallet address
- **Location Uniqueness**: Physical locations can only be claimed once (with configurable precision)
- **Permanent Storage**: Artworks are immutable and permanently stored

## Architecture

### Smart Contract (`GraffitiAnchor.sol`)

The smart contract handles:
- Location claiming with coordinate precision
- Artist ownership tracking
- IPFS hash storage
- Location availability checking
- Graffiti querying and listing

### Unity Integration

Three main components:

1. **BlockchainManager.cs**: Core blockchain interaction logic
2. **BlockchainConfig.cs**: Configuration for network and contract settings
3. **BlockchainSaveUI.cs**: User interface for saving artwork to blockchain

---

## Setup Instructions

### Step 1: Install Dependencies

```bash
cd Contracts
npm install
```

This will install:
- Hardhat (Ethereum development environment)
- OpenZeppelin contracts (security & standards)
- Ethers.js (blockchain interaction)

### Step 2: Configure Environment

1. Copy the example environment file:
```bash
cp .env.example .env
```

2. Edit `.env` with your configuration:
```bash
# Get these from Kairo documentation
KAIRO_RPC_URL=https://your-kairo-rpc-url
KAIRO_CHAIN_ID=your-chain-id

# Your wallet private key (KEEP SECRET!)
PRIVATE_KEY=your_private_key_without_0x

# Optional: IPFS credentials
IPFS_API_URL=https://ipfs.infura.io:5001
IPFS_API_KEY=your_ipfs_project_id
IPFS_API_SECRET=your_ipfs_secret
```

⚠️ **IMPORTANT**: Never commit your `.env` file! It's already in `.gitignore`.

### Step 3: Deploy Smart Contract

#### Local Testing (Hardhat Network)

```bash
# Start local blockchain
npx hardhat node

# In another terminal, deploy
npx hardhat run scripts/deploy.js --network localhost
```

#### Deploy to Kairo Testnet

```bash
npx hardhat run scripts/deploy.js --network kairoTestnet
```

#### Deploy to Kairo Mainnet (Production)

```bash
npx hardhat run scripts/deploy.js --network kairoMainnet
```

**Save the deployed contract address!** You'll need it for Unity configuration.

### Step 4: Configure Unity

1. Open Unity and navigate to your Assets folder
2. Find the `BlockchainConfig` ScriptableObject (or create one via Assets → Create → Spatial Ink → Blockchain Config)
3. Configure the settings:

```
RPC URL: [Your Kairo RPC URL]
Chain ID: [Your Kairo Chain ID]
Contract Address: [Deployed contract address from Step 3]
IPFS API URL: [Your IPFS endpoint]
Test Mode: ☑️ (Check for testing without real blockchain)
```

4. Attach the `BlockchainManager` to a GameObject in your scene
5. Assign the `BlockchainConfig` to the manager
6. Add the `BlockchainSaveUI` component to your UI canvas
7. Connect UI elements (buttons, text fields) to the `BlockchainSaveUI` component

### Step 5: Unity Setup Details

#### Required Components

1. **BlockchainManager** (Singleton)
   - Place on a persistent GameObject (e.g., GameManager)
   - Assign BlockchainConfig ScriptableObject
   - This handles all blockchain interactions

2. **BlockchainSaveUI** (UI Controller)
   - Place on your Canvas or UI parent
   - Link to BlockchainManager
   - Connect UI elements:
     - Save to Blockchain Button
     - Connect Wallet Button
     - Status Text
     - Wallet Address Text
     - Loading Panel

3. **Drawing Anchor**
   - Reference to your AR drawing's anchor transform
   - This provides the physical location for blockchain storage

#### Example Scene Setup

```
Scene Hierarchy:
├── XR Origin (AR)
├── GameManager
│   └── BlockchainManager ← Script attached here
├── Canvas
│   └── BlockchainUI
│       ├── ConnectWalletButton
│       ├── SaveToBlockchainButton
│       ├── StatusText
│       ├── WalletAddressText
│       └── LoadingPanel
└── DrawingAnchor ← Your AR drawing parent
```

---

## Usage Flow

### For Players (Meta Quest 3)

1. **Create AR Artwork**: Draw in 3D space using your app
2. **Connect Wallet**: Tap "Connect Wallet" button (generates wallet in test mode)
3. **Save to Blockchain**: Tap "Save to Blockchain"
4. **Transaction Process**:
   - App checks if location is available
   - Uploads artwork data to IPFS
   - Sends transaction to Kairo blockchain
   - Shows transaction hash when complete
5. **Ownership**: Artwork is now permanently owned by the creator's wallet

### For Developers

#### Test Mode (No Blockchain Required)

Set `testMode = true` in BlockchainConfig to simulate blockchain interactions without real transactions. Perfect for development and testing.

#### Production Mode

1. Set `testMode = false`
2. Ensure contract is deployed to Kairo mainnet
3. Update `contractAddress` with mainnet address
4. Implement wallet connection (see Integration section)

---

## Integration with Web3 Libraries

The current implementation provides a framework. To connect to real blockchain:

### Option 1: Nethereum (Recommended for Unity)

```bash
# In Unity, install via Package Manager or import
# NuGet for Unity: com.github-glitchenzo.nugetforunity
```

```csharp
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

// In BlockchainManager.Initialize()
var account = new Account(privateKey);
var web3 = new Web3(account, config.rpcUrl);
var contract = web3.Eth.GetContract(contractABI, config.contractAddress);
```

### Option 2: ChainSafe Gaming SDK

```bash
# Download from: https://github.com/ChainSafe/web3.unity
# Import into Unity
```

Provides ready-made wallet connection and transaction handling for Unity.

### Option 3: WalletConnect for Quest

For Meta Quest 3, use WalletConnect with QR code or deep linking:
- Display QR code on PC/phone
- User scans with wallet app
- Transactions signed on mobile device

---

## Smart Contract Functions

### Main Functions

#### `claimLocation(int256 x, int256 y, int256 z, string artworkHash)`
Claims a physical location and stores artwork reference.

**Parameters:**
- `x`, `y`, `z`: Coordinates in millimeters
- `artworkHash`: IPFS hash of artwork data

**Returns:** Location hash (bytes32)

**Events:** Emits `GraffitiClaimed`

#### `isLocationAvailable(int256 x, int256 y, int256 z)`
Checks if a location can be claimed.

**Returns:** `true` if available, `false` if already claimed

#### `getGraffiti(bytes32 locationHash)`
Retrieves artwork data for a location.

**Returns:** Tuple of (x, y, z, artist, artworkHash, timestamp, isActive)

#### `updateGraffiti(bytes32 locationHash, string newArtworkHash)`
Allows artist to update their artwork (only original artist).

#### `getArtistGraffitis(address artist)`
Gets all location hashes claimed by an artist.

**Returns:** Array of bytes32 location hashes

---

## IPFS Integration

Artwork data is stored on IPFS for decentralized, permanent storage:

### What Gets Stored on IPFS:
- Full 3D stroke data (points, colors, widths)
- Artist information
- Timestamp
- Anchor position and rotation
- Metadata (title, description)

### What Gets Stored on Blockchain:
- XYZ coordinates (location claim)
- IPFS hash (reference to full data)
- Artist wallet address
- Timestamp
- Active status

### IPFS Services:

**Infura IPFS** (Easiest)
```bash
Project ID: your_project_id
Project Secret: your_project_secret
Endpoint: https://ipfs.infura.io:5001
```

**Pinata** (Good UI)
```bash
API Key: your_pinata_api_key
API Secret: your_pinata_secret
Endpoint: https://api.pinata.cloud
```

**Web3.Storage** (Free)
```bash
API Token: your_web3_storage_token
Endpoint: https://api.web3.storage
```

---

## Security Considerations

### Private Keys
- **Never** hardcode private keys in Unity
- **Never** commit `.env` files
- For production, use secure key management:
  - WalletConnect (user's own wallet)
  - Cloud key management (AWS KMS, Google Cloud KMS)
  - Hardware security modules

### Smart Contract Security
- Contract uses OpenZeppelin's secure contracts
- ReentrancyGuard prevents reentrancy attacks
- Ownable provides admin controls
- Location hashing prevents coordinate manipulation

### Gas Optimization
- Events indexed for efficient querying
- Minimal on-chain storage
- Batch operations where possible

---

## Testing

### Unit Tests

Create tests in `Contracts/test/GraffitiAnchor.test.js`:

```bash
npx hardhat test
```

### Unity Testing

Enable test mode in BlockchainConfig:
```
Test Mode: ☑️
```

This simulates all blockchain operations locally.

---

## Troubleshooting

### "Location already claimed"
- Location precision is set to 1 meter by default
- Two artworks within 1 meter are considered same location
- Adjust `locationPrecision` in smart contract or move artwork

### "Transaction failed"
- Check wallet has enough KAIRO tokens for gas
- Verify RPC URL is correct
- Check network connection
- Look at transaction hash in block explorer

### "IPFS upload failed"
- Verify IPFS credentials
- Check IPFS service is operational
- Try alternative IPFS service
- Check artwork data size (some services have limits)

### Unity Compilation Errors
- Install TextMeshPro if not already installed
- Ensure .NET 4.x or later is set in Player Settings
- Install required Web3 packages

---

## Roadmap

### Phase 1 (Current)
- ✅ Smart contract deployment
- ✅ Basic Unity integration
- ✅ Test mode for development

### Phase 2 (Next Steps)
- [ ] Full Web3 library integration
- [ ] WalletConnect for Quest 3
- [ ] IPFS upload implementation
- [ ] Gallery view of claimed locations

### Phase 3 (Future)
- [ ] NFT minting for artworks
- [ ] Artwork marketplace
- [ ] Social features (likes, follows)
- [ ] AR artwork discovery/navigation

---

## Cost Estimates

### Gas Costs (Estimated)
- Claim Location: ~100,000 gas
- Update Graffiti: ~50,000 gas
- At 20 gwei and ETH = $2000: ~$4-8 per artwork

**Note:** Actual costs depend on Kairo gas prices and network congestion.

### IPFS Storage
- Most services: Free for reasonable usage
- Infura: 5GB free, then $0.08/GB/month
- Pinata: 1GB free, then $20/month for 20GB

---

## Support & Resources

### Kairo Blockchain
- Documentation: [Kairo Docs URL]
- Discord: [Kairo Discord]
- Block Explorer: [Kairo Explorer URL]

### Development Tools
- Hardhat: https://hardhat.org/docs
- OpenZeppelin: https://docs.openzeppelin.com/
- Nethereum: https://docs.nethereum.com/

### Unity Web3
- ChainSafe Gaming SDK: https://gaming.chainsafe.io/
- WalletConnect: https://walletconnect.com/

---

## Contributing

This is a feature branch implementation. To contribute:

1. Create a feature branch: `git checkout -b feature/your-feature`
2. Make changes and test thoroughly
3. Update documentation
4. Submit pull request

---

## License

[Your License Here]

---

## Contact

For questions or issues:
- GitHub Issues: [Your repo]
- Discord: [Your Discord]
- Email: [Your email]

---

**Happy Building! 🎨⛓️**
