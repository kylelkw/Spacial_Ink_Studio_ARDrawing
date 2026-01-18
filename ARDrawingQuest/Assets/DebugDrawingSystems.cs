using UnityEngine;

public class DebugDrawingSystem : MonoBehaviour
{
    private GameObject statusDisplay;
    
    void Start()
    {
        // Create visual status display in VR
        CreateStatusDisplay();
    }
    
    void CreateStatusDisplay()
    {
        statusDisplay = GameObject.CreatePrimitive(PrimitiveType.Cube);
        statusDisplay.transform.localScale = new Vector3(0.3f, 0.2f, 0.01f);
        statusDisplay.transform.position = new Vector3(0, 1.5f, 2);
        Destroy(statusDisplay.GetComponent<Collider>());
    }
    
    void Update()
    {
        UpdateStatus();
    }
    
    void UpdateStatus()
    {
        if (statusDisplay == null) return;
        
        // Position in front of camera
        Camera cam = Camera.main;
        if (cam != null)
        {
            statusDisplay.transform.position = cam.transform.position + cam.transform.forward * 1.5f;
            statusDisplay.transform.LookAt(cam.transform);
        }
        
        // Check trigger
        float trigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
        
        // Check components
        VRControllerInputProvider input = FindObjectOfType<VRControllerInputProvider>();
        DrawingManager dm = FindObjectOfType<DrawingManager>();
        
        // Check TrackingSpace
        GameObject ovrRig = GameObject.Find("OVRCameraRig");
        Transform trackingSpace = null;
        if (ovrRig != null)
        {
            trackingSpace = ovrRig.transform.Find("TrackingSpace");
        }
        
        // Color code the cube based on status
        Color statusColor = Color.red; // Default: something is wrong
        
        if (input == null)
        {
            statusColor = Color.red; // No input provider
            Debug.LogError("[Debug] VRControllerInputProvider NOT FOUND on GameManager!");
        }
        else if (dm == null)
        {
            statusColor = new Color(1f, 0.5f, 0f); // Orange: No drawing manager
            Debug.LogError("[Debug] DrawingManager NOT FOUND on GameManager!");
        }
        else if (trackingSpace == null)
        {
            statusColor = Color.yellow; // No tracking space
            Debug.LogError("[Debug] TrackingSpace NOT FOUND in scene!");
        }
        else if (trigger > 0.5f)
        {
            statusColor = Color.green; // Everything working, trigger pressed!
            Debug.Log($"[Debug] TRIGGER PRESSED: {trigger:F2} - Drawing should be active!");
        }
        else
        {
            statusColor = Color.blue; // Everything set up correctly, waiting for trigger
        }
        
        statusDisplay.GetComponent<Renderer>().material.color = statusColor;
    }
}
