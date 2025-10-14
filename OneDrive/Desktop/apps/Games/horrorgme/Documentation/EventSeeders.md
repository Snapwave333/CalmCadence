# Event Seeders

Provides deterministic spawning/location selection using a fixed seed.

## API
- new EventSeeder(seed)
- PickWeighted(points): picks a Vector3 from a weighted list deterministically

## Usage
- Use a fixed seed during testing to get reproducible runs.
