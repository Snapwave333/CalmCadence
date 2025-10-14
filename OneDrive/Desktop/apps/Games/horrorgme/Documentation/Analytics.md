# Analytics (Opt-in)

Records non-identifying events to local telemetry files when enabled.

## Usage
- Add AnalyticsManager to a bootstrap object; check optIn to enable.
- Record events: AnalyticsManager.Record("SceneLoaded", fields)

## Privacy
- Local only by default. Pair with LogUploader to upload if desired.
