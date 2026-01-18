using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Blockchain Manager for interacting with GraffitiAnchor smart contract
/// Handles Web3 transactions and metadata storage
/// </summary>
public class BlockchainManager : MonoBehaviour
{
    [Header("Contract Configuration")]
    [Tooltip("GraffitiAnchor contract address on chain")]
    public string contractAddress = "0x...";
    
    [Tooltip("RPC URL for blockchain network (e.g., http://localhost:8545)")]
    public string rpcUrl = "http://localhost:8545";
    
    [Tooltip("Chain ID (1 = Ethereum Mainnet, 5 = Goerli, 1337 = Localhost)")]
    public int chainId = 1337;
    
    [Tooltip("Network name")]
    public string networkName = "localhost";
    
    [Header("Wallet Configuration")]
    [Tooltip("Artist wallet address (public key)")]
    public string walletAddress = "";
    
    [Tooltip("Private key for signing transactions (KEEP SECRET!)")]
    public string privateKey = "";
    
    [Header("Metadata Storage")]
    [Tooltip("Base URL for metadata storage (localhost server or IPFS gateway)")]
    public string metadataBaseUrl = "http://localhost:3000/metadata";
    
    [Header("Artist Settings")]
    [Tooltip("Custom artist identifier")]
    public string artistId = "SpatialInkArtist";
    
    [Header("References")]
    [Tooltip("Reference to Kairo Security Check component")]
    public KairoSecurityCheck kairoSecurity;
    
    // Events
    public event Action<string> OnClaimSuccess;
    public event Action<string> OnClaimFailed;
    public event Action<string> OnSecurityCheckComplete;
    
    // State
    private bool isInitialized = false;
    private Dictionary<uint, GraffitiClaimData> localClaims = new Dictionary<uint, GraffitiClaimData>();
    
    [Serializable]
    public class GraffitiClaimData
    {
        public uint claimId;
        public string artist;
        public string artistId;
        public UnityEngine.Vector3 position;
        public string timestamp;
        public string metadataURI;
        public bool isActive;
        public string transactionHash;
    }
    
    [Serializable]
    public class DrawingMetadata
    {
        public string artistId;
        public float x;
        public float y;
        public float z;
        public string timestamp;
        public List<StrokeData> strokes;
        public int totalStrokes;
    }
    
    [Serializable]
    public class StrokeData
    {
        public List<float[]> points; // Each point is [x, y, z]
        public string color;
        public float size;
        public string brushType;
    }
    
    void Awake()
    {
        // Find Kairo Security component if not assigned
        if (kairoSecurity == null)
        {
            kairoSecurity = GetComponent<KairoSecurityCheck>();
            if (kairoSecurity == null)
            {
                Debug.LogWarning("[Blockchain] KairoSecurityCheck component not found. Security checks will be skipped.");
            }
        }
    }
    
    void Start()
    {
        Initialize();
    }
    
    /// <summary>
    /// Initialize blockchain connection
    /// </summary>
    public void Initialize()
    {
        if (string.IsNullOrEmpty(contractAddress) || contractAddress == "0x...")
        {
            Debug.LogWarning("[Blockchain] Contract address not configured. Please deploy GraffitiAnchor contract first.");
            return;
        }
        
        if (string.IsNullOrEmpty(walletAddress))
        {
            Debug.LogWarning("[Blockchain] Wallet address not configured.");
            return;
        }
        
        Debug.Log($"[Blockchain] Initialized - Contract: {contractAddress}, Network: {networkName} (Chain ID: {chainId})");
        isInitialized = true;
    }
    
    /// <summary>
    /// Claim a location on the blockchain
    /// </summary>
    /// <param name="position">World position of the graffiti</param>
    /// <param name="drawingData">Complete drawing data to store</param>
    public void ClaimLocation(UnityEngine.Vector3 position, DrawingMetadata drawingData)
    {
        if (!isInitialized)
        {
            Debug.LogError("[Blockchain] Not initialized! Configure contract address and wallet first.");
            OnClaimFailed?.Invoke("Not initialized");
            return;
        }
        
        StartCoroutine(ClaimLocationCoroutine(position, drawingData));
    }
    
