// using UnityEngine;

// public class SimpleDebug : MonoBehaviour
// {
//     private GameObject sphere;
//     private GameObject statusSphere1;
//     private GameObject statusSphere2;
//     private GameObject statusSphere3;
    
//     void Start()
//     {
//         sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//         sphere.transform.localScale = Vector3.one * 0.2f;
//         Destroy(sphere.GetComponent<Collider>());
        
//         statusSphere1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//         statusSphere1.transform.localScale = Vector3.one * 0.1f;
//         Destroy(statusSphere1.GetComponent<Collider>());
        
//         statusSphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//         statusSphere2.transform.localScale = Vector3.one * 0.1f;
//         Destroy(statusSphere2.GetComponent<Collider>());
        
//         statusSphere3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//         statusSphere3.transform.localScale = Vector3.one * 0.1f;
//         Destroy(statusSphere3.GetComponent<Collider>());
//     }
    
//     void Update()
//     {
//         Camera cam = Camera.main;
//         if (cam == null) return;
        
//         Vector3 forward = cam.transform.position + cam.transform.forward * 1.5f;
//         sphere.transform.position = forward;
//         statusSphere1.transform.position = forward + Vector3.left * 0.3f;
//         statusSphere2.transform.position = forward + Vector3.right * 0.3f;
//         statusSphere3.transform.position = forward + Vector3.up * 0.3f;
        
//         // Main sphere: Trigger state
//         float trigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
//         sphere.GetComponent<Renderer>().material.color = trigger > 0.5f ? Color.green : Color.red;
        
//         // Status 1: Input provider
//         VRControllerInputProvider input = FindObjectOfType<VRControllerInputProvider>();
//         statusSphere1.GetComponent<Renderer>().material.color = input != null ? Color.green : Color.red;
        
//         // Status 2: Check controller position using OVRInput directly
//         Vector3 controllerPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
//         bool validTracking = controllerPos.magnitude > 0.01f;
        
//         // Create a cyan cube at the reported position
//         GameObject cube = GameObject.Find("DebugCube");
//         if (cube == null)
//         {
//             cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//             cube.name = "DebugCube";
//             cube.transform.localScale = Vector3.one * 0.1f;
//             Destroy(cube.GetComponent<Collider>());
//         }
//         cube.GetComponent<Renderer>().material.color = Color.cyan;
//         cube.transform.localPosition = controllerPos;
        
//         // Color the right sphere
//         statusSphere2.GetComponent<Renderer>().material.color = validTracking ? Color.green : Color.red;
        
//         // Status 3: DrawingManager
//         DrawingManager dm = FindObjectOfType<DrawingManager>();
//         statusSphere3.GetComponent<Renderer>().material.color = dm != null ? Color.green : Color.red;
//     }
// }
