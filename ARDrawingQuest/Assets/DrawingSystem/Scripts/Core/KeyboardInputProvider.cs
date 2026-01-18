using System;
using UnityEngine;

public class KeyboardInputProvider : MonoBehaviour, IInputProvider
{
    public event Action OnDrawPressed;
    public event Action OnDrawReleased;
    public event Action OnCycleBrush;
    public event Action OnCycleColor;
    public event Action OnUndoPressed;
    public event Action OnErasePressed;
    public event Action<float> OnBrushSizeChanged;
    
    [Header("Keyboard Settings")]
    public float sizeChangeAmount = 0.001f;
    
    void Update()
    {
        // Drawing
        if (Input.GetKeyDown(KeyCode.Space)) OnDrawPressed?.Invoke();
        if (Input.GetKeyUp(KeyCode.Space)) OnDrawReleased?.Invoke();
        
        // Brush and Color
        if (Input.GetKeyDown(KeyCode.B)) OnCycleBrush?.Invoke();
        if (Input.GetKeyDown(KeyCode.C)) OnCycleColor?.Invoke();
        
        // Undo
        if (Input.GetKeyDown(KeyCode.U) || Input.GetKeyDown(KeyCode.Z)) OnUndoPressed?.Invoke();
        
        // Erase (not implemented)
        if (Input.GetKeyDown(KeyCode.E)) OnErasePressed?.Invoke();
        
        // Brush Size Adjustment
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.Plus))
        {
            OnBrushSizeChanged?.Invoke(sizeChangeAmount);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Minus))
        {
            OnBrushSizeChanged?.Invoke(-sizeChangeAmount);
        }
    }
}
