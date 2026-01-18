#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SuppressOVRErrors
{
    static SuppressOVRErrors()
    {
        Application.logMessageReceived += HandleLog;
    }

    static void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Suppress OVR config errors
        if (type == LogType.Error && 
            (logString.Contains("OVRProjectConfig") || 
             logString.Contains("OVRProjectSetup") ||
             logString.Contains("MetaXRSimulator")))
        {
            // Don't display these errors
            return;
        }
    }
}
#endif
