using UnityEngine;

namespace SpatialInk.Blockchain
{
    /// <summary>
    /// Configuration for blockchain connection
    /// </summary>
    [CreateAssetMenu(fileName = "BlockchainConfig", menuName = "Spatial Ink/Blockchain Config")]
    public class BlockchainConfig : ScriptableObject
    {
        [Header("Network Configuration")]
        [Tooltip("Kairo RPC URL")]
        public string rpcUrl = "https://kairo-rpc-url.com"; // Replace with actual Kairo RPC
        
        [Tooltip("Chain ID for Kairo network")]
        public int chainId = 1; // Replace with actual Kairo chain ID
        
        [Header("Contract Configuration")]
        [Tooltip("Deployed GraffitiAnchor contract address")]
        public string contractAddress = "";
        
        [Header("IPFS Configuration")]
        [Tooltip("IPFS API endpoint for storing artwork data")]
        public string ipfsApiUrl = "https://ipfs.infura.io:5001";
        
        [Tooltip("IPFS Gateway for retrieving content")]
        public string ipfsGateway = "https://ipfs.io/ipfs/";
        
        [Header("Transaction Settings")]
        [Tooltip("Gas limit for transactions")]
        public int gasLimit = 300000;
        
        [Tooltip("Location precision (in millimeters)")]
        public int locationPrecision = 1000; // 1 meter tolerance
        
        [Header("Testing")]
        [Tooltip("Enable test mode (uses mock blockchain)")]
        public bool testMode = true;
    }
}
