using System.Collections.Generic;
using UnityEngine;

public static class LineSmoother
{
    public static Vector3[] SmoothLine(Vector3[] points, int factor = 2)
    {
        if (points == null || points.Length < 3) return points;
        
        List<Vector3> smoothed = new List<Vector3>();
        smoothed.Add(points[0]);
        
        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector3 p0 = i == 0 ? points[i] : points[i - 1];
            Vector3 p1 = points[i];
            Vector3 p2 = points[i + 1];
            Vector3 p3 = i + 2 < points.Length ? points[i + 2] : points[i + 1];
            
            for (int j = 1; j <= factor; j++)
            {
                float t = j / (float)factor;
                float t2 = t * t;
                float t3 = t2 * t;
                
                Vector3 point = 0.5f * (
                    (2f * p1) +
                    (-p0 + p2) * t +
                    (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
                    (-p0 + 3f * p1 - 3f * p2 + p3) * t3
                );
                smoothed.Add(point);
            }
        }
        
        smoothed.Add(points[points.Length - 1]);
        return smoothed.ToArray();
    }
    
    public static Vector3[] SimplifyLine(Vector3[] points, float tolerance)
    {
        if (points == null || points.Length < 3) return points;
        return points; // Simplified for now
    }
}
