# Oakhaven's Point — Unity 6 Project

[![Unity Version](https://img.shields.io/badge/Unity-6000.2.6f1-blue.svg)](https://unity.com/releases/editor/whats-new/6000.2)
[![Platforms](https://img.shields.io/badge/Platforms-Windows%20%7C%20Android-lightgrey.svg)](https://unity.com)

A Unity 6 project for an atmospheric horror narrative prototype. The repo includes a simple interactive scene, an editor build utility, and a curated set of assets and documentation to help you build and distribute.

## Quick start

### Requirements
- Unity 6 Editor: 6000.2.6f1 (ProjectVersion.txt)
- Windows 10/11 or macOS for editing
- Optional: Python 3.10+ and Node 18+ for asset tooling

### Open and run
1. Open the folder in Unity Hub and use Editor 6000.2.6f1.
2. Open `Assets/Scenes/Game.unity`.
3. Press Play to run the prototype.

### Build
- Manual: File > Build Settings… > Standalone Windows 64 > Build.
- Editor menu utility: Tools > Build > Create Scene and Build
  - Runs `Assets/Editor/SimpleBuildScript.cs`, creates a minimal UI scene if needed and outputs to `Builds/Windows/HorrorGame.exe`.

## Project layout
```
Assets/
├─ Scripts/
│  └─ SimpleGame.cs               # Minimal interactive narrative flow
├─ Editor/
│  └─ SimpleBuildScript.cs        # One-click build utility
├─ Scenes/
│  └─ Game.unity                  # Main scene
├─ StreamingAssets/
│  └─ HorrorGame/...              # Packaged runtime assets
Documentation/
├─ Build-Instructions.md
├─ Architecture-Overview.md
├─ API-Reference.md
└─ User-Guide.md
Builds/
└─ Windows/…                      # Generated build artifacts
```

## Tooling (optional)
- Python utilities: `generate_assets.py`, `generate_audio.py`, `generate_meshes.py`.
- Node tooling: `generate_assets.js` (see `package.json`).
- Use these to regenerate or batch-process assets as described in the docs.

## Supported platforms
- Windows (primary). Android is optional via standard Unity platform switch.

## Documentation
- See `Documentation/Build-Instructions.md` for detailed build steps and CI pointers.
- See `Documentation/Architecture-Overview.md` and `TECHNICAL_ARCHITECTURE_OVERVIEW.md` for system design notes.
- See `Documentation/User-Guide.md` for gameplay and UX details.

## License
Proprietary. All rights reserved.

## Credits / Contact
- Project name: Oakhaven's Point
- Unity version: 6000.2.6f1
- For support, consult the docs above or the build logs in `BuildLogs/`.