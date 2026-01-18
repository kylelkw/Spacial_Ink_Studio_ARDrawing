# Spatial Ink Studio AR Drawing - Complete Setup Guide

## Quick Start Checklist

### 1. Unity Project Setup
- ✅ Unity Version: **2022.3 LTS** (confirmed)
- ✅ Platform: **Android** 
- ✅ Scripts: All core scripts implemented

### 2. Meta XR SDK Installation

**CRITICAL:** Use Meta XR SDK version **68.0.0** or **71.0.0** - DO NOT use 83.0.1 (has critical bugs)

1. Open **Window > Package Manager**
2. Click **+** > **Add package by name**
3. Enter: `com.meta.xr.sdk.all`
4. Select version **68.0.0** or **71.0.0** from dropdown
5. Click **Add**

### 3. Project Settings Configuration

#### Platform Settings
1. **File > Build Settings**
   - Select **Android**
   - Click **Switch Platform**

2. **Player Settings > Android Settings**
   - **Minimum API Level:** Android 10.0 (API Level 29)
   - **Scripting Backend:** IL2CPP
   - **Target Architectures:** ARM64 ✓

3. **Player Settings > Other Settings**
   - **Color Space:** Linear
   - **Graphics APIs:** OpenGLES3 only (remove Vulkan if present)
   - **Managed Stripping Level:** Minimal
   - **Texture Compression:** ASTC

4. **XR Plug-in Management (Android tab)**
   - Enable **Oculus** ✓

5. **Quality Settings**
   - Set active quality level to **Medium** or **Low** for Quest 3

---

## Scene Setup Instructions

### Step 1: Create New Scene
1. **File > New Scene** (3D Core)
2. Save as `ARDrawingScene`

### Step 2: Add OVRCameraRig
1. **Delete** default Main Camera
2. **Right-click** in Hierarchy > **XR > OVR Camera Rig** (from Meta XR SDK)
   - Alternatively: Use Meta XR Building Blocks > Camera Rig
3. Select `OVRCameraRig` GameObject
4. In Inspector, ensure **OVRManager** component is attached
5. OVRManager Settings:
   - **Tracking Origin Type:** Floor Level
   - **Target Devices:** Quest 3

### Step 3: Enable Passthrough (AR Mode)
1. Select `OVRCameraRig > TrackingSpace > CenterEyeAnchor` (Camera)
2. Add Component: **OVR Passthrough Layer**
3. Camera Settings:
   - **Clear Flags:** Solid Color
   - **Background:** Set Alpha to 0 (RGBA: 0, 0, 0, 0)
4. In **OVRManager** component on OVRCameraRig:
   - Enable **Passthrough Support** if available

### Step 4: Create Drawing System GameObject
1. **Create Empty GameObject** named `GameManager`
2. Add Component: **Drawing Manager**
3. Add Component: **VR Controller Input Provider** (or **Keyboard Input Provider** for testing)

### Step 5: Assign Assets to DrawingManager
1. Select `GameManager`
2. In **Drawing Manager** Inspector:
   - **Color Palette:** Drag `Assets/DrawingSystem/Resources/DefaultPalette.asset`
   - **Available Brushes:** Set size to 2
     - Element 0: `Assets/DrawingSystem/Resources/BrushData/WireBrush.asset`
     - Element 1: `Assets/DrawingSystem/Resources/BrushData/FlatBrush.asset`
   - **Min Distance Before New Point:** 0.01
   - **Max Points Per Line:** 500
   - **Max Total Lines:** 50
   - **Brush Size:** 0.01

---

## Verify Assets Exist

### Materials (should use Unlit/Color shader)
- ✅ `Assets/DrawingSystem/Materials/Brushes/WireMaterial.mat`
- ✅ `Assets/DrawingSystem/Materials/Brushes/FlatMaterial.mat`

**Material Settings:**
- Shader: **Unlit/Color** (NOT Standard - causes magenta/pink lines)
- Rendering Mode: Opaque

### Prefabs (LineRenderer GameObjects)
- ✅ `Assets/DrawingSystem/Prefabs/Brushes/WireBrushLine.prefab`
- ✅ `Assets/DrawingSystem/Prefabs/Brushes/FlatBrushLine.prefab`

**LineRenderer Settings:**
- Use World Space: ✓
- Width: 0.01
- Alignment: View (or TransformZ)
- Corner Vertices: 5
- End Cap Vertices: 5
- Material: Assigned from Materials folder

### ScriptableObjects
- ✅ `Assets/DrawingSystem/Resources/DefaultPalette.asset`
- ✅ `Assets/DrawingSystem/Resources/BrushData/WireBrush.asset`
- ✅ `Assets/DrawingSystem/Resources/BrushData/FlatBrush.asset`

---

## Final Scene Hierarchy

```
ARDrawingScene
├── OVRCameraRig
│   ├── OVRManager (component)
│   └── TrackingSpace
│       ├── LeftHandAnchor
│       ├── RightHandAnchor
│       └── CenterEyeAnchor (Camera + OVRPassthroughLayer)
└── GameManager
    ├── DrawingManager (component)
    ├── VRControllerInputProvider (component)
    └── LinePool (auto-added)
```

---

## Controls

