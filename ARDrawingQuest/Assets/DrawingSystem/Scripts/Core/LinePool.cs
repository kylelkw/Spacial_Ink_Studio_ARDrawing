using System.Collections.Generic;
using UnityEngine;

public class LinePool : MonoBehaviour
{
    private Dictionary<int, Queue<GameObject>> pools = new Dictionary<int, Queue<GameObject>>();
    private List<BrushData> brushes;
    
    public void InitializeWithBrushes(List<BrushData> brushList)
    {
        brushes = brushList;
        
        for (int i = 0; i < brushes.Count; i++)
        {
            pools[i] = new Queue<GameObject>();
            
            for (int j = 0; j < 10; j++)
            {
                GameObject line = Instantiate(brushes[i].linePrefab);
                line.SetActive(false);
                pools[i].Enqueue(line);
            }
        }
    }
    
    public GameObject GetLine(int brushIndex)
    {
        if (!pools.ContainsKey(brushIndex)) return null;
        
        GameObject line;
        if (pools[brushIndex].Count > 0)
        {
            line = pools[brushIndex].Dequeue();
        }
        else
        {
            line = Instantiate(brushes[brushIndex].linePrefab);
        }
        
        line.SetActive(true);
        return line;
    }
    
    public void ReturnLine(GameObject line, int brushIndex)
    {
        if (line == null) return;
        line.SetActive(false);
        
        LineRenderer lr = line.GetComponent<LineRenderer>();
        if (lr != null) lr.positionCount = 0;
        
        pools[brushIndex].Enqueue(line);
    }
}
