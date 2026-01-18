using UnityEngine;

[System.Serializable]
public class ColorData
{
    public string colorName = "Unnamed";
    public Color color = Color.white;
    
    public ColorData(string name, Color col)
    {
        colorName = name;
        color = col;
    }
}
