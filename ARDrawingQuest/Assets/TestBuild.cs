using UnityEngine;

public class TestBuild : MonoBehaviour
{
    void Start()
    {
        // Create a big red cube in front of you
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0, 1.5f, 2);
        cube.transform.localScale = Vector3.one * 0.5f;
        cube.GetComponent<Renderer>().material.color = Color.red;
    }
}