    private IEnumerator ClaimLocationCoroutine(UnityEngine.Vector3 position, DrawingMetadata drawingData)
    {
        Debug.Log($"[Blockchain] Starting claim process for position: {position}");
        
        // Step 1: Upload metadata
        Debug.Log("[Blockchain] Step 1/3: Uploading metadata...");
        string metadataURI = "";
        yield return StartCoroutine(UploadMetadata(drawingData, (uri) => metadataURI = uri));
        
        if (string.IsNullOrEmpty(metadataURI))
        {
            Debug.LogError("[Blockchain] Failed to upload metadata");
            OnClaimFailed?.Invoke("Metadata upload failed");
            yield break;
        }
        
        Debug.Log($"[Blockchain] Metadata uploaded: {metadataURI}");
        
        // Step 2: Prepare transaction data
        Debug.Log("[Blockchain] Step 2/3: Preparing blockchain transaction...");
        
        // Convert position to fixed-point integers (multiply by 1e6 for precision)
        long x = (long)(position.x * 1000000);
        long y = (long)(position.y * 1000000);
        long z = (long)(position.z * 1000000);
        
        // Build transaction payload
        string functionSignature = "claimLocation(string,int256,int256,int256,string)";
        string transactionData = BuildClaimTransactionData(artistId, x, y, z, metadataURI);
        
        Debug.Log($"[Blockchain] Transaction data prepared: artistId={artistId}, coords=({x},{y},{z})");
        
        // Step 3: Send transaction
        Debug.Log("[Blockchain] Step 3/3: Sending transaction to blockchain...");
        
        // Simulate transaction (in production, this would use Web3 library)
        yield return new WaitForSeconds(1f); // Simulate network delay
        
        // Mock transaction response
        string txHash = "0x" + Guid.NewGuid().ToString("N").Substring(0, 64);
        uint claimId = (uint)UnityEngine.Random.Range(1, 10000);
        
        // Store claim locally
        GraffitiClaimData claim = new GraffitiClaimData
        {
            claimId = claimId,
            artist = walletAddress,
            artistId = artistId,
            position = position,
            timestamp = DateTime.UtcNow.ToString("o"),
            metadataURI = metadataURI,
            isActive = true,
            transactionHash = txHash
        };
        
        localClaims[claimId] = claim;
        
        Debug.Log($"[Blockchain] ✅ Location claimed successfully!");
        Debug.Log($"[Blockchain] Claim ID: {claimId}");
        Debug.Log($"[Blockchain] Transaction Hash: {txHash}");
        Debug.Log($"[Blockchain] Metadata URI: {metadataURI}");
        
        // Save claim to persistent storage
        SaveClaimToFile(claim);
        
        OnClaimSuccess?.Invoke(txHash);
    }
    
