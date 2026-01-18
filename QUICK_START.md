# Kairo Blockchain Integration - Quick Reference

## 🚀 Quick Commands

### Deploy Smart Contract

```bash
# Local testing
cd Contracts
npm install
npx hardhat node                                    # Terminal 1
npx hardhat run scripts/deploy.js --network localhost  # Terminal 2

# Testnet
npx hardhat run scripts/deploy.js --network kairoTestnet

# Mainnet
npx hardhat run scripts/deploy.js --network kairoMainnet
```

### Test Smart Contract

```bash
cd Contracts
npx hardhat test
npx hardhat coverage  # For coverage report
```

## 📁 File Locations

### Smart Contract
- **Contract:** `Contracts/GraffitiAnchor.sol`
- **Config:** `Contracts/hardhat.config.js`
- **Deploy:** `Contracts/scripts/deploy.js`
- **Environment:** `Contracts/.env` (create from `.env.example`)

### Unity Scripts
- **Manager:** `Assets/Scripts/Blockchain/BlockchainManager.cs`
- **Config:** `Assets/Scripts/Blockchain/BlockchainConfig.cs`
- **UI:** `Assets/Scripts/Blockchain/BlockchainSaveUI.cs`
- **Data:** `Assets/Scripts/Blockchain/GraffitiData.cs`

## 🔑 Key Addresses to Save

After deployment, save these:

```
Contract Address: 0x___________________________
RPC URL: https://kairo-rpc-url.com
Chain ID: [your-chain-id]
IPFS Gateway: https://ipfs.io/ipfs/
```

## ⚙️ Unity Configuration Checklist

- [ ] Create BlockchainConfig ScriptableObject
- [ ] Set RPC URL
- [ ] Set Chain ID
- [ ] Set Contract Address (after deployment)
- [ ] Set IPFS API URL
- [ ] Enable Test Mode for development
- [ ] Attach BlockchainManager to scene GameObject
- [ ] Setup BlockchainSaveUI with button references
- [ ] Link drawing anchor transform

## 🧪 Testing Workflow

1. **Local Mode (No Blockchain)**
   ```
   BlockchainConfig → testMode = ☑️
   ```

2. **Local Hardhat Network**
   ```bash
   npx hardhat node
   npx hardhat run scripts/deploy.js --network localhost
   Update Unity: contractAddress + testMode = ☐
   ```

3. **Testnet**
   ```bash
   npx hardhat run scripts/deploy.js --network kairoTestnet
   Update Unity: contractAddress + testMode = ☐
   ```

## 🔗 Integration Steps

### When User Clicks "Save to Blockchain"

1. **Check location availability**
   ```csharp
   bool available = await blockchainManager.IsLocationAvailable(position);
   ```

2. **Create drawing metadata**
   ```csharp
   DrawingMetadata metadata = new DrawingMetadata {
       title = "My Artwork",
       strokePoints = drawingPoints,
       strokeColors = colors,
       anchorPosition = position
   };
   ```

3. **Claim location**
   ```csharp
   TransactionResponse response = await blockchainManager.ClaimLocation(position, metadata);
   ```

4. **Handle response**
   ```csharp
   if (response.success) {
       Debug.Log($"Saved! TX: {response.transactionHash}");
   }
   ```

## 📊 Smart Contract Functions

### Read Functions (No Gas)
```solidity
isLocationAvailable(x, y, z) → bool
getGraffiti(locationHash) → (x, y, z, artist, hash, timestamp, isActive)
getArtistGraffitis(artist) → bytes32[]
getTotalGraffitis() → uint256
```

### Write Functions (Requires Gas)
```solidity
claimLocation(x, y, z, artworkHash) → bytes32
updateGraffiti(locationHash, newArtworkHash)
deactivateGraffiti(locationHash)
```

## 💰 Gas Estimates

- **Claim Location:** ~100,000 gas
- **Update Graffiti:** ~50,000 gas
- **Deactivate:** ~30,000 gas

## 🐛 Common Issues

### "Location already claimed"
- Another artwork exists within precision radius (default: 1m)
- Solution: Move artwork or adjust `locationPrecision`

### "Transaction failed"
- Check wallet has enough KAIRO tokens
- Verify RPC URL is correct
- Check network connection

### "IPFS upload failed"
- Verify IPFS credentials in BlockchainConfig
- Check IPFS service status
- Try alternative IPFS provider

### Unity Compilation Errors
- Install TextMeshPro
- Check .NET version in Player Settings (4.x+)
- Install required Web3 packages

## 🔐 Security Checklist

- [ ] Never commit `.env` file
- [ ] Never hardcode private keys
- [ ] Use WalletConnect for user wallets
- [ ] Test on testnet first
- [ ] Audit contract before mainnet
- [ ] Use HTTPS for RPC URLs
- [ ] Validate user inputs
- [ ] Handle transaction errors gracefully

## 📞 Need Help?

See full documentation: [BLOCKCHAIN_SETUP.md](BLOCKCHAIN_SETUP.md)

## 🎯 Production Deployment Checklist

- [ ] Smart contract audited
- [ ] Deployed to Kairo mainnet
- [ ] Contract verified on explorer
- [ ] Unity configured with mainnet address
- [ ] Test mode disabled
- [ ] WalletConnect integrated
- [ ] IPFS pinning service configured
- [ ] Error handling implemented
- [ ] User documentation created
- [ ] Support channels established

---

**Ready to save your first artwork on-chain? Follow the steps above! 🎨⛓️**
