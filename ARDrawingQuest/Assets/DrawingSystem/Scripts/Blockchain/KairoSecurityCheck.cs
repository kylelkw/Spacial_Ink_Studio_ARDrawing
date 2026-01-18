using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Kairo AI Security API Integration for Smart Contract validation
/// Checks contracts before deployment to ensure security
/// </summary>
public class KairoSecurityCheck : MonoBehaviour
{
    [Header("Kairo API Configuration")]
    [Tooltip("Your Kairo API Key (kairo_sk_live_xxxxx)")]
    public string apiKey = "YOUR_KAIRO_API_KEY";
    
    [Tooltip("Kairo API Base URL")]
    public string apiBaseUrl = "https://api.kairoaisec.com/v1";
    
    [Header("Security Settings")]
    [Tooltip("Minimum severity threshold: critical, high, medium, low")]
    public string severityThreshold = "high";
    
    [Tooltip("Include AI suggestions in response")]
    public bool includeSuggestions = true;
    
    [Tooltip("Block deployment on security issues")]
    public bool blockOnSecurityIssues = true;
    
    public enum SecurityDecision
    {
        ALLOW,
        WARN,
        BLOCK,
        ESCALATE
    }
    
    [Serializable]
    public class KairoAnalysisResult
    {
        public SecurityDecision decision;
        public string decision_reason;
        public int risk_score;
        public SecuritySummary summary;
        public string[] issues;
        public bool isSecure;
    }
    
    [Serializable]
    public class SecuritySummary
    {
        public int total;
        public int critical;
        public int high;
        public int medium;
        public int low;
    }
    
    [Serializable]
    private class AnalyzeRequest
    {
        public SourceData source;
        public ConfigData config;
    }
    
    [Serializable]
    private class SourceData
    {
        public string type = "inline";
        public FileData[] files;
    }
    
    [Serializable]
    private class FileData
    {
        public string path;
        public string content;
    }
    
    [Serializable]
    private class ConfigData
    {
        public string severity_threshold;
        public bool include_suggestions;
    }
    
    [Serializable]
    private class DeployCheckRequest
    {
        public string project_id;
        public string contract_name;
        public NetworkData network;
    }
    
    [Serializable]
    private class NetworkData
    {
        public int chain_id;
        public string name;
    }
    
    /// <summary>
    /// Analyze smart contract code for security issues
    /// </summary>
    /// <param name="contractCode">Solidity source code</param>
    /// <param name="contractName">Name of the contract file</param>
    /// <param name="onComplete">Callback with analysis result</param>
    public void AnalyzeContract(string contractCode, string contractName, Action<KairoAnalysisResult> onComplete)
    {
        if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_KAIRO_API_KEY")
        {
            Debug.LogError("Kairo API Key not configured! Please set your API key in the inspector.");
            onComplete?.Invoke(new KairoAnalysisResult 
            { 
                decision = SecurityDecision.BLOCK,
                decision_reason = "API Key not configured",
                isSecure = false
            });
            return;
        }
        
        StartCoroutine(AnalyzeContractCoroutine(contractCode, contractName, onComplete));
    }
    
    private IEnumerator AnalyzeContractCoroutine(string contractCode, string contractName, Action<KairoAnalysisResult> onComplete)
    {
        string endpoint = $"{apiBaseUrl}/analyze";
        
        // Build request payload
        AnalyzeRequest request = new AnalyzeRequest
        {
            source = new SourceData
            {
                type = "inline",
                files = new FileData[]
                {
                    new FileData
                    {
                        path = contractName,
                        content = contractCode
                    }
                }
            },
            config = new ConfigData
            {
                severity_threshold = severityThreshold,
                include_suggestions = includeSuggestions
            }
        };
        
        string jsonPayload = JsonUtility.ToJson(request);
        
        // Make API request
        using (UnityWebRequest www = new UnityWebRequest(endpoint, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            
            Debug.Log($"[Kairo] Analyzing contract: {contractName}");
            
            yield return www.SendWebRequest();
            
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[Kairo] API Error: {www.error}");
                Debug.LogError($"[Kairo] Response: {www.downloadHandler.text}");
                
                onComplete?.Invoke(new KairoAnalysisResult
                {
                    decision = SecurityDecision.BLOCK,
                    decision_reason = $"API call failed: {www.error}",
                    isSecure = false
                });
            }
            else
            {
                string responseText = www.downloadHandler.text;
                Debug.Log($"[Kairo] Raw Response: {responseText}");
                
                try
                {
                    KairoAnalysisResult result = ParseKairoResponse(responseText);
                    
                    // Log results
                    Debug.Log($"[Kairo] Security Decision: {result.decision}");
                    Debug.Log($"[Kairo] Risk Score: {result.risk_score}");
                    Debug.Log($"[Kairo] Reason: {result.decision_reason}");
                    
                    if (result.summary != null)
                    {
                        Debug.Log($"[Kairo] Issues - Critical: {result.summary.critical}, High: {result.summary.high}, Medium: {result.summary.medium}, Low: {result.summary.low}");
                    }
                    
                    onComplete?.Invoke(result);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[Kairo] Failed to parse response: {e.Message}");
                    
                    onComplete?.Invoke(new KairoAnalysisResult
                    {
                        decision = SecurityDecision.BLOCK,
                        decision_reason = $"Failed to parse API response: {e.Message}",
                        isSecure = false
                    });
                }
            }
        }
    }
    
