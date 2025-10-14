# CI

Runs Unity tests and builds a Windows player on push/PR.

## Requirements
- Set a Unity Pro/Plus license secret UNITY_LICENSE in repo secrets.

## What it does
- Uses game-ci actions to run EditMode/PlayMode tests.
- Builds StandaloneWindows64 to Builds/Windows/ci-build.exe and uploads as artifact.

## Local
- Run tests from Unity Test Runner or CLI with -runTests.
- Build locally via File > Build Settings.
