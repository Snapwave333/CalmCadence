# Crash Reporting / Log Upload

Opt-in log capture and upload for testing.

## How it works
- Assets/Scripts/Telemetry/LogUploader.cs captures error/exception/assert logs.
- Writes rolling files to Application.persistentDataPath/Logs/yyyyMMdd.log.
- If opt-in enabled, attempts to POST logs to a configurable endpoint.

## Default endpoint
- http://localhost:5000/upload
- Run the mock server: python tools/log_uploader_mock_server.py

## Privacy
- Logs may contain file paths and error text. No PII is collected by default.
- Opt-in is off by default; users must enable it.

## Testing
1. Add LogUploader to a bootstrap GameObject.
2. Check uploadOptIn and keep default URL.
3. Run the mock server locally.
4. Trigger an error in play mode; observe server output and a new log file in Logs/.
