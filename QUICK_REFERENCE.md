# Spatial Ink Studio - Quick Reference Card

## 🎮 Quest 3 Controls

| Action | Button | Controller |
|--------|--------|-----------|
| **Draw** | Hold Trigger | RIGHT |
| **Cycle Brush** | Y Button | LEFT |
| **Cycle Color** | X Button | LEFT |
| **Undo** | A Button | RIGHT |
| **Increase Size** | Thumbstick Up | LEFT |
| **Decrease Size** | Thumbstick Down | LEFT |

## ⌨️ Keyboard Controls (Editor Testing)

| Action | Key |
|--------|-----|
| **Draw** | Hold Spacebar |
| **Cycle Brush** | B |
| **Cycle Color** | C |
| **Undo** | U or Z |
| **Increase Size** | Up Arrow or + |
| **Decrease Size** | Down Arrow or - |

## 🎨 Color Palette

1. **Red** (R,G,B: 255,0,0)
2. **Blue** (0,0,255)
3. **Green** (0,255,0)
4. **Yellow** (255,255,0)
5. **Orange** (255,128,0)
6. **Magenta** (255,0,255)
7. **Cyan** (0,255,255)
8. **White** (255,255,255)

Press LEFT X to cycle through colors.

## 🖌️ Brush Types

### Wire Brush
- **Style:** Thin, precise lines
- **Width:** ~0.005m (default)
- **Use Case:** Detailed drawings, outlines

### Flat Brush
- **Style:** Thick, painterly strokes
- **Width:** ~0.015m (default)
- **Use Case:** Bold drawings, filling areas

Press LEFT Y to cycle between brushes.

## 📏 Technical Limits

| Parameter | Value |
|-----------|-------|
| Max Lines | 50 |
| Max Points per Line | 500 |
| Min Point Distance | 0.01m (1cm) |
| Brush Size Range | 0.005m - 0.02m |
| Target FPS | 72 FPS |
| Max Memory | <500 MB |

## ✅ Scene Setup Checklist

- [ ] Unity 2022.3 LTS
- [ ] Meta XR SDK 68.0.0 or 71.0.0 installed
- [ ] Platform: Android
- [ ] Color Space: Linear
- [ ] Graphics API: OpenGLES3
- [ ] XR Plug-in: Oculus enabled
- [ ] OVRCameraRig in scene
- [ ] OVRPassthroughLayer on CenterEyeAnchor
- [ ] Camera background alpha = 0
- [ ] GameManager with DrawingManager
- [ ] VRControllerInputProvider (or KeyboardInputProvider)
- [ ] DefaultPalette assigned
- [ ] WireBrush and FlatBrush assigned

## 🏗️ Scene Hierarchy

```
ARDrawingScene
├── OVRCameraRig
│   ├── OVRManager
│   └── TrackingSpace
│       ├── LeftHandAnchor
│       ├── RightHandAnchor
│       └── CenterEyeAnchor
│           └── OVRPassthroughLayer
└── GameManager
    ├── DrawingManager
    └── VRControllerInputProvider
```

## 📦 Required Assets

### Materials (Unlit/Color shader)
- `DrawingSystem/Materials/Brushes/WireMaterial.mat`
- `DrawingSystem/Materials/Brushes/FlatMaterial.mat`

### Prefabs (LineRenderer)
- `DrawingSystem/Prefabs/Brushes/WireBrushLine.prefab`
- `DrawingSystem/Prefabs/Brushes/FlatBrushLine.prefab`

### ScriptableObjects
- `DrawingSystem/Resources/DefaultPalette.asset`
- `DrawingSystem/Resources/BrushData/WireBrush.asset`
- `DrawingSystem/Resources/BrushData/FlatBrush.asset`

## 🐛 Troubleshooting Quick Fixes

| Problem | Solution |
|---------|----------|
| Pink lines | Change shader to Unlit/Color |
| Lines at origin | Check TrackingSpace exists |
| No passthrough | Camera alpha must be 0 |
| Buttons don't work | Use Button.Three (Y), Button.Four (X) |
| SDK errors | Use version 68.0.0 or 71.0.0 |

## 📱 Build Settings Quick Config

```
File > Build Settings
├── Platform: Android ✓
├── Run Device: Quest 3
└── Player Settings
    ├── Minimum API: Android 10.0 (API 29)
    ├── Scripting Backend: IL2CPP
    ├── Color Space: Linear
    ├── Graphics APIs: OpenGLES3
    └── XR: Oculus ✓
```

## 🎯 Testing Steps

### Editor Test
1. Attach KeyboardInputProvider to GameManager
2. Press Play
3. Hold Spacebar + move mouse
4. Verify line appears

### Quest 3 Test
1. Build and Run to Quest 3
2. Put on headset
3. Verify passthrough shows real world
4. Hold RIGHT trigger + move controller
5. Verify line follows controller

## 📊 Performance Targets

- **Frame Rate:** 72 FPS minimum
- **Build Size:** 200-300 MB
- **Memory:** <500 MB
- **Line Smoothness:** Catmull-Rom applied
- **Input Latency:** <16ms

## 🚀 Ready to Build!

Your project is complete. Follow the setup checklist, assign assets in the Inspector, and build to Quest 3!

---

**Need Help?** See full documentation:
- `SETUP_GUIDE.md` - Complete setup instructions
- `TECHNICAL_IMPLEMENTATION.md` - Implementation details
