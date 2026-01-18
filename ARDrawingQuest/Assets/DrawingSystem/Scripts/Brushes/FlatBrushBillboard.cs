using UnityEngine;

/// <summary>
/// Makes flat brush lines face the camera (billboard effect)
/// Attach to flat brush prefabs
/// </summary>
public class FlatBrushBillboard : MonoBehaviour
{
    private Transform cameraTransform;
    private LineRenderer lineRenderer;
    
    void Start()
    {
        cameraTransform = Camera.main.transform;
        
        if (cameraTransform == null)
        {
            // Try finding VR camera
            GameObject cameraRig = GameObject.Find("OVRCameraRig");
            if (cameraRig != null)
            {
                cameraTransform = cameraRig.transform.Find("TrackingSpace/CenterEyeAnchor");
            }
        }
        
        lineRenderer = GetComponent<LineRenderer>();
    }
    
    void LateUpdate()
    {
        if (cameraTransform != null && lineRenderer != null && lineRenderer.positionCount > 0)
        {
            // Get center point of line
            Vector3 center = Vector3.zero;
            Vector3[] positions = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(positions);
            
            foreach (Vector3 pos in positions)
            {
                center += pos;
            }
            center /= positions.Length;
            
            // Face camera
            Vector3 directionToCamera = cameraTransform.position - center;
            transform.rotation = Quaternion.LookRotation(-directionToCamera);
        }
    }
}