### Meta Quest 3 Controllers
- **RIGHT Trigger:** Press and hold to draw
- **LEFT Y Button:** Cycle brush type (Wire ↔ Flat)
- **LEFT X Button:** Cycle color (8 colors)
- **RIGHT A Button:** Undo last line
- **LEFT Thumbstick Up/Down:** Adjust brush size (0.005m - 0.02m)

### Keyboard (Unity Editor Testing)
- **Spacebar:** Press and hold to draw
- **B:** Cycle brush type
- **C:** Cycle color
- **U or Z:** Undo last line
- **Up Arrow / +:** Increase brush size
- **Down Arrow / -:** Decrease brush size
- **Mouse Movement:** Simulates controller position

---

## Building to Quest 3

### Build Settings
1. **File > Build Settings**
2. **Add Open Scenes** (add your ARDrawingScene)
3. **Run Device:** Connect Quest 3 via USB or use Meta Quest Link
4. Click **Build and Run**

### Quest 3 Preparation
1. Enable **Developer Mode** in Meta Quest mobile app
2. Connect Quest 3 to PC via USB-C
3. Allow USB debugging on headset
4. In Unity, select your Quest 3 from **Run Device** dropdown

### Build Size & Performance Targets
- **Build Size:** ~200-300 MB
- **Frame Rate:** 72 FPS
- **Memory Usage:** <500 MB

---

## Testing Without Quest (Editor)

1. Replace `VRControllerInputProvider` with `KeyboardInputProvider` on GameManager
2. Press Play in Unity Editor
3. Use keyboard controls (see above)
4. Move mouse while holding Spacebar to simulate drawing

**Note:** Drawing position in Editor will use mouse position projected forward from camera, not true 3D tracking.

---

## Troubleshooting

### Lines appear pink/magenta
**Cause:** Using Standard shader  
**Fix:** Change material shader to **Unlit/Color**

### No drawing appears
**Check:**
1. OVRCameraRig exists with TrackingSpace child
2. DrawingManager has ColorPalette and BrushData assigned
3. Console shows "[DrawingManager] Initialized successfully!"
4. Console shows "[DrawingManager] StartDrawing() called" when trigger pressed

### Controller position incorrect
**Cause:** Not using TrackingSpace transform  
**Fix:** Ensure DrawingManager finds "OVRCameraRig/TrackingSpace" in Start()

### Passthrough not showing
**Check:**
1. OVRPassthroughLayer component on CenterEyeAnchor
2. Camera background alpha = 0
3. OVRManager has Passthrough Support enabled
4. Meta XR SDK version is not 83.0.1

### Buttons not working
**Verify:**
- LEFT Y = Button.Three (cycle brush)
- LEFT X = Button.Four (cycle color)
- RIGHT A = Button.One (undo)
- RIGHT Trigger = PrimaryIndexTrigger (draw)

---

## Color Palette (Default 8 Colors)
1. Red
2. Blue
3. Green
4. Yellow
5. Orange
6. Magenta
7. Cyan
8. White

---

## Success Criteria

When everything works:
1. Put on Quest 3 headset
2. See real world through passthrough (not black screen)
3. Hold RIGHT trigger
4. Move controller - colored line appears in mid-air
5. Release trigger - line stays in place
6. Press LEFT Y - next line uses different brush thickness
7. Press LEFT X - next line uses different color
8. Press RIGHT A - last line disappears

---

## Additional Notes

### Line Smoothing
- Lines are automatically smoothed using **Catmull-Rom spline interpolation**
- Smoothing applied when trigger is released
- Can be disabled per brush in BrushData asset

### Object Pooling
- 10 lines pre-instantiated per brush type
- Lines reused when undoing for performance
- Additional lines created dynamically if needed

### Performance Tips
- Keep line count under 50 (current limit)
- Keep points per line under 500 (current limit)
- Use Medium or Low quality settings
- Disable antialiasing if frame rate drops

---

## Files Created/Modified

### Core Scripts
- ✅ `DrawingManager.cs` - Main controller
- ✅ `IInputProvider.cs` - Input interface
- ✅ `VRControllerInputProvider.cs` - Quest controller input
- ✅ `KeyboardInputProvider.cs` - Editor testing input
- ✅ `LinePool.cs` - Object pooling system
- ✅ `LineSmoother.cs` - Catmull-Rom smoothing

### Data Scripts
- ✅ `BrushData.cs` - ScriptableObject for brush config
- ✅ `ColorData.cs` - Color data structure
- ✅ `ColorPalette.cs` - ScriptableObject for color palette

### Assets
- ✅ Materials (WireMaterial, FlatMaterial)
- ✅ Prefabs (WireBrushLine, FlatBrushLine)
- ✅ ScriptableObjects (DefaultPalette, WireBrush, FlatBrush)

---

## Next Steps

1. ✅ Test in Unity Editor with KeyboardInputProvider
2. ⬜ Build to Quest 3 and test with VRControllerInputProvider
3. ⬜ Fine-tune brush sizes and colors
4. ⬜ Test performance with max lines (50)
5. ⬜ Optional: Add save/load functionality
6. ⬜ Optional: Add brush texture support
7. ⬜ Optional: Add eraser functionality

---

## Support & Resources

- **Meta XR SDK Docs:** https://developer.oculus.com/documentation/unity/
- **Unity XR Docs:** https://docs.unity3d.com/Manual/XR.html
- **Quest 3 Developer Guide:** https://developer.oculus.com/quest3/

**Project Status:** ✅ Core implementation complete - Ready for testing!
