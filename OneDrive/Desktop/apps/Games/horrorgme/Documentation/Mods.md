# Mods

Lightweight mod loader reads StreamingAssets/Mods/<modname>/manifest.json and exposes discovered mods.

## Add a mod
1. Create StreamingAssets/Mods/MyMod/manifest.json
2. Keep file small (<1MB) and valid JSON
3. Place any referenced assets alongside the manifest

## API
- ModLoader.DiscoverMods(): IEnumerable<string>
- ModLoader.LoadManifestJson(modName): string
