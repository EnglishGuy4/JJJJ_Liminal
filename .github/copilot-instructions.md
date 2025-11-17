# JJJJ_Liminal Copilot Instructions

## Project Overview
**Sentinel** is a VR game built in Unity 2019.1.10f1 featuring a cannon-based defense game where players survive waves of asteroids and enemies. The codebase spans multiple subsystems including wave management, enemy AI, score tracking, and VR hardware integration (Oculus VR, SteamVR).

## Architecture Patterns

### Manager-Based Architecture
The project uses centralized Manager classes for game systems:
- **`SpawnerManager`** (`Assets/Cannon Roar/Scripts/Managers/`) - Orchestrates wave spawning with two modes (Timed/Endless)
- **`GameManager`** - Tracks score, shield health, enemy lists, fade transitions
- **`PoolManager`** - Implements object pooling with singleton static reference (`PoolManager.current`)
- **`TargetManager`** - Manages target/goal tracking

Managers use **`FindObjectOfType<T>()` and name-based lookups** (e.g., `GameObject.Find("GameManager")`) rather than DI containers. Register manager references at scene start in `Awake()` methods.

### Singleton Access Pattern
- `PoolManager` exposes `public static PoolManager current` for global access
- Used by `SpawnerManager.SpawnEnemy()` to pool enemies by name lookup
- **Defensive programming**: Always null-check before using (`if (PoolManager.current != null)`)

### Wave System (Timed vs. Endless)
`SpawnerManager` supports two gameplay modes via `WaveMode` enum:

**Timed Mode**: Predefined waves in a list with configurable:
- `waveTime` - Duration each wave runs
- `spawnRate` - Seconds between enemy spawns
- `maxEnemies` - Concurrent enemy cap
- `enemyPrefabs` - Random selection pool per wave
- `extraDelayAfterWave` - Custom intermission delay
- `waveDialogueObject` - Dialogue to activate when wave ends

**Endless Mode**: Difficulty scaling each wave:
- Spawn rate decreases by `spawnRateDecrease` (config: 0.1s)
- `maxEnemies` increases by `maxEnemiesIncrease` (config: 2 enemies)
- Clamped by `minSpawnRate` and `maxEnemiesCap`

### Object Pooling Strategy
`PoolManager` initializes pools at start with:
- `collectionOfObjectsToBePooled[]` - Prefab array
- `pooledAmountForEachObject[]` - Per-prefab pool size
- `willGrow = true` - Auto-creates objects if pool exhausted

`SpawnerManager.SpawnEnemy()` retrieves via:
```csharp
string sanitized = chosenPrefab.name.Replace("(Clone)", "").Trim();
enemy = PoolManager.current.GetPooledObject(sanitized);
```
**Note**: Sanitizes "(Clone)" suffix due to Unity instantiation behavior. Falls back to `Instantiate()` if pool returns null.

### Component Lookup Pattern
Spawned enemies initialize components via `GetComponent<T>()` with defensive null checks:
```csharp
EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
if (enemyHealth != null) {
    enemyHealth.enemySpawnerScript = this;
}
```
This pattern prevents crashes if a prefab is missing required components—logs a warning instead.

### Tag-Based Organization
Critical tags used:
- **"SpawnPoint"** - Child transforms of SpawnerManager marked for enemy spawning
- **"Waypoint"** - Path nodes enemies follow via `waypoints` list
- **"Border"** - Collision boundaries (used in `EnemyShip.cs`)

Tags auto-populate in `SpawnerManager.SetUpChildObjects()` during `Awake()` if empty.

## Key Data Structures

### Wave (Serializable Class)
Defines enemy spawn behavior within a time window:
```csharp
[System.Serializable]
public class Wave {
    public float waveTime = 30f;
    public float spawnRate = 2f;
    public int maxEnemies = 10;
    public List<GameObject> enemyPrefabs;  // Random selection
    public float extraDelayAfterWave = 0f;
    public GameObject waveDialogueObject;  // Activates on wave end
}
```

### Game State Tracking
`SpawnerManager` tracks:
- `currentWaveIndex` - Zero-based index (1-based in UI via `+1`)
- `waveTimer`, `spawnTimer`, `intermissionTimer` - Frame-accurate timing
- `inIntermission`, `allWavesComplete`, `wavesStarted` - State flags
- `enemiesFromThisSpawnerList` - Owned enemies (synced to `GameManager.enemies`)

## Integration Points

### Cannon Powerups (Wave-Based Triggers)
SpawnerManager grants powerups to a `Cannon` object during specific wave ranges:
- **ScatterShot**: Waves `scatterShotStartWave` to `scatterShotEndWave-1`
- **FullAuto**: Waves `fullAutoStartWave` to `fullAutoEndWave-1`
- Call `cannon.ActivateScatterShot()` / `DeactivateScatterShot()` etc.
- Triggers `TriggerPowerUpFeedback()` for VFX/audio

### Event System
- `BeginSpawningEvent` - Invoked when spawning starts via `BeginSpawning()`
- `OnAllWavesComplete` - Invoked when final wave ends
- Listeners subscribe in inspector or code

### End-of-Game Sequence
Final wave triggers `EndAllWaves()` → `StartEndDialogueWithDelay()` → `HandleEndDialogueSequence()`:
1. Activates `endDialogue` GameObject
2. Waits `endDialogueDuration` (default 5s)
3. Calls `GameManager.FadeAndLoadResults()` to transition to Results scene

