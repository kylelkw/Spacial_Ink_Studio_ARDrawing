// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

/**
 * @title GraffitiAnchor
 * @dev Store spatial graffiti coordinates on-chain, making digital art permanent and tradeable
 * @notice Stores XYZ coordinates + Artist ID for AR drawings
 */
contract GraffitiAnchor {
    
    struct GraffitiClaim {
        address artist;           // Ethereum address of the artist
        string artistId;          // Custom artist identifier
        int256 x;                 // X coordinate (multiplied by 1e6 for precision)
        int256 y;                 // Y coordinate (multiplied by 1e6 for precision)
        int256 z;                 // Z coordinate (multiplied by 1e6 for precision)
        uint256 timestamp;        // When the claim was made
        string metadataURI;       // IPFS or server URI for drawing data
        bool isActive;            // Whether the claim is still active
    }
    
    // Mapping from claim ID to GraffitiClaim
    mapping(uint256 => GraffitiClaim) public claims;
    
    // Mapping from location hash to claim ID (prevents duplicate locations)
    mapping(bytes32 => uint256) public locationToClaim;
    
    // Mapping from artist address to their claim IDs
    mapping(address => uint256[]) public artistClaims;
    
    // Counter for generating unique claim IDs
    uint256 public nextClaimId;
    
    // Minimum distance between claims (in coordinate units)
    uint256 public minDistance = 100000; // 0.1 meter with 1e6 precision
    
    // Owner of the contract
    address public owner;
    
    // Events
    event GraffitiClaimed(
        uint256 indexed claimId,
        address indexed artist,
        string artistId,
        int256 x,
        int256 y,
        int256 z,
        string metadataURI
    );
    
    event ClaimRevoked(uint256 indexed claimId);
    event MetadataUpdated(uint256 indexed claimId, string newMetadataURI);
    
    modifier onlyOwner() {
        require(msg.sender == owner, "Only owner can call this");
        _;
    }
    
    modifier onlyArtist(uint256 claimId) {
        require(claims[claimId].artist == msg.sender, "Only artist can modify their claim");
        _;
    }
    
    constructor() {
        owner = msg.sender;
        nextClaimId = 1; // Start from 1, reserve 0 as invalid
    }
    
    /**
     * @dev Claim a physical location with digital graffiti
     * @param artistId Custom artist identifier
     * @param x X coordinate (multiply by 1e6 before calling)
     * @param y Y coordinate (multiply by 1e6 before calling)
     * @param z Z coordinate (multiply by 1e6 before calling)
     * @param metadataURI URI pointing to drawing metadata (IPFS/server)
     * @return claimId The ID of the newly created claim
     */
    function claimLocation(
        string memory artistId,
        int256 x,
        int256 y,
        int256 z,
        string memory metadataURI
    ) public returns (uint256) {
        // Create location hash
        bytes32 locationHash = keccak256(abi.encodePacked(x, y, z));
        
        // Check if location is already claimed
        uint256 existingClaimId = locationToClaim[locationHash];
        require(existingClaimId == 0 || !claims[existingClaimId].isActive, 
                "Location already claimed");
        
        // Create new claim
        uint256 claimId = nextClaimId++;
        
        claims[claimId] = GraffitiClaim({
            artist: msg.sender,
            artistId: artistId,
            x: x,
            y: y,
            z: z,
            timestamp: block.timestamp,
            metadataURI: metadataURI,
            isActive: true
        });
        
        locationToClaim[locationHash] = claimId;
        artistClaims[msg.sender].push(claimId);
        
        emit GraffitiClaimed(claimId, msg.sender, artistId, x, y, z, metadataURI);
        
        return claimId;
    }
    
    /**
     * @dev Update metadata URI for an existing claim
     * @param claimId The claim to update
     * @param newMetadataURI New URI for metadata
     */
    function updateMetadata(uint256 claimId, string memory newMetadataURI) 
        public 
        onlyArtist(claimId) 
    {
        require(claims[claimId].isActive, "Claim is not active");
        claims[claimId].metadataURI = newMetadataURI;
        emit MetadataUpdated(claimId, newMetadataURI);
    }
    
    /**
     * @dev Revoke a claim (artist can remove their own graffiti)
     * @param claimId The claim to revoke
     */
    function revokeClaim(uint256 claimId) public onlyArtist(claimId) {
        require(claims[claimId].isActive, "Claim already revoked");
        claims[claimId].isActive = false;
        emit ClaimRevoked(claimId);
    }
    
    /**
     * @dev Get all claims by an artist
     * @param artist Address of the artist
     * @return Array of claim IDs
     */
    function getArtistClaims(address artist) public view returns (uint256[] memory) {
        return artistClaims[artist];
    }
    
    /**
     * @dev Check if a location is available for claiming
     * @param x X coordinate
     * @param y Y coordinate
     * @param z Z coordinate
     * @return bool Whether the location is available
     */
    function isLocationAvailable(int256 x, int256 y, int256 z) 
        public 
        view 
        returns (bool) 
    {
        bytes32 locationHash = keccak256(abi.encodePacked(x, y, z));
        uint256 existingClaimId = locationToClaim[locationHash];
        return existingClaimId == 0 || !claims[existingClaimId].isActive;
    }
    
    /**
     * @dev Get claim details by ID
     * @param claimId The claim ID to query
     * @return All claim details
     */
    function getClaim(uint256 claimId) 
        public 
        view 
        returns (
            address artist,
            string memory artistId,
            int256 x,
            int256 y,
            int256 z,
            uint256 timestamp,
            string memory metadataURI,
            bool isActive
        ) 
    {
        GraffitiClaim memory claim = claims[claimId];
        return (
            claim.artist,
            claim.artistId,
            claim.x,
            claim.y,
            claim.z,
            claim.timestamp,
            claim.metadataURI,
            claim.isActive
        );
    }
    
    /**
     * @dev Update minimum distance between claims (owner only)
     * @param newMinDistance New minimum distance
     */
    function setMinDistance(uint256 newMinDistance) public onlyOwner {
        minDistance = newMinDistance;
    }
}