    /// <summary>
    /// Perform pre-deployment security check
    /// </summary>
    public void DeploymentCheck(string projectId, string contractName, int chainId, string networkName, Action<KairoAnalysisResult> onComplete)
    {
        StartCoroutine(DeploymentCheckCoroutine(projectId, contractName, chainId, networkName, onComplete));
    }
    
    private IEnumerator DeploymentCheckCoroutine(string projectId, string contractName, int chainId, string networkName, Action<KairoAnalysisResult> onComplete)
    {
        string endpoint = $"{apiBaseUrl}/deploy/check";
        
        DeployCheckRequest request = new DeployCheckRequest
        {
            project_id = projectId,
            contract_name = contractName,
            network = new NetworkData
            {
                chain_id = chainId,
                name = networkName
            }
        };
        
        string jsonPayload = JsonUtility.ToJson(request);
        
        using (UnityWebRequest www = new UnityWebRequest(endpoint, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            
            Debug.Log($"[Kairo] Deployment check for: {contractName} on {networkName}");
            
            yield return www.SendWebRequest();
            
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[Kairo] Deployment check failed: {www.error}");
                
                onComplete?.Invoke(new KairoAnalysisResult
                {
                    decision = SecurityDecision.BLOCK,
                    decision_reason = $"Deployment check failed: {www.error}",
                    isSecure = false
                });
            }
            else
            {
                try
                {
                    KairoAnalysisResult result = ParseKairoResponse(www.downloadHandler.text);
                    onComplete?.Invoke(result);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[Kairo] Failed to parse deployment check response: {e.Message}");
                    
                    onComplete?.Invoke(new KairoAnalysisResult
                    {
                        decision = SecurityDecision.BLOCK,
                        decision_reason = $"Failed to parse response: {e.Message}",
                        isSecure = false
                    });
                }
            }
        }
    }
    
    private KairoAnalysisResult ParseKairoResponse(string jsonResponse)
    {
        // Simple JSON parsing (you may want to use a better JSON library)
        KairoAnalysisResult result = new KairoAnalysisResult();
        
        // Parse decision
        if (jsonResponse.Contains("\"decision\":\"ALLOW\""))
        {
            result.decision = SecurityDecision.ALLOW;
            result.isSecure = true;
        }
        else if (jsonResponse.Contains("\"decision\":\"WARN\""))
        {
            result.decision = SecurityDecision.WARN;
            result.isSecure = true;
        }
        else if (jsonResponse.Contains("\"decision\":\"BLOCK\""))
        {
            result.decision = SecurityDecision.BLOCK;
            result.isSecure = false;
        }
        else if (jsonResponse.Contains("\"decision\":\"ESCALATE\""))
        {
            result.decision = SecurityDecision.ESCALATE;
            result.isSecure = false;
        }
        
        // Extract decision_reason (simple approach)
        int reasonStart = jsonResponse.IndexOf("\"decision_reason\":\"") + 19;
        if (reasonStart > 18)
        {
            int reasonEnd = jsonResponse.IndexOf("\"", reasonStart);
            result.decision_reason = jsonResponse.Substring(reasonStart, reasonEnd - reasonStart);
        }
        
        // Extract risk_score
        int riskStart = jsonResponse.IndexOf("\"risk_score\":") + 13;
        if (riskStart > 12)
        {
            int riskEnd = jsonResponse.IndexOf(",", riskStart);
            if (riskEnd == -1) riskEnd = jsonResponse.IndexOf("}", riskStart);
            if (int.TryParse(jsonResponse.Substring(riskStart, riskEnd - riskStart), out int risk))
            {
                result.risk_score = risk;
            }
        }
        
        // Parse summary if present
        if (jsonResponse.Contains("\"summary\""))
        {
            result.summary = new SecuritySummary();
            
            // Extract counts (simple parsing)
            ExtractCount(jsonResponse, "\"critical\":", out result.summary.critical);
            ExtractCount(jsonResponse, "\"high\":", out result.summary.high);
            ExtractCount(jsonResponse, "\"medium\":", out result.summary.medium);
            ExtractCount(jsonResponse, "\"low\":", out result.summary.low);
            ExtractCount(jsonResponse, "\"total\":", out result.summary.total);
        }
        
        return result;
    }
    
    private void ExtractCount(string json, string field, out int value)
    {
        value = 0;
        int start = json.IndexOf(field);
        if (start > -1)
        {
            start += field.Length;
            int end = json.IndexOf(",", start);
            if (end == -1) end = json.IndexOf("}", start);
            if (end > start)
            {
                string numStr = json.Substring(start, end - start).Trim();
                int.TryParse(numStr, out value);
            }
        }
    }
}
