using UnityEngine;
using System.Text;

/// <summary>
/// Diagnostic tool to check why drawing isn't working.
/// Attach to GameManager and press Play to see detailed diagnostics.
/// </summary>
public class DrawingDiagnostics : MonoBehaviour
{
    [Header("Run Diagnostics")]
    [Tooltip("Check this in Inspector to run diagnostics continuously")]
    public bool runContinuousDiagnostics = false;
    
    private DrawingManager drawingManager;
    private IInputProvider inputProvider;
    private Transform trackingSpace;
    private bool hasLoggedOnce = false;
    
    void Start()
    {
        Debug.Log("=== DRAWING DIAGNOSTICS START ===");
        RunFullDiagnostics();
        Debug.Log("=== DRAWING DIAGNOSTICS END ===");
    }
    
    void Update()
    {
        if (runContinuousDiagnostics)
        {
            CheckControllerInput();
        }
        
        // Press D key to run diagnostics again
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("=== MANUAL DIAGNOSTICS TRIGGERED ===");
            RunFullDiagnostics();
        }
    }
    
    void RunFullDiagnostics()
    {
        StringBuilder report = new StringBuilder();
        report.AppendLine("\n╔══════════════════════════════════════════════════╗");
        report.AppendLine("║    SPATIAL INK STUDIO - DIAGNOSTIC REPORT       ║");
        report.AppendLine("╚══════════════════════════════════════════════════╝\n");
        
        // Check 1: DrawingManager
        drawingManager = GetComponent<DrawingManager>();
        if (drawingManager == null)
        {
            report.AppendLine("❌ CRITICAL: DrawingManager component NOT FOUND!");
            report.AppendLine("   → Add DrawingManager component to this GameObject");
        }
        else
        {
            report.AppendLine("✓ DrawingManager component found");
            
            // Check ColorPalette
            if (drawingManager.colorPalette == null)
            {
                report.AppendLine("❌ CRITICAL: ColorPalette is NULL!");
                report.AppendLine("   → Assign DefaultPalette.asset in Inspector");
            }
            else
            {
                report.AppendLine($"✓ ColorPalette assigned: {drawingManager.colorPalette.name}");
                report.AppendLine($"  Colors available: {drawingManager.colorPalette.colors.Length}");
            }
            
            // Check Brushes
            if (drawingManager.availableBrushes == null || drawingManager.availableBrushes.Count == 0)
            {
                report.AppendLine("❌ CRITICAL: No brushes assigned!");
                report.AppendLine("   → Assign WireBrush and FlatBrush in Inspector");
            }
            else
            {
                report.AppendLine($"✓ Brushes assigned: {drawingManager.availableBrushes.Count}");
                for (int i = 0; i < drawingManager.availableBrushes.Count; i++)
                {
                    var brush = drawingManager.availableBrushes[i];
                    if (brush == null)
                    {
                        report.AppendLine($"  ❌ Brush [{i}] is NULL!");
                    }
                    else
                    {
                        report.AppendLine($"  ✓ [{i}] {brush.brushName}");
                        if (brush.linePrefab == null)
                        {
                            report.AppendLine($"      ❌ Line prefab is NULL!");
                        }
                    }
                }
            }
            
            report.AppendLine($"  Brush Size: {drawingManager.brushSize}m");
            report.AppendLine($"  Min Distance: {drawingManager.minDistanceBeforeNewPoint}m");
            report.AppendLine($"  Max Lines: {drawingManager.maxTotalLines}");
            report.AppendLine($"  Max Points/Line: {drawingManager.maxPointsPerLine}");
        }
        
        // Check 2: Input Provider
        inputProvider = GetComponent<IInputProvider>();
        if (inputProvider == null)
        {
            report.AppendLine("\n❌ CRITICAL: No IInputProvider component found!");
            report.AppendLine("   → Add VRControllerInputProvider or KeyboardInputProvider");
        }
        else
        {
            report.AppendLine($"\n✓ Input Provider found: {inputProvider.GetType().Name}");
        }
        
        // Check 3: OVRCameraRig
        GameObject ovrRig = GameObject.Find("OVRCameraRig");
        
        // Try alternate names
        if (ovrRig == null)
        {
            ovrRig = GameObject.Find("OVR Camera Rig");
        }
        if (ovrRig == null)
        {
            ovrRig = GameObject.Find("CameraRig");
        }
        
        // Search for TrackingSpace
        if (ovrRig == null)
        {
            Transform[] allTransforms = FindObjectsOfType<Transform>();
            foreach (Transform t in allTransforms)
            {
                if (t.parent == null && t.Find("TrackingSpace") != null)
                {
                    ovrRig = t.gameObject;
                    report.AppendLine($"\n✓ Found camera rig by TrackingSpace: {ovrRig.name}");
                    break;
                }
            }
        }
        
        if (ovrRig == null)
        {
            report.AppendLine("\n❌ CRITICAL: No VR Camera Rig found in scene!");
            report.AppendLine("   → Add OVRCameraRig: GameObject > XR > OVR Camera Rig");
            report.AppendLine("\n   Root GameObjects currently in scene:");
            foreach (GameObject go in FindObjectsOfType<GameObject>())
            {
                if (go.transform.parent == null)
                {
                    report.AppendLine($"     - {go.name}");
                }
            }
        }
        else
        {
            report.AppendLine("\n✓ OVRCameraRig found");
            
            // Check TrackingSpace
            trackingSpace = ovrRig.transform.Find("TrackingSpace");
            if (trackingSpace == null)
            {
                report.AppendLine("❌ CRITICAL: TrackingSpace NOT FOUND!");
                report.AppendLine("   → OVRCameraRig structure is incorrect");
            }
            else
            {
                report.AppendLine("✓ TrackingSpace found");
                
                // Check hand anchors
                Transform rightHand = trackingSpace.Find("RightHandAnchor");
                Transform leftHand = trackingSpace.Find("LeftHandAnchor");
                Transform centerEye = trackingSpace.Find("CenterEyeAnchor");
                
                report.AppendLine($"  Right Hand: {(rightHand != null ? "✓" : "❌")}");
                report.AppendLine($"  Left Hand: {(leftHand != null ? "✓" : "❌")}");
                report.AppendLine($"  Center Eye: {(centerEye != null ? "✓" : "❌")}");
                
                // Check for passthrough
                if (centerEye != null)
                {
                    var passthrough = centerEye.GetComponent("OVRPassthroughLayer");
                    if (passthrough == null)
                    {
                        report.AppendLine("⚠ WARNING: OVRPassthroughLayer not found on CenterEyeAnchor");
                        report.AppendLine("   → Passthrough AR won't work");
                    }
                    else
                    {
                        report.AppendLine("✓ OVRPassthroughLayer found");
                    }
                    
                    var camera = centerEye.GetComponent<Camera>();
                    if (camera != null)
                    {
                        Color bg = camera.backgroundColor;
                        if (bg.a > 0.01f)
                        {
                            report.AppendLine($"⚠ WARNING: Camera background alpha is {bg.a} (should be 0)");
                        }
                        else
                        {
                            report.AppendLine("✓ Camera background alpha = 0 (correct for passthrough)");
                        }
                    }
                }
            }
            
            // Check OVRManager
            var ovrManager = ovrRig.GetComponent("OVRManager");
            if (ovrManager == null)
            {
                report.AppendLine("⚠ WARNING: OVRManager component not found");
            }
            else
            {
                report.AppendLine("✓ OVRManager found");
            }
        }
        
        // Check 4: LinePool
        var linePool = GetComponent<LinePool>();
        if (linePool == null)
        {
            report.AppendLine("\n⚠ LinePool will be auto-created by DrawingManager");
        }
        else
        {
            report.AppendLine("\n✓ LinePool component found");
        }
        
        // Check 5: Scene objects
        report.AppendLine("\n--- Scene Setup ---");
        report.AppendLine($"Current Scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        report.AppendLine($"Total GameObjects: {FindObjectsOfType<GameObject>().Length}");
        
        Debug.Log(report.ToString());
        
        // Summary
        bool allGood = drawingManager != null && 
                       inputProvider != null && 
                       ovrRig != null && 
                       trackingSpace != null &&
                       drawingManager.colorPalette != null &&
                       drawingManager.availableBrushes.Count > 0;
        
        if (allGood)
        {
            Debug.Log("✓✓✓ ALL CRITICAL SYSTEMS READY - Drawing should work! ✓✓✓");
        }
        else
        {
            Debug.LogError("❌❌❌ SETUP INCOMPLETE - Fix errors above! ❌❌❌");
        }
    }
    
    void CheckControllerInput()
    {
        if (!hasLoggedOnce)
        {
            Debug.Log("=== CONTROLLER INPUT MONITORING (Press D to stop) ===");
            hasLoggedOnce = true;
        }
        
        // Check if OVRInput is available
        if (OVRInput.GetActiveController() != OVRInput.Controller.None)
        {
            // Right trigger
            float trigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
            if (trigger > 0.1f)
            {
                Debug.Log($"[Controller] RIGHT Trigger value: {trigger:F2}");
            }
            
            // Check controller position
            if (trackingSpace != null)
            {
                Vector3 localPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
                Vector3 worldPos = trackingSpace.TransformPoint(localPos);
                
                if (trigger > 0.5f)
                {
                    Debug.Log($"[Controller] Local: {localPos}, World: {worldPos}");
                }
            }
            
            // Check buttons
            if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
            {
                Debug.Log("[Controller] RIGHT A button pressed");
            }
            if (OVRInput.GetDown(OVRInput.Button.Three, OVRInput.Controller.LTouch))
            {
                Debug.Log("[Controller] LEFT Y button pressed");
            }
            if (OVRInput.GetDown(OVRInput.Button.Four, OVRInput.Controller.LTouch))
            {
                Debug.Log("[Controller] LEFT X button pressed");
            }
        }
        else
        {
            if (!hasLoggedOnce)
            {
                Debug.LogWarning("No VR controller detected! Are you in VR mode or on device?");
                hasLoggedOnce = true;
            }
        }
    }
}
