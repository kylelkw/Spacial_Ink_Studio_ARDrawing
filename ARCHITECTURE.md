# Architecture Diagrams

## System Overview

```
┌────────────────────────────────────────────────────────────────┐
│                     META QUEST 3S HEADSET                      │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │                   Unity AR Application                    │  │
│  │                                                           │  │
│  │  ┌─────────────┐  ┌──────────────┐  ┌────────────────┐  │  │
│  │  │  Drawing    │  │  Blockchain  │  │     Kairo      │  │  │
│  │  │  Manager    │─▶│   Manager    │─▶│    Security    │  │  │
│  │  └─────────────┘  └──────────────┘  └────────────────┘  │  │
│  │         │                 │                    │         │  │
│  │         ▼                 ▼                    ▼         │  │
│  │  ┌─────────────────────────────────────────────────┐    │  │
│  │  │          DrawingSaveManager                     │    │  │
│  │  │  • Collects stroke data                         │    │  │
│  │  │  • Uploads metadata                             │    │  │
│  │  │  • Claims blockchain location                   │    │  │
│  │  └─────────────────────────────────────────────────┘    │  │
│  └──────────────────────────────────────────────────────────┘  │
└──────────────────────────┬─────────────────────────────────────┘
                           │ WiFi
                           ▼
┌────────────────────────────────────────────────────────────────┐
│                    LOCALHOST / PC SERVER                       │
│  ┌──────────────────────┐       ┌──────────────────────────┐  │
│  │  Hardhat Node        │       │  Metadata Server         │  │
│  │  Port: 8545          │       │  Port: 3000              │  │
│  │                      │       │                          │  │
│  │  • RPC endpoint      │       │  • POST /metadata        │  │
│  │  • Transaction pool  │       │  • GET /metadata/:id     │  │
│  │  • Event logs        │       │  • File storage          │  │
│  └──────────┬───────────┘       └────────────┬─────────────┘  │
│             │                                 │                │
│             ▼                                 ▼                │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │           GraffitiAnchor Smart Contract                  │ │
│  │  • claimLocation(x, y, z, metadataURI)                   │ │
│  │  • isLocationAvailable(x, y, z)                          │ │
│  │  • getArtistClaims(address)                              │ │
│  └──────────────────────────────────────────────────────────┘ │
└──────────────────────────┬─────────────────────────────────────┘
                           │ HTTPS
                           ▼
┌────────────────────────────────────────────────────────────────┐
│                   KAIRO SECURITY API (Cloud)                   │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │  POST /v1/analyze                                        │ │
│  │  • AI-powered vulnerability detection                    │ │
│  │  • Risk scoring                                          │ │
│  │  • Security recommendations                              │ │
│  └──────────────────────────────────────────────────────────┘ │
└────────────────────────────────────────────────────────────────┘
```

## Save Flow Diagram

```
┌─────────────┐
│ User Draws  │
│   in AR     │
└──────┬──────┘
       │
       ▼
┌─────────────────┐
│ User Presses    │
│  "Save" Button  │
└──────┬──────────┘
       │
       ▼
┌──────────────────────────────────────────────────────┐
│ DrawingSaveManager.SaveDrawing()                     │
└──────┬───────────────────────────────────────────────┘
       │
       ▼
┌──────────────────────────────────────────────────────┐
│ Step 1: Collect Drawing Data                         │
│  • Extract all strokes from DrawingManager           │
│  • Get positions, colors, sizes, brush types         │
│  • Calculate center position                         │
└──────┬───────────────────────────────────────────────┘
       │
       ▼
┌──────────────────────────────────────────────────────┐
│ Step 2: Save Locally (Optional)                      │
│  • Serialize to JSON                                 │
│  • Save to Application.persistentDataPath            │
│  • File: drawing_{timestamp}.json                    │
└──────┬───────────────────────────────────────────────┘
       │
       ▼
┌──────────────────────────────────────────────────────┐
│ Step 3: Check Location Availability (Optional)       │
│  • Convert position to fixed-point (x1e6)            │
│  • Query: isLocationAvailable(x, y, z)               │
│  • Wait for blockchain response                      │
└──────┬───────────────────────────────────────────────┘
       │
       ├─── Not Available ───▶ Show Error & Exit
       │
       ▼ Available
┌──────────────────────────────────────────────────────┐
│ Step 4: Upload Metadata                              │
│  • POST to http://localhost:3000/metadata            │
│  • Payload: { artistId, x, y, z, strokes, ... }     │
│  • Receive: { uri: "http://localhost:3000/.json" }  │
└──────┬───────────────────────────────────────────────┘
       │
       ▼
┌──────────────────────────────────────────────────────┐
│ Step 5: Prepare Blockchain Transaction               │
│  • Convert coordinates to int256 (multiply by 1e6)   │
│  • Build function call:                              │
│    claimLocation(artistId, x, y, z, metadataURI)    │
│  • Sign with private key                             │
└──────┬───────────────────────────────────────────────┘
       │
       ▼
┌──────────────────────────────────────────────────────┐
│ Step 6: Send Transaction                             │
│  • Send to RPC: http://localhost:8545                │
│  • Wait for transaction confirmation                 │
│  • Receive transaction hash                          │
└──────┬───────────────────────────────────────────────┘
       │
       ▼
┌──────────────────────────────────────────────────────┐
│ Step 7: Store Claim Locally                          │
│  • Save claim ID, transaction hash, timestamp        │
│  • File: claim_{claimId}_{timestamp}.json            │
└──────┬───────────────────────────────────────────────┘
       │
       ▼
┌──────────────────────────────────────────────────────┐
│ Step 8: Display Success                              │
│  • Show transaction hash in UI                       │
│  • Log to console                                    │
│  • Fire OnClaimSuccess event                         │
└──────────────────────────────────────────────────────┘
```