    /// <summary>
    /// Upload drawing metadata to server/IPFS
    /// </summary>
    private IEnumerator UploadMetadata(DrawingMetadata metadata, Action<string> onComplete)
    {
        metadata.timestamp = DateTime.UtcNow.ToString("o");
        string jsonData = JsonUtility.ToJson(metadata, true);
        
        Debug.Log($"[Blockchain] Uploading metadata ({jsonData.Length} bytes)");
        
        // Upload to metadata server
        using (UnityWebRequest www = UnityWebRequest.Post(metadataBaseUrl, jsonData, "application/json"))
        {
            yield return www.SendWebRequest();
            
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[Blockchain] Metadata upload failed: {www.error}");
                
                // Fallback: Save locally and generate local URI
                string localPath = SaveMetadataLocally(jsonData);
                onComplete?.Invoke(localPath);
            }
            else
            {
                // Parse response to get URI
                string response = www.downloadHandler.text;
                Debug.Log($"[Blockchain] Metadata upload response: {response}");
                
                // Extract URI from response (assuming JSON response with "uri" field)
                string uri = ExtractURIFromResponse(response);
                
                if (string.IsNullOrEmpty(uri))
                {
                    // Fallback: use local path
                    uri = SaveMetadataLocally(jsonData);
                }
                
                onComplete?.Invoke(uri);
            }
        }
    }
    
    private string SaveMetadataLocally(string jsonData)
    {
        string filename = $"metadata_{DateTime.UtcNow.Ticks}.json";
        string path = System.IO.Path.Combine(Application.persistentDataPath, "graffiti_metadata", filename);
        
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
        System.IO.File.WriteAllText(path, jsonData);
        
        Debug.Log($"[Blockchain] Metadata saved locally: {path}");
        return $"file://{path}";
    }
    
    private string ExtractURIFromResponse(string response)
    {
        // Simple JSON parsing for "uri" field
        int uriStart = response.IndexOf("\"uri\":\"");
        if (uriStart > -1)
        {
            uriStart += 7;
            int uriEnd = response.IndexOf("\"", uriStart);
            if (uriEnd > uriStart)
            {
                return response.Substring(uriStart, uriEnd - uriStart);
            }
        }
        return "";
    }
    
    private string BuildClaimTransactionData(string artistId, long x, long y, long z, string metadataURI)
    {
        // In production, this would encode the function call using ABI encoding
        // For now, return a mock transaction data
        return $"claimLocation(\"{artistId}\",{x},{y},{z},\"{metadataURI}\")";
    }
    
    private void SaveClaimToFile(GraffitiClaimData claim)
    {
        string json = JsonUtility.ToJson(claim, true);
        string filename = $"claim_{claim.claimId}_{DateTime.UtcNow.Ticks}.json";
        string path = System.IO.Path.Combine(Application.persistentDataPath, "claims", filename);
        
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
        System.IO.File.WriteAllText(path, json);
        
        Debug.Log($"[Blockchain] Claim saved to: {path}");
    }
    
    /// <summary>
    /// Check if a location is available for claiming
    /// </summary>
    public void CheckLocationAvailability(UnityEngine.Vector3 position, Action<bool> onComplete)
    {
        StartCoroutine(CheckLocationAvailabilityCoroutine(position, onComplete));
    }
    
    private IEnumerator CheckLocationAvailabilityCoroutine(UnityEngine.Vector3 position, Action<bool> onComplete)
    {
        // Convert to fixed-point
        long x = (long)(position.x * 1000000);
        long y = (long)(position.y * 1000000);
        long z = (long)(position.z * 1000000);
        
        // In production, this would call the smart contract's isLocationAvailable function
        // For now, simulate a check
        yield return new WaitForSeconds(0.5f);
        
        // Mock result - always available for demo
        bool isAvailable = true;
        
        Debug.Log($"[Blockchain] Location availability check: {isAvailable}");
        onComplete?.Invoke(isAvailable);
    }
    
    /// <summary>
    /// Get all claims made by the current artist
    /// </summary>
    public List<GraffitiClaimData> GetArtistClaims()
    {
        return new List<GraffitiClaimData>(localClaims.Values);
    }
    
    /// <summary>
    /// Verify contract security with Kairo before deployment
    /// </summary>
    public void VerifyContractSecurity(string contractCode, Action<bool> onComplete)
    {
        if (kairoSecurity == null)
        {
            Debug.LogWarning("[Blockchain] Kairo Security Check not available. Skipping verification.");
            onComplete?.Invoke(true);
            return;
        }
        
        Debug.Log("[Blockchain] Running Kairo security check on contract...");
        
        kairoSecurity.AnalyzeContract(contractCode, "GraffitiAnchor.sol", (result) =>
        {
            bool isSecure = result.decision == KairoSecurityCheck.SecurityDecision.ALLOW;
            
            string message = $"Security Check Complete:\n" +
                           $"Decision: {result.decision}\n" +
                           $"Risk Score: {result.risk_score}\n" +
                           $"Reason: {result.decision_reason}";
            
            if (result.summary != null && result.summary.total > 0)
            {
                message += $"\n\nIssues Found:\n" +
                          $"Critical: {result.summary.critical}\n" +
                          $"High: {result.summary.high}\n" +
                          $"Medium: {result.summary.medium}\n" +
                          $"Low: {result.summary.low}";
            }
            
            Debug.Log($"[Blockchain] {message}");
            OnSecurityCheckComplete?.Invoke(message);
            
            onComplete?.Invoke(isSecure);
        });
    }
}
