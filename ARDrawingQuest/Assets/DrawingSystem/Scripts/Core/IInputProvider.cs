using System;

public interface IInputProvider
{
    event Action OnDrawPressed;
    event Action OnDrawReleased;
    event Action OnCycleBrush;
    event Action OnCycleColor;
    event Action OnUndoPressed;
    event Action OnErasePressed;
}
