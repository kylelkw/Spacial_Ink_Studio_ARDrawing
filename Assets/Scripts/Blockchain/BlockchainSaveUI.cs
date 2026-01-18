using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SpatialInk.Blockchain;
using System.Threading.Tasks;

namespace SpatialInk.UI
{
    /// <summary>
    /// UI Controller for blockchain save functionality
    /// </summary>
    public class BlockchainSaveUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BlockchainManager blockchainManager;
        [SerializeField] private Button saveToBlockchainButton;
        [SerializeField] private Button connectWalletButton;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI walletAddressText;
        [SerializeField] private GameObject loadingPanel;
        
        [Header("Drawing Reference")]
        [SerializeField] private Transform drawingAnchor; // Your AR drawing anchor
        
        private bool isSaving = false;
        
        private void Start()
        {
            InitializeUI();
            InitializeBlockchain();
        }
        
        private void InitializeUI()
        {
            if (saveToBlockchainButton != null)
            {
                saveToBlockchainButton.onClick.AddListener(OnSaveToBlockchain);
                saveToBlockchainButton.interactable = false;
            }
            
            if (connectWalletButton != null)
            {
                connectWalletButton.onClick.AddListener(OnConnectWallet);
            }
            
            if (loadingPanel != null)
            {
                loadingPanel.SetActive(false);
            }
            
            UpdateStatusText("Initializing blockchain...");
        }
        
        private async void InitializeBlockchain()
        {
            if (blockchainManager == null)
            {
                Debug.LogError("BlockchainManager reference is missing!");
                UpdateStatusText("Error: Blockchain manager not found");
                return;
            }
            
            bool success = await blockchainManager.Initialize();
            
            if (success)
            {
                UpdateStatusText("Ready. Connect wallet to save.");
                
                // Subscribe to events
                blockchainManager.OnTransactionComplete += OnTransactionComplete;
                blockchainManager.OnWalletConnected += OnWalletConnected;
                blockchainManager.OnError += OnBlockchainError;
            }
            else
            {
                UpdateStatusText("Failed to initialize blockchain");
            }
        }
        
        private async void OnConnectWallet()
        {
            if (isSaving) return;
            
            UpdateStatusText("Connecting wallet...");
            connectWalletButton.interactable = false;
            
            string address = await blockchainManager.ConnectWallet();
            
            if (!string.IsNullOrEmpty(address))
            {
                OnWalletConnected(address);
            }
            else
            {
                UpdateStatusText("Failed to connect wallet");
                connectWalletButton.interactable = true;
            }
        }
        
        private async void OnSaveToBlockchain()
        {
            if (isSaving) return;
            
            isSaving = true;
            saveToBlockchainButton.interactable = false;
            
            if (loadingPanel != null)
            {
                loadingPanel.SetActive(true);
            }
            
            UpdateStatusText("Checking location availability...");
            
            // Get current drawing position
            Vector3 position = drawingAnchor != null ? drawingAnchor.position : Vector3.zero;
            
            // Check if location is available
            bool available = await blockchainManager.IsLocationAvailable(position);
            
            if (!available)
            {
                UpdateStatusText("Location already claimed!");
                ResetSaveButton();
                return;
            }
            
            UpdateStatusText("Preparing artwork data...");
            
            // Create drawing metadata
            DrawingMetadata metadata = CreateDrawingMetadata(position);
            
            UpdateStatusText("Uploading to IPFS and blockchain...");
            
            // Claim location on blockchain
            TransactionResponse response = await blockchainManager.ClaimLocation(position, metadata);
            
            // Response will be handled by OnTransactionComplete event
        }
        
        private DrawingMetadata CreateDrawingMetadata(Vector3 anchorPosition)
        {
            // TODO: Collect actual drawing data from your drawing system
            // This is a placeholder implementation
            
            return new DrawingMetadata
            {
                title = "AR Graffiti",
                description = "Created with Spatial Ink Studio",
                artistName = "Anonymous",
                createdAt = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                anchorPosition = anchorPosition,
                anchorRotation = drawingAnchor != null ? drawingAnchor.rotation : Quaternion.identity,
                
                // TODO: Get actual stroke data from your drawing system
                strokePoints = new Vector3[0],
                strokeColors = new Color[0],
                strokeWidths = new float[0]
            };
        }
        
        private void OnWalletConnected(string address)
        {
            if (walletAddressText != null)
            {
                walletAddressText.text = $"Wallet: {ShortenAddress(address)}";
            }
            
            UpdateStatusText("Wallet connected! Ready to save.");
            
            if (saveToBlockchainButton != null)
            {
                saveToBlockchainButton.interactable = true;
            }
            
            if (connectWalletButton != null)
            {
                connectWalletButton.gameObject.SetActive(false);
            }
        }
        
        private void OnTransactionComplete(TransactionResponse response)
        {
            if (response.success)
            {
                UpdateStatusText($"Success! TX: {ShortenAddress(response.transactionHash)}");
                Debug.Log($"Graffiti saved to blockchain! Transaction: {response.transactionHash}");
                
                // TODO: Show success animation or feedback
            }
            else
            {
                UpdateStatusText($"Failed: {response.errorMessage}");
                Debug.LogError($"Transaction failed: {response.errorMessage}");
            }
            
            ResetSaveButton();
        }
        
        private void OnBlockchainError(string error)
        {
            UpdateStatusText($"Error: {error}");
            Debug.LogError($"Blockchain error: {error}");
            ResetSaveButton();
        }
        
        private void ResetSaveButton()
        {
            isSaving = false;
            
            if (loadingPanel != null)
            {
                loadingPanel.SetActive(false);
            }
            
            if (saveToBlockchainButton != null && !string.IsNullOrEmpty(blockchainManager.WalletAddress))
            {
                saveToBlockchainButton.interactable = true;
            }
        }
        
        private void UpdateStatusText(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
            Debug.Log($"[BlockchainSave] {message}");
        }
        
        private string ShortenAddress(string address)
        {
            if (string.IsNullOrEmpty(address) || address.Length < 10)
                return address;
            
            return $"{address.Substring(0, 6)}...{address.Substring(address.Length - 4)}";
        }
        
        private void OnDestroy()
        {
            if (blockchainManager != null)
            {
                blockchainManager.OnTransactionComplete -= OnTransactionComplete;
                blockchainManager.OnWalletConnected -= OnWalletConnected;
                blockchainManager.OnError -= OnBlockchainError;
            }
            
            if (saveToBlockchainButton != null)
            {
                saveToBlockchainButton.onClick.RemoveListener(OnSaveToBlockchain);
            }
            
            if (connectWalletButton != null)
            {
                connectWalletButton.onClick.RemoveListener(OnConnectWallet);
            }
        }
    }
}
