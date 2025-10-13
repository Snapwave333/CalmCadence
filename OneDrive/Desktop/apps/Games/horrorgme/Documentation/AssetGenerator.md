# Asset Generator Window

Run local Python/Node asset-generation scripts directly from the Unity Editor.

## Requirements
- Python 3.10+
- Node.js 18+

## Usage
- Open Tools > Assets > Asset Generator
- Set paths if needed (default assumes python/node on PATH)
- Click the desired buttons:
  - Generate Textures (Python) → generate_assets.py
  - Generate Audio (Python) → generate_audio.py
  - Generate Meshes (Python) → generate_meshes.py
  - Generate Assets (Node) → generate_assets.js
  - Run All → calls all of the above

After each run, the editor refreshes the AssetDatabase automatically.
