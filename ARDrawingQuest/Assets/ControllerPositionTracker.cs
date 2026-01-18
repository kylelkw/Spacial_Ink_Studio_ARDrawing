using UnityEngine;

public class ControllerPositionTracker : MonoBehaviour
{
    public Transform rightHandPosition;
    private Transform trackingSpace;
    
    void Start()
    {
        GameObject rightHand = new GameObject("RightHandTracker");
        rightHandPosition = rightHand.transform;
        
        GameObject ovrRig = GameObject.Find("OVRCameraRig");
        if (ovrRig != null)
        {
            trackingSpace = ovrRig.transform.Find("TrackingSpace");
            if (trackingSpace != null)
            {
                rightHandPosition.SetParent(trackingSpace, false);
                Debug.Log("[ControllerTracker] Initialized!");
            }
            else
            {
                Debug.LogError("[ControllerTracker] TrackingSpace not found!");
            }
        }
        else
        {
            Debug.LogError("[ControllerTracker] OVRCameraRig not found!");
        }
    }
    
    void Update()
    {
        if (rightHandPosition != null)
        {
            Vector3 localPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            Quaternion localRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
            
            // Add forward offset to position at controller tip (about 10cm forward)
            Vector3 tipOffset = localRot * new Vector3(0, 0, 0.1f);
            
            rightHandPosition.localPosition = localPos + tipOffset;
            rightHandPosition.localRotation = localRot;
            
            // DEBUG: Show magenta sphere at tracker position
            GameObject debugSphere = GameObject.Find("TrackerDebugSphere");
            if (debugSphere == null)
            {
                debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                debugSphere.name = "TrackerDebugSphere";
                debugSphere.transform.localScale = Vector3.one * 0.02f;
                debugSphere.GetComponent<Renderer>().material.color = Color.magenta;
                Destroy(debugSphere.GetComponent<Collider>());
            }
            debugSphere.transform.position = rightHandPosition.position;
        }
    }
}
