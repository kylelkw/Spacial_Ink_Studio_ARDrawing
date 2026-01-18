using System.Collections.Generic;
using UnityEngine;

public class DrawingManager : MonoBehaviour
{
    [Header("System References")]
    public ColorPalette colorPalette;
    public List<BrushData> availableBrushes = new List<BrushData>();
    
    [Header("Drawing Settings")]
    public float minDistanceBeforeNewPoint = 0.01f;
    public int maxPointsPerLine = 500;
    public int maxTotalLines = 50;
    
    [Header("Size Settings")]
    public float brushSize = 0.01f;
    
    private int currentBrushIndex = 0;
    private int currentColorIndex = 0;
    
    private LineRenderer currentLine;
    private List<Vector3> currentLinePositions = new List<Vector3>();
    private bool isDrawing = false;
    
    private List<GameObject> allLines = new List<GameObject>();
    private List<int> allLineBrushIndices = new List<int>();
    
    private LinePool linePool;
    private IInputProvider inputProvider;
    
    private Transform trackingSpace;
    
    void Start()
    {
        if (colorPalette == null || availableBrushes.Count == 0)
        {
            Debug.LogError("[DrawingManager] Missing setup!");
            enabled = false;
            return;
        }
        
        FindTrackingSpace();
        InitializePool();
        ConnectInput();
        
        Debug.Log("[DrawingManager] Initialized successfully!");
    }
    
    void FindTrackingSpace()
    {
        // Try to find OVRCameraRig by name
        GameObject ovrRig = GameObject.Find("OVRCameraRig");
        
        if (ovrRig == null)
            ovrRig = GameObject.Find("OVR Camera Rig");
        if (ovrRig == null)
            ovrRig = GameObject.Find("CameraRig");
        
        // Search for any GameObject with TrackingSpace child
        if (ovrRig == null)
        {
            Transform[] allTransforms = FindObjectsOfType<Transform>();
            foreach (Transform t in allTransforms)
            {
                if (t.parent == null && t.Find("TrackingSpace") != null)
                {
                    ovrRig = t.gameObject;
                    break;
                }
            }
        }
        
        if (ovrRig != null)
        {
            trackingSpace = ovrRig.transform.Find("TrackingSpace");
            if (trackingSpace == null)
            {
                Debug.LogError($"TrackingSpace not found in {ovrRig.name}");
            }
        }
        else
        {
            Debug.LogError("No VR Camera Rig found. Add via GameObject > XR > OVR Camera Rig");
        }
    }
    
    void InitializePool()
    {
        linePool = GetComponent<LinePool>();
        if (linePool == null) linePool = gameObject.AddComponent<LinePool>();
        linePool.InitializeWithBrushes(availableBrushes);
    }
    
    void ConnectInput()
    {
        // Try to get any component that implements IInputProvider
        var providers = GetComponents<MonoBehaviour>();
        foreach (var provider in providers)
        {
            if (provider is IInputProvider)
            {
                inputProvider = provider as IInputProvider;
                break;
            }
        }
        
        if (inputProvider != null)
        {
            inputProvider.OnDrawPressed += StartDrawing;
            inputProvider.OnDrawReleased += StopDrawing;
            inputProvider.OnCycleBrush += CycleBrush;
            inputProvider.OnCycleColor += CycleColor;
            inputProvider.OnUndoPressed += UndoLastLine;
            inputProvider.OnErasePressed += ToggleEraseMode;
            
            if (inputProvider is VRControllerInputProvider vrProvider)
            {
                vrProvider.OnBrushSizeChanged += AdjustBrushSize;
            }
        }
        else
        {
            Debug.LogError("No input provider found. Add VRControllerInputProvider or KeyboardInputProvider component.");
        }
    }
    
    void OnDestroy()
    {
        if (inputProvider != null)
        {
            inputProvider.OnDrawPressed -= StartDrawing;
            inputProvider.OnDrawReleased -= StopDrawing;
            inputProvider.OnCycleBrush -= CycleBrush;
            inputProvider.OnCycleColor -= CycleColor;
            inputProvider.OnUndoPressed -= UndoLastLine;
            inputProvider.OnErasePressed -= ToggleEraseMode;
            
            if (inputProvider is VRControllerInputProvider vrProvider)
            {
                vrProvider.OnBrushSizeChanged -= AdjustBrushSize;
            }
        }
    }
    
