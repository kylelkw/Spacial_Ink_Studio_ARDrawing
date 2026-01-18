// SPDX-License-Identifier: MIT
pragma solidity ^0.8.20;

import "@openzeppelin/contracts/access/Ownable.sol";
import "@openzeppelin/contracts/security/ReentrancyGuard.sol";

/**
 * @title GraffitiAnchor
 * @dev Store AR graffiti locations on-chain permanently
 * @notice This contract allows artists to claim physical locations for their digital art
 */
contract GraffitiAnchor is Ownable, ReentrancyGuard {
    
    struct Graffiti {
        int256 x;           // X coordinate (in millimeters or custom unit)
        int256 y;           // Y coordinate
        int256 z;           // Z coordinate
        address artist;     // Artist's wallet address
        string artworkHash; // IPFS hash or metadata URI
        uint256 timestamp;  // Creation timestamp
        bool isActive;      // Active status
    }
    
    // Mapping from location hash to Graffiti
    mapping(bytes32 => Graffiti) public graffitis;
    
    // Mapping from artist address to their graffiti IDs
    mapping(address => bytes32[]) public artistGraffitis;
    
    // Array of all graffiti location hashes
    bytes32[] public allGraffitiLocations;
    
    // Events
    event GraffitiClaimed(
        bytes32 indexed locationHash,
        address indexed artist,
        int256 x,
        int256 y,
        int256 z,
        string artworkHash,
        uint256 timestamp
    );
    
    event GraffitiUpdated(
        bytes32 indexed locationHash,
        string newArtworkHash,
        uint256 timestamp
    );
    
    event GraffitiDeactivated(
        bytes32 indexed locationHash,
        uint256 timestamp
    );
    
    // Precision for location matching (e.g., 1000 = 1 meter tolerance)
    uint256 public locationPrecision = 1000;
    
    /**
     * @dev Constructor
     */
    constructor() Ownable(msg.sender) {}
    
    /**
     * @dev Calculate location hash with precision
     * @param x X coordinate
     * @param y Y coordinate
     * @param z Z coordinate
     * @return Location hash
     */
    function calculateLocationHash(
        int256 x,
        int256 y,
        int256 z
    ) public view returns (bytes32) {
        // Round coordinates to precision
        int256 roundedX = (x / int256(locationPrecision)) * int256(locationPrecision);
        int256 roundedY = (y / int256(locationPrecision)) * int256(locationPrecision);
        int256 roundedZ = (z / int256(locationPrecision)) * int256(locationPrecision);
        
        return keccak256(abi.encodePacked(roundedX, roundedY, roundedZ));
    }
    
    /**
     * @dev Claim a location for graffiti
     * @param x X coordinate
     * @param y Y coordinate
     * @param z Z coordinate
     * @param artworkHash IPFS hash or metadata URI
     */
    function claimLocation(
        int256 x,
        int256 y,
        int256 z,
        string calldata artworkHash
    ) external nonReentrant returns (bytes32) {
        require(bytes(artworkHash).length > 0, "Artwork hash cannot be empty");
        
        bytes32 locationHash = calculateLocationHash(x, y, z);
        
        // Check if location is already claimed
        require(!graffitis[locationHash].isActive, "Location already claimed");
        
        // Create new graffiti
        graffitis[locationHash] = Graffiti({
            x: x,
            y: y,
            z: z,
            artist: msg.sender,
            artworkHash: artworkHash,
            timestamp: block.timestamp,
            isActive: true
        });
        
        // Track artist's graffitis
        artistGraffitis[msg.sender].push(locationHash);
        
        // Track all locations
        allGraffitiLocations.push(locationHash);
        
        emit GraffitiClaimed(
            locationHash,
            msg.sender,
            x,
            y,
            z,
            artworkHash,
            block.timestamp
        );
        
        return locationHash;
    }
    
    /**
     * @dev Update artwork at a claimed location (only by original artist)
     * @param locationHash Location hash
     * @param newArtworkHash New IPFS hash or metadata URI
     */
    function updateGraffiti(
        bytes32 locationHash,
        string calldata newArtworkHash
    ) external {
        Graffiti storage graffiti = graffitis[locationHash];
        require(graffiti.isActive, "Location not claimed");
        require(graffiti.artist == msg.sender, "Only artist can update");
        require(bytes(newArtworkHash).length > 0, "Artwork hash cannot be empty");
        
        graffiti.artworkHash = newArtworkHash;
        graffiti.timestamp = block.timestamp;
        
        emit GraffitiUpdated(locationHash, newArtworkHash, block.timestamp);
    }
    
    /**
     * @dev Deactivate graffiti (only by artist or owner)
     * @param locationHash Location hash
     */
    function deactivateGraffiti(bytes32 locationHash) external {
        Graffiti storage graffiti = graffitis[locationHash];
        require(graffiti.isActive, "Location not claimed");
        require(
            graffiti.artist == msg.sender || owner() == msg.sender,
            "Not authorized"
        );
        
        graffiti.isActive = false;
        
        emit GraffitiDeactivated(locationHash, block.timestamp);
    }
    
    /**
     * @dev Get graffiti details by location hash
     * @param locationHash Location hash
     */
    function getGraffiti(bytes32 locationHash)
        external
        view
        returns (
            int256 x,
            int256 y,
            int256 z,
            address artist,
            string memory artworkHash,
            uint256 timestamp,
            bool isActive
        )
    {
        Graffiti memory graffiti = graffitis[locationHash];
        return (
            graffiti.x,
            graffiti.y,
            graffiti.z,
            graffiti.artist,
            graffiti.artworkHash,
            graffiti.timestamp,
            graffiti.isActive
        );
    }
    
    /**
     * @dev Get all graffitis by an artist
     * @param artist Artist address
     */
    function getArtistGraffitis(address artist)
        external
        view
        returns (bytes32[] memory)
    {
        return artistGraffitis[artist];
    }
    
    /**
     * @dev Get total number of graffitis
     */
    function getTotalGraffitis() external view returns (uint256) {
        return allGraffitiLocations.length;
    }
    
    /**
     * @dev Get graffitis in a range (for pagination)
     * @param start Start index
     * @param count Number of items
     */
    function getGraffitisInRange(uint256 start, uint256 count)
        external
        view
        returns (bytes32[] memory)
    {
        require(start < allGraffitiLocations.length, "Start index out of bounds");
        
        uint256 end = start + count;
        if (end > allGraffitiLocations.length) {
            end = allGraffitiLocations.length;
        }
        
        bytes32[] memory result = new bytes32[](end - start);
        for (uint256 i = start; i < end; i++) {
            result[i - start] = allGraffitiLocations[i];
        }
        
        return result;
    }
    
    /**
     * @dev Update location precision (only owner)
     * @param newPrecision New precision value
     */
    function setLocationPrecision(uint256 newPrecision) external onlyOwner {
        require(newPrecision > 0, "Precision must be positive");
        locationPrecision = newPrecision;
    }
    
    /**
     * @dev Check if a location is available
     * @param x X coordinate
     * @param y Y coordinate
     * @param z Z coordinate
     */
    function isLocationAvailable(
        int256 x,
        int256 y,
        int256 z
    ) external view returns (bool) {
        bytes32 locationHash = calculateLocationHash(x, y, z);
        return !graffitis[locationHash].isActive;
    }
}
