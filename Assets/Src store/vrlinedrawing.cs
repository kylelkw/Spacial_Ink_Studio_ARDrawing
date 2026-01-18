using UnityEngine;

public class VRLineDrawingOVR : MonoBehaviour
{
    [Header("Drawing Settings")]
    [SerializeField] private OVRInput.Controller controller = OVRInput.Controller.RTouch;
    [SerializeField] private OVRInput.Button drawButton = OVRInput.Button.PrimaryIndexTrigger;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private float lineWidth = 0.01f;
    [SerializeField] private Color lineColor = Color.white;
    [SerializeField] private float minDistanceBetweenPoints = 0.01f;

    private LineRenderer currentLine;
    private bool isDrawing = false;
    private Vector3 lastPosition;

    void Update()
    {
        // Check if the draw button is pressed
        bool buttonPressed = OVRInput.Get(drawButton, controller);

        if (buttonPressed && !isDrawing)
        {
            StartDrawing();
        }
        else if (buttonPressed && isDrawing)
        {
            ContinueDrawing();
        }
        else if (!buttonPressed && isDrawing)
        {
            StopDrawing();
        }
    }

    void StartDrawing()
    {
        isDrawing = true;

        // Create a new GameObject for this line
        GameObject lineObject = new GameObject("DrawnLine");
        currentLine = lineObject.AddComponent<LineRenderer>();

        // Configure the LineRenderer
        currentLine.material = lineMaterial;
        currentLine.startWidth = lineWidth;
        currentLine.endWidth = lineWidth;
        currentLine.startColor = lineColor;
        currentLine.endColor = lineColor;
        currentLine.numCapVertices = 5;
        currentLine.numCornerVertices = 5;
        currentLine.useWorldSpace = true;

        // Add the first point
        lastPosition = transform.position;
        currentLine.positionCount = 1;
        currentLine.SetPosition(0, lastPosition);
    }

    void ContinueDrawing()
    {
        Vector3 currentPosition = transform.position;

        // Only add a new point if we've moved far enough
        if (Vector3.Distance(currentPosition, lastPosition) >= minDistanceBetweenPoints)
        {
            int positionCount = currentLine.positionCount;
            currentLine.positionCount = positionCount + 1;
            currentLine.SetPosition(positionCount, currentPosition);
            lastPosition = currentPosition;
        }
    }

    void StopDrawing()
    {
        isDrawing = false;
        currentLine = null;
    }

    // Optional: Add an erase function
    public void EraseAllLines()
    {
        GameObject[] lines = GameObject.FindGameObjectsWithTag("DrawnLine");
        foreach (GameObject line in lines)
        {
            Destroy(line);
        }
    }
}