using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Drawing Save Manager with Blockchain Integration
/// Handles saving drawings locally and claiming locations on-chain
/// </summary>
public class DrawingSaveManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Button to trigger save and blockchain claim")]
    public Button saveButton;
    
    [Tooltip("Text to display save status")]
    public Text statusText;
    
    [Tooltip("Panel to show during save process")]
    public GameObject savingPanel;
    
    [Header("References")]
    [Tooltip("Reference to DrawingManager")]
    public DrawingManager drawingManager;
    
    [Tooltip("Reference to BlockchainManager")]
    public BlockchainManager blockchainManager;
    
    [Tooltip("Center point for calculating graffiti location (usually main camera)")]
    public Transform centerPoint;
    
    [Header("Save Settings")]
    [Tooltip("Save drawings locally as JSON")]
    public bool saveLocalJSON = true;
    
    [Tooltip("Claim location on blockchain")]
    public bool claimOnBlockchain = true;
    
    [Tooltip("Require location availability check before claiming")]
    public bool checkAvailability = true;
    
    private bool isSaving = false;
    
    void Start()
    {
        // Find components if not assigned
        if (drawingManager == null)
            drawingManager = FindObjectOfType<DrawingManager>();
        
        if (blockchainManager == null)
            blockchainManager = FindObjectOfType<BlockchainManager>();
        
        if (centerPoint == null)
            centerPoint = Camera.main.transform;
        
        // Setup button
        if (saveButton != null)
        {
            saveButton.onClick.AddListener(OnSaveButtonClicked);
        }
        
        // Subscribe to blockchain events
        if (blockchainManager != null)
        {
            blockchainManager.OnClaimSuccess += OnBlockchainClaimSuccess;
            blockchainManager.OnClaimFailed += OnBlockchainClaimFailed;
        }
        
        // Hide saving panel
        if (savingPanel != null)
            savingPanel.SetActive(false);
    }
    
    void OnDestroy()
    {
        if (blockchainManager != null)
        {
            blockchainManager.OnClaimSuccess -= OnBlockchainClaimSuccess;
            blockchainManager.OnClaimFailed -= OnBlockchainClaimFailed;
        }
    }
    
    /// <summary>
    /// Called when save button is clicked
    /// </summary>
    public void OnSaveButtonClicked()
    {
        if (isSaving)
        {
            Debug.LogWarning("[SaveManager] Save already in progress");
            return;
        }
        
        SaveDrawing();
    }
    
    /// <summary>
    /// Save the current drawing and claim location on blockchain
    /// </summary>
    public void SaveDrawing()
    {
        if (drawingManager == null)
        {
            Debug.LogError("[SaveManager] DrawingManager not found!");
            UpdateStatus("Error: DrawingManager not found");
            return;
        }
        
        isSaving = true;
        
        if (savingPanel != null)
            savingPanel.SetActive(true);
        
        UpdateStatus("Preparing to save...");
        
        // Get drawing data
        DrawingData data = CollectDrawingData();
        
        if (data.strokes.Count == 0)
        {
            Debug.LogWarning("[SaveManager] No strokes to save");
            UpdateStatus("Nothing to save");
            isSaving = false;
            
            if (savingPanel != null)
                savingPanel.SetActive(false);
            
            return;
        }
        
        Debug.Log($"[SaveManager] Saving drawing with {data.strokes.Count} strokes");
        
        // Step 1: Save locally if enabled
        if (saveLocalJSON)
        {
            SaveToJSON(data);
        }
        
        // Step 2: Claim on blockchain if enabled
        if (claimOnBlockchain && blockchainManager != null)
        {
            // Calculate center position of drawing
            Vector3 graffitPosition = CalculateGraffitiCenter(data);
            
            if (checkAvailability)
            {
                UpdateStatus("Checking location availability...");
                blockchainManager.CheckLocationAvailability(graffitPosition, (isAvailable) =>
                {
                    if (isAvailable)
                    {
                        ClaimLocationOnChain(graffitPosition, data);
                    }
                    else
                    {
                        Debug.LogWarning("[SaveManager] Location already claimed!");
                        UpdateStatus("Location already claimed");
                        isSaving = false;
                        
                        if (savingPanel != null)
                            savingPanel.SetActive(false);
                    }
                });
            }
            else
            {
                ClaimLocationOnChain(graffitPosition, data);
            }
        }
        else
        {
            Debug.Log("[SaveManager] Save complete (blockchain disabled)");
            UpdateStatus("Saved locally");
            isSaving = false;
            
            if (savingPanel != null)
            {
                savingPanel.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// Collect all drawing data from DrawingManager
    /// </summary>
    private DrawingData CollectDrawingData()
    {
        DrawingData data = new DrawingData();
        
        // This is a simplified version - you'll need to adapt this to match
        // your DrawingManager's actual structure for accessing stroke data
        
        // For now, using reflection to access private fields (not ideal for production)
        var drawnLinesField = typeof(DrawingManager).GetField("drawnLines", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (drawnLinesField != null)
        {
            List<GameObject> drawnLines = drawnLinesField.GetValue(drawingManager) as List<GameObject>;
            
            if (drawnLines != null)
            {
                foreach (GameObject lineObj in drawnLines)
                {
                    LineRenderer lr = lineObj.GetComponent<LineRenderer>();
                    if (lr != null && lr.positionCount > 0)
                    {
                        StrokeData stroke = new StrokeData
                        {
                            points = new List<float[]>(),
                            color = ColorToHex(lr.startColor),
                            size = lr.startWidth,
                            brushType = lineObj.name
                        };
                        
                        // Collect all points
                        for (int i = 0; i < lr.positionCount; i++)
                        {
                            Vector3 point = lr.GetPosition(i);
                            stroke.points.Add(new float[] { point.x, point.y, point.z });
                        }
                        
                        data.strokes.Add(stroke);
                    }
                }
            }
        }
        
        data.totalStrokes = data.strokes.Count;
        
        return data;
    }
    
    /// <summary>
    /// Calculate the center position of all drawing strokes
    /// </summary>
    private Vector3 CalculateGraffitiCenter(DrawingData data)
    {
        if (data.strokes.Count == 0)
            return centerPoint != null ? centerPoint.position : Vector3.zero;
        
        Vector3 sum = Vector3.zero;
        int totalPoints = 0;
        
        foreach (var stroke in data.strokes)
        {
            foreach (var point in stroke.points)
            {
                sum += new Vector3(point[0], point[1], point[2]);
                totalPoints++;
            }
        }
        
        if (totalPoints > 0)
            return sum / totalPoints;
        
        return centerPoint != null ? centerPoint.position : Vector3.zero;
    }
    
    /// <summary>
    /// Save drawing data to local JSON file
    /// </summary>
    private void SaveToJSON(DrawingData data)
    {
        string json = JsonUtility.ToJson(data, true);
        string filename = $"drawing_{System.DateTime.UtcNow.Ticks}.json";
        string path = System.IO.Path.Combine(Application.persistentDataPath, "drawings", filename);
        
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
        System.IO.File.WriteAllText(path, json);
        
        Debug.Log($"[SaveManager] Drawing saved to: {path}");
    }
    
    /// <summary>
    /// Claim the graffiti location on blockchain
    /// </summary>
    private void ClaimLocationOnChain(Vector3 position, DrawingData data)
    {
        UpdateStatus("Claiming location on blockchain...");
        
        // Convert DrawingData to BlockchainManager.DrawingMetadata
        BlockchainManager.DrawingMetadata metadata = new BlockchainManager.DrawingMetadata
        {
            artistId = blockchainManager.artistId,
            x = position.x,
            y = position.y,
            z = position.z,
            timestamp = System.DateTime.UtcNow.ToString("o"),
            strokes = new List<BlockchainManager.StrokeData>(),
            totalStrokes = data.totalStrokes
        };
        
        // Convert strokes
        foreach (var stroke in data.strokes)
        {
            metadata.strokes.Add(new BlockchainManager.StrokeData
            {
                points = stroke.points,
                color = stroke.color,
                size = stroke.size,
                brushType = stroke.brushType
            });
        }
        
        // Claim on blockchain
        blockchainManager.ClaimLocation(position, metadata);
    }
    
    /// <summary>
    /// Called when blockchain claim succeeds
    /// </summary>
    private void OnBlockchainClaimSuccess(string txHash)
    {
        Debug.Log($"[SaveManager] ✅ Blockchain claim successful! TX: {txHash}");
        UpdateStatus($"Success! Location claimed on-chain\nTX: {txHash.Substring(0, 10)}...");
        
        isSaving = false;
        
        // Hide panel after delay
        if (savingPanel != null)
        {
            Invoke("HideSavingPanel", 3f);
        }
    }
    
    /// <summary>
    /// Called when blockchain claim fails
    /// </summary>
    private void OnBlockchainClaimFailed(string error)
    {
        Debug.LogError($"[SaveManager] ❌ Blockchain claim failed: {error}");
        UpdateStatus($"Failed: {error}");
        
        isSaving = false;
        
        if (savingPanel != null)
        {
            Invoke("HideSavingPanel", 3f);
        }
    }
    
    private void HideSavingPanel()
    {
        if (savingPanel != null)
            savingPanel.SetActive(false);
    }
    
    private void UpdateStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
        
        Debug.Log($"[SaveManager] Status: {message}");
    }
    
    private string ColorToHex(Color color)
    {
        return $"#{ColorUtility.ToHtmlStringRGB(color)}";
    }
    
    // Data structures
    [System.Serializable]
    public class DrawingData
    {
        public List<StrokeData> strokes = new List<StrokeData>();
        public int totalStrokes;
        public string timestamp;
        public Vector3 centerPosition;
    }
    
    [System.Serializable]
    public class StrokeData
    {
        public List<float[]> points;
        public string color;
        public float size;
        public string brushType;
    }
}
