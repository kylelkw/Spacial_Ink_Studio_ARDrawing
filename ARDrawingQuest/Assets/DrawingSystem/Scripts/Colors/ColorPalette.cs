using UnityEngine;

[CreateAssetMenu(fileName = "ColorPalette", menuName = "Drawing System/Color Palette")]
public class ColorPalette : ScriptableObject
{
    public ColorData[] colors = new ColorData[9];
    
    void OnValidate()
    {
        if (colors == null || colors.Length != 9)
        {
            colors = new ColorData[9];
            colors[0] = new ColorData("Red", Color.red);
            colors[1] = new ColorData("Blue", Color.blue);
            colors[2] = new ColorData("Green", Color.green);
            colors[3] = new ColorData("Yellow", Color.yellow);
            colors[4] = new ColorData("Orange", new Color(1f, 0.5f, 0f));
            colors[5] = new ColorData("Magenta", Color.magenta);
            colors[6] = new ColorData("Cyan", Color.cyan);
            colors[7] = new ColorData("White", Color.white);
            colors[8] = new ColorData("Black", Color.black);
        }
    }
    
    public Color GetColor(int index)
    {
        if (index >= 0 && index < colors.Length)
            return colors[index].color;
        return Color.white;
    }
}