## Smart Contract State Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                    GraffitiAnchor Contract                   │
└─────────────────────────────────────────────────────────────┘

State Variables:
┌──────────────────────────────────────────────────────────┐
│ mapping(uint256 => GraffitiClaim) public claims;        │
│ mapping(bytes32 => uint256) public locationToClaim;     │
│ mapping(address => uint256[]) public artistClaims;      │
│ uint256 public nextClaimId;                             │
└──────────────────────────────────────────────────────────┘

Claim Lifecycle:
┌─────────────┐
│  Location   │
│ Unclaimed   │
└──────┬──────┘
       │ claimLocation()
       ▼
┌─────────────────────┐
│  Location Claimed   │
│  isActive = true    │
│  • Has artist       │
│  • Has metadata URI │
│  • Has coordinates  │
└──────┬─────┬────────┘
       │     │
       │     │ updateMetadata()
       │     └────────────────────┐
       │                          │
       │ revokeClaim()            ▼
       ▼                    ┌──────────────┐
┌─────────────────┐         │  Metadata    │
│   Location      │         │   Updated    │
│   Revoked       │         └──────────────┘
│ isActive = false│
└─────────────────┘
       │
       │ (Can be reclaimed)
       ▼
┌─────────────┐
│  Available  │
│  for New    │
│   Claim     │
└─────────────┘
```

## Data Flow Diagram

```
┌──────────────────────────────────────────────────────────────┐
│                      Unity Application                        │
└──────────────────────────────────────────────────────────────┘
       │
       │ 1. Stroke Data (Vector3[], Color, float)
       ▼
┌──────────────────────────────────────────────────────────────┐
│                   DrawingSaveManager                          │
│  Transforms: Vector3 → JSON → DrawingMetadata               │
└──────────────────────────────────────────────────────────────┘
       │
       │ 2. DrawingMetadata (JSON)
       ▼
