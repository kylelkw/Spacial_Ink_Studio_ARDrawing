using System;
using UnityEngine;

public class VRControllerInputProvider : MonoBehaviour, IInputProvider
{
    public event Action OnDrawPressed;
    public event Action OnDrawReleased;
    public event Action OnCycleBrush;
    public event Action OnCycleColor;
    public event Action OnUndoPressed;
    public event Action OnErasePressed;
    public event Action<float> OnBrushSizeChanged;
    
    [Header("Size Adjustment Settings")]
    public float sizeChangeAmount = 0.001f;
    public float thumbstickThreshold = 0.3f;
    
    private bool wasTriggerPressed = false;
    private float lastThumbstickY = 0f;
    
    void Update()
    {
        // Right Trigger: Draw
        float trigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
        bool isTriggerPressed = trigger > 0.5f;
        
        if (isTriggerPressed && !wasTriggerPressed)
            OnDrawPressed?.Invoke();
        else if (!isTriggerPressed && wasTriggerPressed)
            OnDrawReleased?.Invoke();
        
        wasTriggerPressed = isTriggerPressed;
        
        // Left Y Button: Cycle Brush
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
            OnCycleBrush?.Invoke();
        
        // Left X Button: Cycle Color
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch))
            OnCycleColor?.Invoke();
        
        // Right A Button: Undo
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
            OnUndoPressed?.Invoke();
        
        // Left Thumbstick: Adjust Brush Size
        Vector2 thumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
        
        if (Mathf.Abs(thumbstick.y) > thumbstickThreshold && Mathf.Abs(lastThumbstickY) <= thumbstickThreshold)
        {
            float delta = thumbstick.y > 0 ? sizeChangeAmount : -sizeChangeAmount;
            OnBrushSizeChanged?.Invoke(delta);
        }
        
        lastThumbstickY = thumbstick.y;
    }
}
