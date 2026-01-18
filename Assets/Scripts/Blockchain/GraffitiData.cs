using System;
using UnityEngine;

namespace SpatialInk.Blockchain
{
    /// <summary>
    /// Data structure for graffiti stored on-chain
    /// </summary>
    [Serializable]
    public class GraffitiData
    {
        public Vector3 position;
        public string artistAddress;
        public string artworkHash;
        public long timestamp;
        public bool isActive;
        public string locationHash;
        
        // Drawing data for IPFS storage
        public DrawingMetadata drawingMetadata;
    }
    
    /// <summary>
    /// Metadata for the drawing stored on IPFS
    /// </summary>
    [Serializable]
    public class DrawingMetadata
    {
        public string title;
        public string description;
        public Vector3[] strokePoints;
        public Color[] strokeColors;
        public float[] strokeWidths;
        public string artistName;
        public long createdAt;
        public Vector3 anchorPosition;
        public Quaternion anchorRotation;
        
        // Optional: Store as compressed base64 or reference to full 3D model
        public string modelDataUri;
    }
    
    /// <summary>
    /// Response from blockchain transaction
    /// </summary>
    [Serializable]
    public class TransactionResponse
    {
        public bool success;
        public string transactionHash;
        public string locationHash;
        public string errorMessage;
    }
}
