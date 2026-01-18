using UnityEngine;

[CreateAssetMenu(fileName = "NewBrush", menuName = "Drawing System/Brush Data")]
public class BrushData : ScriptableObject
{
    public string brushName = "New Brush";
    public GameObject linePrefab;
    public Material brushMaterial;
    
    public LineAlignment alignment = LineAlignment.View;
    public int cornerVertices = 5;
    public int endCapVertices = 5;
    public AnimationCurve widthCurve = AnimationCurve.Linear(0, 1, 1, 1);
    
    public bool usesBillboarding = false;
    public bool appliesSmoothing = true;
    public bool supportsTransparency = false;
    public float alpha = 1f;
    
    public bool enableSimplification = false;
    public float simplificationTolerance = 0.005f;
    
    public Texture2D brushTexture;
    public bool usesTexture = false;
    public Vector2 textureTiling = Vector2.one;
}
