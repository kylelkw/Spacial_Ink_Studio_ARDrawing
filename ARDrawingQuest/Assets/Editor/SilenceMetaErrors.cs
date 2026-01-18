#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class BlockMetaLogs
{
    private static readonly MethodInfo clearMethod;
    
    static BlockMetaLogs()
    {
        // Get internal Clear method
        var consoleWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
        clearMethod = consoleWindowType?.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
        
        Application.logMessageReceivedThreaded += OnLog;
    }
    
    private static void OnLog(string condition, string stackTrace, LogType type)
    {
        if (ShouldBlock(condition, stackTrace))
        {
            EditorApplication.delayCall += () => clearMethod?.Invoke(null, null);
        }
    }
    
    private static bool ShouldBlock(string message, string stack)
    {
        return message.Contains("Meta XR Simulator") ||
               message.Contains("OVRProjectConfig") ||
               message.Contains("XRSimInstallationDetector") ||
               message.Contains("WindowsXRSimInstallationDetector") ||
               stack.Contains("MetaXRSimulator") ||
               stack.Contains("OVRProjectSetup");
    }
}
#endif
