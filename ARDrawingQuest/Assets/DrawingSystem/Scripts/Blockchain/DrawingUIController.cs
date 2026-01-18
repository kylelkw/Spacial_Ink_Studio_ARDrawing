using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Example UI controller for blockchain-enabled AR drawing
/// Shows how to integrate save button with blockchain claiming
/// </summary>
public class DrawingUIController : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Button to trigger save and blockchain claim")]
    public Button saveButton;
    
    [Tooltip("Button to clear all drawings")]
    public Button clearButton;
    
    [Tooltip("Text displaying transaction status")]
    public Text statusText;
    
    [Tooltip("Panel showing during save process")]
    public GameObject savingPanel;
    
    [Tooltip("Text showing transaction hash")]
    public Text transactionHashText;
    
    [Header("Component References")]
    [Tooltip("Drawing save manager with blockchain integration")]
    public DrawingSaveManager saveManager;
    
    [Tooltip("Optional: Blockchain manager for additional features")]
    public BlockchainManager blockchainManager;
    
    [Header("UI Settings")]
    [Tooltip("Auto-hide status message after delay")]
    public bool autoHideStatus = true;
    
    [Tooltip("Seconds before hiding status")]
    public float statusHideDelay = 5f;
    
    void Start()
    {
        SetupUI();
        SetupEventListeners();
    }
    
    void OnDestroy()
    {
        CleanupEventListeners();
    }
    
    /// <summary>
    /// Initialize UI elements
    /// </summary>
    void SetupUI()
    {
        // Hide saving panel initially
        if (savingPanel != null)
            savingPanel.SetActive(false);
        
        // Clear status text
        if (statusText != null)
            statusText.text = "";
        
        // Clear transaction hash
        if (transactionHashText != null)
            transactionHashText.text = "";
        
        // Setup button listeners
        if (saveButton != null)
            saveButton.onClick.AddListener(OnSaveButtonClicked);
        
        if (clearButton != null)
            clearButton.onClick.AddListener(OnClearButtonClicked);
    }
    
    /// <summary>
    /// Subscribe to blockchain events
    /// </summary>
    void SetupEventListeners()
    {
        if (blockchainManager != null)
        {
            blockchainManager.OnClaimSuccess += OnBlockchainSuccess;
            blockchainManager.OnClaimFailed += OnBlockchainFailed;
            blockchainManager.OnSecurityCheckComplete += OnSecurityCheckComplete;
        }
    }
    
    /// <summary>
    /// Unsubscribe from events
    /// </summary>
    void CleanupEventListeners()
    {
        if (blockchainManager != null)
        {
            blockchainManager.OnClaimSuccess -= OnBlockchainSuccess;
            blockchainManager.OnClaimFailed -= OnBlockchainFailed;
            blockchainManager.OnSecurityCheckComplete -= OnSecurityCheckComplete;
        }
    }
    
    /// <summary>
    /// Called when save button is clicked
    /// </summary>
    void OnSaveButtonClicked()
    {
        if (saveManager == null)
        {
            Debug.LogError("[UI] SaveManager not assigned!");
            UpdateStatus("Error: SaveManager not found", true);
            return;
        }
        
        Debug.Log("[UI] Save button clicked");
        
        // Show saving panel
        if (savingPanel != null)
            savingPanel.SetActive(true);
        
        // Update status
        UpdateStatus("Preparing to save...", false);
        
        // Trigger save
        saveManager.SaveDrawing();
    }
    
    /// <summary>
    /// Called when clear button is clicked
    /// </summary>
    void OnClearButtonClicked()
    {
        Debug.Log("[UI] Clear button clicked");
        
        // TODO: Implement clear functionality
        // This would call DrawingManager to clear all strokes
        UpdateStatus("Clear functionality - implement in DrawingManager", true);
    }
    
    /// <summary>
    /// Called when blockchain claim succeeds
    /// </summary>
    void OnBlockchainSuccess(string txHash)
    {
        Debug.Log($"[UI] Blockchain claim successful: {txHash}");
        
        string shortHash = txHash.Length > 10 ? txHash.Substring(0, 10) + "..." : txHash;
        
        UpdateStatus($"✅ Success!\nLocation claimed on-chain", true);
        
        if (transactionHashText != null)
            transactionHashText.text = $"TX: {shortHash}";
        
        // Hide saving panel after delay
        if (savingPanel != null)
            Invoke("HideSavingPanel", 3f);
    }
    
    /// <summary>
    /// Called when blockchain claim fails
    /// </summary>
    void OnBlockchainFailed(string error)
    {
        Debug.LogError($"[UI] Blockchain claim failed: {error}");
        
        UpdateStatus($"❌ Failed: {error}", true);
        
        // Hide saving panel after delay
        if (savingPanel != null)
            Invoke("HideSavingPanel", 5f);
    }
    
    /// <summary>
    /// Called when security check completes
    /// </summary>
    void OnSecurityCheckComplete(string message)
    {
        Debug.Log($"[UI] Security check: {message}");
        
        // Could display security info in UI if desired
        // For now, just log it
    }
    
    /// <summary>
    /// Update status text and optionally auto-hide
    /// </summary>
    void UpdateStatus(string message, bool canHide)
    {
        if (statusText != null)
        {
            statusText.text = message;
            
            // Auto-hide after delay if enabled
            if (canHide && autoHideStatus)
            {
                CancelInvoke("ClearStatus");
                Invoke("ClearStatus", statusHideDelay);
            }
        }
        
        Debug.Log($"[UI] Status: {message}");
    }
    
    /// <summary>
    /// Clear status text
    /// </summary>
    void ClearStatus()
    {
        if (statusText != null)
            statusText.text = "";
    }
    
    /// <summary>
    /// Hide the saving panel
    /// </summary>
    void HideSavingPanel()
    {
        if (savingPanel != null)
            savingPanel.SetActive(false);
    }
    
    // ========================================
    // Additional Helper Methods
    // ========================================
    
    /// <summary>
    /// Show all claims made by current artist
    /// </summary>
    public void ShowMyClaims()
    {
        if (blockchainManager == null)
        {
            UpdateStatus("Blockchain manager not found", true);
            return;
        }
        
        var claims = blockchainManager.GetArtistClaims();
        
        Debug.Log($"[UI] Artist has {claims.Count} claims");
        
        foreach (var claim in claims)
        {
            Debug.Log($"[UI] Claim {claim.claimId}: {claim.position} - {claim.metadataURI}");
        }
        
        UpdateStatus($"You have {claims.Count} claim(s)", true);
    }
    
    /// <summary>
    /// Check if a location is available before saving
    /// </summary>
    public void CheckCurrentLocation()
    {
        if (blockchainManager == null)
        {
            UpdateStatus("Blockchain manager not found", true);
            return;
        }
        
        // Get center position from camera
        Vector3 position = Camera.main.transform.position;
        
        UpdateStatus("Checking location availability...", false);
        
        blockchainManager.CheckLocationAvailability(position, (isAvailable) =>
        {
            if (isAvailable)
            {
                UpdateStatus("✅ Location available!", true);
            }
            else
            {
                UpdateStatus("❌ Location already claimed", true);
            }
        });
    }
    
    /// <summary>
    /// Manually trigger security check
    /// </summary>
    public void RunSecurityCheck()
    {
        if (blockchainManager == null)
        {
            UpdateStatus("Blockchain manager not found", true);
            return;
        }
        
        // Read contract source
        string contractPath = System.IO.Path.Combine(
            Application.dataPath,
            "..",
            "..",
            "Contracts",
            "GraffitiAnchor.sol"
        );
        
        if (!System.IO.File.Exists(contractPath))
        {
            UpdateStatus("Contract file not found", true);
            return;
        }
        
        string contractCode = System.IO.File.ReadAllText(contractPath);
        
        UpdateStatus("Running Kairo security check...", false);
        
        blockchainManager.VerifyContractSecurity(contractCode, (isSecure) =>
        {
            if (isSecure)
            {
                UpdateStatus("✅ Contract is secure!", true);
            }
            else
            {
                UpdateStatus("⚠️ Security issues found", true);
            }
        });
    }
}