    void Update()
    {
        if (isDrawing) DrawLine();
    }
    
    Vector3 GetControllerWorldPosition()
    {
        if (trackingSpace == null) return Vector3.zero;
        
        Vector3 localPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Vector3 worldPos = trackingSpace.TransformPoint(localPos);
        
        return worldPos;
    }
    
    void DrawLine()
    {
        if (currentLine == null) return;
        
        Vector3 pos = GetControllerWorldPosition();
        
        if (currentLinePositions.Count == 0 || 
            Vector3.Distance(pos, currentLinePositions[currentLinePositions.Count - 1]) > minDistanceBeforeNewPoint)
        {
            currentLinePositions.Add(pos);
            currentLine.positionCount = currentLinePositions.Count;
            currentLine.SetPosition(currentLinePositions.Count - 1, pos);
        }
    }
    
    public void StartDrawing()
    {
        if (allLines.Count >= maxTotalLines)
        {
            Debug.LogWarning($"Max lines reached ({maxTotalLines})");
            return;
        }
        
        isDrawing = true;
        currentLinePositions.Clear();
        
        GameObject newLine = linePool.GetLine(currentBrushIndex);
        if (newLine == null) return;
        
        allLines.Add(newLine);
        allLineBrushIndices.Add(currentBrushIndex);
        currentLine = newLine.GetComponent<LineRenderer>();
        
        Color drawColor = colorPalette.GetColor(currentColorIndex);
        
        currentLine.startColor = drawColor;
        currentLine.endColor = drawColor;
        currentLine.startWidth = brushSize;
        currentLine.endWidth = brushSize;
        
        if (currentLine.material != null)
        {
            currentLine.material.color = drawColor;
        }
    }
    
    public void StopDrawing()
    {
        isDrawing = false;
        
        if (currentLine == null || currentLinePositions.Count < 2)
        {
            if (currentLine != null && allLines.Count > 0)
            {
                int lastIndex = allLines.Count - 1;
                linePool.ReturnLine(allLines[lastIndex], allLineBrushIndices[lastIndex]);
                allLines.RemoveAt(lastIndex);
                allLineBrushIndices.RemoveAt(lastIndex);
            }
            currentLine = null;
            return;
        }
        
        BrushData brush = availableBrushes[currentBrushIndex];
        if (brush.appliesSmoothing && currentLinePositions.Count > 2)
        {
            Vector3[] smoothed = LineSmoother.SmoothLine(currentLinePositions.ToArray(), 2);
            currentLine.positionCount = smoothed.Length;
            currentLine.SetPositions(smoothed);
        }
        
        currentLine = null;
        Debug.Log($"[DrawingManager] Stopped drawing - Total lines: {allLines.Count}");
    }
    
    public void UndoLastLine()
    {
        if (allLines.Count == 0) return;
        int lastIndex = allLines.Count - 1;
        linePool.ReturnLine(allLines[lastIndex], allLineBrushIndices[lastIndex]);
        allLines.RemoveAt(lastIndex);
        allLineBrushIndices.RemoveAt(lastIndex);
    }
    
    public void CycleColor()
    {
        int oldIndex = currentColorIndex;
        currentColorIndex = (currentColorIndex + 1) % colorPalette.colors.Length;
        Debug.Log($"[DrawingManager] CycleColor: {oldIndex}({colorPalette.colors[oldIndex].colorName}) -> {currentColorIndex}({colorPalette.colors[currentColorIndex].colorName})");
    }
    
    public void CycleBrush()
    {
        currentBrushIndex = (currentBrushIndex + 1) % availableBrushes.Count;
    }
    
    public void ToggleEraseMode()
    {
        // Not implemented
    }
    
    public void AdjustBrushSize(float delta)
    {
        brushSize = Mathf.Clamp(brushSize + delta, 0.005f, 0.02f);
    }
}
