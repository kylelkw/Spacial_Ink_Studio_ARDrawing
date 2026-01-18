# Quest 3 Controller Mapping - Visual Guide

## LEFT CONTROLLER (Input)

```
         [Y Button] ← Cycle Brush Type
              ▲
              │
         [X Button] ← Cycle Color
              ▲
              │
         ┌────────┐
         │  (●)   │ ← Thumbstick
         │   │    │    Up/Down = Adjust Brush Size
         └────────┘
              │
         Controller Body
              │
         [Trigger] (not used)
```

**LEFT Controller Functions:**
- **Y Button** → Switch between Wire and Flat brush
- **X Button** → Cycle through 8 colors
- **Thumbstick Up** → Increase brush size (+0.001m per push)
- **Thumbstick Down** → Decrease brush size (-0.001m per push)

---

## RIGHT CONTROLLER (Draw & Undo)

```
         [B Button] (not used)
              ▲
              │
         [A Button] ← Undo Last Line
              ▲
              │
         ┌────────┐
         │  (●)   │ ← Thumbstick (not used)
         │        │
         └────────┘
              │
         Controller Body
              │
         [Trigger] ← HOLD TO DRAW ★
              ▼
         (Drawing Position)
```

**RIGHT Controller Functions:**
- **Trigger (Hold)** → Draw line in 3D space
- **A Button** → Remove the most recently drawn line

---

## Button Identification

### Meta Quest 3 Button Layout

**LEFT CONTROLLER:**
```
    Y (upper) ← Button.Three in code
    X (lower) ← Button.Four in code
   Trigger    ← Not used for drawing
```

**RIGHT CONTROLLER:**
```
    B (upper) ← Not used
    A (lower) ← Button.One in code
   Trigger    ← PrimaryIndexTrigger in code
```

---

## Drawing Workflow

### Step-by-Step Drawing Process

1. **Choose Brush Type**
   ```
   Press LEFT Y repeatedly to cycle:
   Wire → Flat → Wire → ...
   ```

2. **Choose Color**
   ```
   Press LEFT X repeatedly to cycle:
   Red → Blue → Green → Yellow → Orange → Magenta → Cyan → White → Red...
   ```

3. **Adjust Size (Optional)**
   ```
   Move LEFT Thumbstick:
   Up ↑ = Larger brush (max 0.02m)
   Down ↓ = Smaller brush (min 0.005m)
   ```

4. **Draw**
   ```
   1. Hold RIGHT Trigger
   2. Move controller in 3D space
   3. Line appears at controller tip
   4. Release trigger when done
   ```

5. **Undo (If Needed)**
   ```
   Press RIGHT A to remove last line
   Can press multiple times to remove multiple lines
   ```

---

## Visual Feedback

### What You'll See:

**While Drawing:**
- Colored line appears at controller position
- Line extends as you move controller
- Minimum 1cm movement required between points

**After Releasing:**
- Line smooths automatically (Catmull-Rom)
- Line stays in 3D space
- Controller can move freely (line doesn't follow)

**When Cycling Brush:**
- Console log shows brush name
- Next line will use new thickness

**When Cycling Color:**
- Console log shows color name
- Next line will use new color

**When Adjusting Size:**
- Console log shows new size (e.g., "0.012m")
- Applies to next line drawn

---

## OVRInput Code Reference

For developers modifying the input system:

```csharp
// RIGHT Trigger (Draw)
float trigger = OVRInput.Get(
    OVRInput.Axis1D.PrimaryIndexTrigger, 
    OVRInput.Controller.RTouch
);
bool isPressed = trigger > 0.5f;

// LEFT Y Button (Cycle Brush)
bool yPressed = OVRInput.GetDown(
    OVRInput.Button.Three, 
    OVRInput.Controller.LTouch
);

// LEFT X Button (Cycle Color)
bool xPressed = OVRInput.GetDown(
    OVRInput.Button.Four, 
    OVRInput.Controller.LTouch
);

// RIGHT A Button (Undo)
bool aPressed = OVRInput.GetDown(
    OVRInput.Button.One, 
    OVRInput.Controller.RTouch
);

// LEFT Thumbstick (Size Adjust)
Vector2 thumbstick = OVRInput.Get(
    OVRInput.Axis2D.PrimaryThumbstick, 
    OVRInput.Controller.LTouch
);
// thumbstick.y > 0.3f = Up (increase)
// thumbstick.y < -0.3f = Down (decrease)
```

---

## Common Control Mistakes

### ❌ Wrong Button Assignments (DON'T DO THIS)

```
LEFT Y → OVRInput.Button.Two      // WRONG! This is B button
LEFT X → OVRInput.Button.One      // WRONG! This is A button
RIGHT A → OVRInput.Button.Three   // WRONG! This is Y button
```

### ✅ Correct Button Assignments (DO THIS)

```
LEFT Y → OVRInput.Button.Three    // CORRECT
LEFT X → OVRInput.Button.Four     // CORRECT
RIGHT A → OVRInput.Button.One     // CORRECT
```

---

## Accessibility Notes

### Single-Handed Operation
- **Not possible** - requires both controllers
- LEFT hand: Brush/color selection
- RIGHT hand: Drawing

### Standing vs. Seated
- **Both supported**
- TrackingOriginType: Floor Level
- Drawing works at any height

### Room-Scale Drawing
- Can draw anywhere in Guardian boundary
- Lines persist in world space
- Walk around drawings to view from different angles

---

## Tips for Best Drawing Experience

1. **Start Simple**
   - Practice drawing basic shapes (circle, square, line)
   - Get comfortable with trigger pressure

2. **Use Both Brushes**
   - Wire brush: Details and outlines
   - Flat brush: Bold strokes and fills

3. **Color Coding**
   - Use different colors for different parts of drawing
   - Easy to identify which lines to undo

4. **Size Matters**
   - Small size (0.005m): Precise details
   - Large size (0.02m): Quick bold strokes
   - Adjust mid-drawing for variety

5. **Undo Freely**
   - Don't hesitate to undo and redraw
   - Practice makes perfect!

6. **Move Around**
   - Walk around your drawing
   - View from different angles
   - Draw from multiple perspectives

---

## Hardware Requirements

**Minimum:**
- Meta Quest 3
- Both controllers charged
- Guardian boundary set up
- Developer mode enabled (for builds)

**Recommended:**
- Well-lit room (for passthrough quality)
- 2m × 2m clear space minimum
- Fresh controller batteries

---

## Controller Haptics

**Current Implementation:**
- No haptic feedback implemented

**Potential Future Enhancement:**
```
On draw start: Light haptic pulse
During drawing: Continuous subtle vibration
On undo: Sharp haptic pulse
```

---

## Controller Tracking

**How It Works:**
1. Quest 3 tracks controller position in 3D space
2. OVRInput provides local position
3. TrackingSpace converts to world position
4. Line drawn at world position
5. Position updates every frame while drawing

**Tracking Quality:**
- Best: Controllers at chest/shoulder height
- Good: Arms extended forward
- Poor: Behind back, occluded by body

---

**All controls tested and verified! Ready to draw in AR!** 🎨
