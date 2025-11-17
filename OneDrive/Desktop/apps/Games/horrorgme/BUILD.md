# Building Horror Game

## Prerequisites

- Unity 2021.3 LTS or newer (2022.3 LTS recommended)
- Windows 10/11, macOS 10.15+, or Ubuntu 20.04+ for building
- Git (for cloning the repository)

## Quick Start

1. **Clone the repository:**
   ```bash
   git clone https://github.com/Snapwave333/CalmCadence.git
   cd CalmCadence/OneDrive/Desktop/apps/Games/horrorgme
   ```

2. **Open in Unity:**
   - Launch Unity Hub
   - Click "Add" and select the `horrorgme` folder
   - Open the project with Unity 2021.3 LTS or newer

3. **Build the game:**
   - Go to `Build > Build Windows x64` in Unity menu
   - Or use `File > Build Settings` for custom builds

## Command Line Build

For CI/CD or automated builds:

```bash
# Windows
Unity.exe -quit -batchmode -projectPath "." -executeMethod BuildScript.BuildFromCommandLine -buildTarget windows

# Linux
Unity -quit -batchmode -projectPath "." -executeMethod BuildScript.BuildFromCommandLine -buildTarget linux

# macOS
Unity -quit -batchmode -projectPath "." -executeMethod BuildScript.BuildFromCommandLine -buildTarget macos
```

## Build Output

Builds are created in:
- **Windows:** `Builds/Windows/HorrorGame.exe`
- **Linux:** `Builds/Linux/HorrorGame`
- **macOS:** `Builds/macOS/HorrorGame.app`

## Required Unity Packages

The project uses these packages (will be auto-installed):
- Newtonsoft.Json (com.unity.nuget.newtonsoft-json)
- Test Framework (com.unity.test-framework)

## Creating a Release Build

1. Open Build Settings (`File > Build Settings`)
2. Select target platform (Windows, Linux, macOS)
3. Click "Player Settings" and verify:
   - Company Name: CalmCadence
   - Product Name: Horror Game
   - Version: 1.0.0
4. Enable "Development Build" for testing, disable for release
5. Click "Build" and select output folder

## Packaging for Distribution

After building, create a zip archive:

```bash
# Windows
cd Builds/Windows
zip -r HorrorGame-v1.0-Windows.zip .

# Linux
cd Builds/Linux
tar -czf HorrorGame-v1.0-Linux.tar.gz *

# macOS
cd Builds/macOS
zip -r HorrorGame-v1.0-macOS.zip HorrorGame.app
```

## Known Issues

- First build may take longer due to shader compilation
- macOS builds require code signing for distribution
- WebGL builds not yet supported (planned for v2.0)

## Troubleshooting

**Build fails with missing scenes:**
- Ensure `Assets/Scenes/Game.unity` exists
- Add scene to Build Settings (`File > Build Settings > Add Open Scenes`)

**Missing dependencies:**
- Go to `Window > Package Manager`
- Install any missing packages

**Script compilation errors:**
- Check Unity version compatibility
- Ensure .NET Standard 2.1 is set in Player Settings