┌──────────────────────────────────────────────────────────────┐
│                    Metadata Server                            │
│  Stores: metadata_{timestamp}.json                           │
│  Returns: URI (http://localhost:3000/metadata/...)           │
└──────────────────────────────────────────────────────────────┘
       │
       │ 3. Metadata URI (string)
       ▼
┌──────────────────────────────────────────────────────────────┐
│                   BlockchainManager                           │
│  Converts: Vector3 → int256 (multiply by 1e6)               │
│  Builds: Transaction data                                    │
└──────────────────────────────────────────────────────────────┘
       │
       │ 4. Transaction (function call + signature)
       ▼
┌──────────────────────────────────────────────────────────────┐
│                    Hardhat Node (RPC)                         │
│  Processes: Transaction                                      │
│  Mines: Block with transaction                               │
└──────────────────────────────────────────────────────────────┘
       │
       │ 5. Transaction Hash + Claim ID
       ▼
┌──────────────────────────────────────────────────────────────┐
│                   GraffitiAnchor Contract                     │
│  Stores:                                                     │
│  • Claim ID → GraffitiClaim struct                           │
│  • Location hash → Claim ID                                  │
│  • Artist address → [Claim IDs]                              │
│  Emits: GraffitiClaimed event                                │
└──────────────────────────────────────────────────────────────┘
       │
       │ 6. Transaction Receipt
       ▼
┌──────────────────────────────────────────────────────────────┐
│                   Unity Application                           │
│  Displays: Transaction hash, Claim ID                        │
│  Stores Locally: claim_{id}.json                             │
└──────────────────────────────────────────────────────────────┘
```

## Security Flow with Kairo

```
┌─────────────────┐
│  Smart Contract │
│   Source Code   │
└────────┬────────┘
         │
         ▼
┌──────────────────────────────────────────────────┐
│  KairoSecurityCheck.AnalyzeContract()            │
│  • Reads contract source                         │
│  • Builds API request                            │
└────────┬─────────────────────────────────────────┘
         │
         ▼ POST /v1/analyze
┌──────────────────────────────────────────────────┐
│         Kairo AI Security API                     │
│  • Parses Solidity code                          │
│  • Runs AI analysis                              │
│  • Checks vulnerability database                 │
│  • Generates risk score                          │
└────────┬─────────────────────────────────────────┘
         │
         ▼ JSON Response
┌──────────────────────────────────────────────────┐
│  Security Analysis Result                        │
│  {                                               │
│    decision: "ALLOW" | "WARN" | "BLOCK",        │
│    risk_score: 0-100,                           │
│    summary: {                                   │
│      critical: 0,                               │
│      high: 0,                                   │
│      medium: 0,                                 │
│      low: 0                                     │
│    }                                            │
│  }                                              │
└────────┬─────────────────────────────────────────┘
         │
         ▼
┌────────────────────────────────────────────────────┐
│  Decision Logic                                    │
│  • ALLOW → Proceed with deployment                │
│  • WARN  → Log warnings, proceed with caution     │
│  • BLOCK → Stop deployment, show issues           │
│  • ESCALATE → Require human review                │
└────────┬───────────────────────────────────────────┘
         │
         ├─── ALLOW ───▶ Deploy Contract
         │
         └─── BLOCK ───▶ Show Errors & Stop
```

## Component Dependencies

```
DrawingManager (Existing)
         │
         ▼ Used by
DrawingSaveManager (NEW)
         │
         ├────▶ BlockchainManager (NEW)
         │              │
         │              ├────▶ KairoSecurityCheck (NEW)
         │              │
         │              └────▶ UnityWebRequest (Unity API)
         │
         └────▶ File System (Unity API)


External Dependencies:
• Hardhat Node (localhost:8545)
• Metadata Server (localhost:3000)
• Kairo API (api.kairoaisec.com)
• Ethereum RPC
```

## Network Topology

```
Development Setup:

┌─────────────────────┐
│   Meta Quest 3S     │
│   192.168.1.50      │
└──────────┬──────────┘
           │ WiFi
           │
┌──────────▼───────────────────────────────┐
│         Home Router                      │
│         192.168.1.1                      │
└──────────┬───────────────────────────────┘
           │
           │
┌──────────▼──────────┐
│  Development PC     │
│  192.168.1.100      │
│                     │
│  ┌───────────────┐  │
│  │ Hardhat Node  │  │
│  │ :8545         │  │
│  └───────────────┘  │
│                     │
│  ┌───────────────┐  │
│  │ Metadata Srvr │  │
│  │ :3000         │  │
│  └───────────────┘  │
└─────────────────────┘


Production Setup:

┌─────────────────────┐
│   Meta Quest 3S     │
└──────────┬──────────┘
           │ Internet
           │
┌──────────▼──────────────────────┐
│  Ethereum Network (Mainnet)     │
│  • RPC: https://mainnet...      │
│  • Contract: 0x...              │
└─────────────────────────────────┘
           │
           │
┌──────────▼──────────┐
│  IPFS Network       │
│  • Metadata storage │
│  • ipfs://Qm...     │
└─────────────────────┘
           │
           │
┌──────────▼──────────┐
│  Kairo API          │
│  • Security checks  │
│  • Risk analysis    │
└─────────────────────┘
```

## File Storage Structure

```
Application.persistentDataPath/
├── drawings/
│   ├── drawing_1234567890.json
│   ├── drawing_1234567891.json
│   └── ...
│
├── claims/
│   ├── claim_1_1234567890.json
│   ├── claim_2_1234567891.json
│   └── ...
│
└── graffiti_metadata/
    ├── metadata_1234567890.json
    ├── metadata_1234567891.json
    └── ...

Contracts/metadata/ (Server storage)
├── metadata_1234567890.json
├── metadata_1234567891.json
└── ...
```

---

These diagrams show the complete architecture and data flow of the blockchain-integrated AR drawing system.