### UI Text Updates
- `waveText` (TextMeshProUGUI) - Displays "WAVE: #", countdowns, intermission status
- `waveTimerText` - Shows MM:SS countdown (clamped to 00:00)
- Updated every frame during active waves

### Audio Integration
SpawnerManager triggers:
- `countdownSFX` - 6 seconds before wave starts
- `waveStartSFX` / `waveEndSFX` - Transition markers
- `allWavesCompleteSFX` - Game completion sound
- `musicSource` - Looping wave music (plays on `BeginSpawning()`, stops on `EndAllWaves()`)
- `powerUpSFX` + `powerUpVFX` - Powerup feedback animation

## VR & Input Integration
- Project uses **Liminal SDK** (`using Liminal.Core.Fader`, `Liminal.Experience`)
- References Oculus VR and SteamVR subsystems via `.csproj` files
- Fade transitions use `Liminal.Core.Fader` for scene transitions
- Scene management via `SceneManager.LoadScene()`

## Development Conventions

### Debug Logging
Many managers expose `debugSpawning` bool in inspector. When true:
- Logs detailed spawn decisions: prefab selection, pool lookups, missing components
- Example: `Debug.LogFormat("[SpawnerManager] Spawned '{0}' at spawnIndex {1}. PoolUsed={2}", ...)`
- Use consistent `[SpawnerManager]` prefixes for easy filtering

### Coroutine Patterns
Heavy use of `StartCoroutine()` for:
- Timed delays: `yield return new WaitForSeconds(delay)`
- Animation loops: `while (timer < duration)` with `Time.deltaTime`
- UI animations and sound playback sequences

Powerup VFX animation pattern (in `AnimatePowerUpVFX()`):
```csharp
// Scale up → Hold → Scale down
// Each phase uses Lerp with frame-based timer
```

### Temporal Precision
Wave timing uses frame-accumulation: `timer += Time.deltaTime` with comparison against target duration. Critical for intermission countdowns and powerup animations (0.15s scale transition).

### Scene Organization
Scenes likely contain:
- Root GameManager GameObject
- SpawnerManager with child transforms tagged "SpawnPoint" and "Waypoint"
- Cannon prefab
- UI Canvas with TextMeshPro elements
- PoolManager in scene or referenced via prefab

## Common Tasks

### Adding a New Wave Type
1. Expand `WaveMode` enum if new mode needed
2. Create/assign `Wave` object in inspector (or in code)
3. Add `enemyPrefabs` to wave's list
4. Set `waveTime`, `spawnRate`, `maxEnemies`
5. Optional: Assign `waveDialogueObject` and `extraDelayAfterWave`
6. Tag spawn/waypoint transforms if adding new spawner

### Adding Enemy Variants
1. Create enemy prefab with `EnemyHealth`, `EnemyMovement`, `EnemyShoot`, `NavMeshAgent` components
2. Ensure prefab name (without "(Clone)") matches pool lookup
3. Register in `PoolManager` inspector or fallback to `Instantiate()`
4. Add to wave's `enemyPrefabs` list for random selection

### Extending Powerup Feedback
Modify `TriggerPowerUpFeedback()` and `AnimatePowerUpVFX()`:
- Adjust `startScale`, `popScale`, `scaleTransitionTime`, `holdTimeAtPeak`
- Assign new `powerUpVFX` prefab or `powerUpSFX` clip
- Coroutine-based animation uses `Lerp()` and `Time.deltaTime` for frame-rate independence

### Debugging Wave Progression
Enable `SpawnerManager.debugSpawning = true` to log:
- Spawn attempts and pool/instantiate decisions
- Enemy health, movement, and shooting component setup
- Wave timer state and transition events

## File Organization
- **Managers**: `Assets/Cannon Roar/Scripts/Managers/` (GameManager, SpawnerManager, PoolManager, TargetManager)
- **Enemy Logic**: `Assets/Cannon Roar/Scripts/Enemies/` (movement, health, shooting)
- **Cannon**: `Assets/Cannon Roar/Scripts/Cannon/` (LauncherController, powerup logic)
- **Generic Scripts**: `Assets/Scripts/` (UI, scene transitions, shared utilities)
- **Third-Party**: `Assets/Third Party/` (VolumetricLines, Cartoon FX, QuickOutline, SteamVR, Oculus.VR)

## Known Issues & Workarounds
1. **Pool Name Lookup Fragility**: "(Clone)" suffix requires sanitization. Always use `.Replace("(Clone)", "").Trim()` on prefab names.
2. **Component Dependencies**: Spawned enemies crash if missing `EnemyHealth` or `EnemyMovement`. Defensive checks prevent this—always include null checks after `GetComponent<T>()`.
3. **Scene Finding**: `GameObject.Find("GameManager")` is slow and fragile. Prefer passing references via inspector or constructor injection when possible.
4. **Intermission Timing**: `extraDelayAfterWave` per-wave delay can cause unexpected pauses. Document wave delays in wave definitions.

## Testing & Validation
- Open Cannon Roar scene and set `SpawnerManager.waveMode` to test (Timed vs. Endless)
- Assign test waves in inspector with dummy enemy prefabs
- Monitor `waveText` and `waveTimerText` UI for state transitions
- Check `GameManager.enemies` list to verify spawn tracking
- Use `debugSpawning = true` to trace pool/instantiate decisions
