# Core Services

This page describes the service layer under `Assets/Darkmatter/Core/Scripts/Services`. Each service is registered in `CoreLifetimeScope` or can be registered in a custom `LifetimeScope` depending on project needs.

## Audio Service

Source: `Services/AudioService/AudioService.cs`

- Maintains dedicated `AudioSource` instances per `AudioChannelType` (Master, Music, SFX, Voice, Ambient).
- `Initialize()` maps enum channels to serialized `AudioSource` references.
- `AddAudioClipsSO` / `RemoveAudioClipsSO` attach scriptable object catalogs (`AudioClipsSO`) that hold `AudioData` entries.
- `PlayAudio` runs `PlayOneShot` or looping playback; `PlayAudioAsync` awaits clip length via UniTask.
- `SetSoundVolume` clamps volume per channel; `StopAudio` halts a specific clip; `StopAllAudio` clears every channel.

Usage tips:

- Keep `CoreAudioClipsSO` (`Assets/Darkmatter/Core/Scripts/Audio/CoreAudioClipsSO.cs`) as the baseline catalog and extend it with game-specific SOs.
- Inject `IAudioService` into gameplay systems. Do not manipulate serialized audio sources directly.

## Scene Loader Service

Source: `Services/SceneService/SceneLoaderService.cs`

- Supports additive scene loading keyed by `SceneType` enum. The enum value must match the actual scene file name.
- Prevents duplicate loads with `_loadedScenes` and `_loadingScenes` hash sets.
- `TryLoadScene<TEnterData>` triggers:
  1. Validation and additive load.
  2. Scene-specific initiator load/start entry points through `ISceneInitiatorsService`.
- `TryUnloadScene` runs the scene's exit point before unloading.

Recommendations:

- Implement `ISceneInitiator` per scene to execute load/start/exit logic. Register initiators in the DI container.
- Always pass a shared `CancellationTokenSource` so loads/unloads can be aborted during shutdown or scene transitions.

## Scene Initiator Service

Source: `Services/InitiatorService/Scenes/SceneInitiatorsService.cs`

- Tracks `ISceneInitiator` instances by `SceneType`.
- Invokes load, start, and exit entry points when commanded by the scene loader.
- Use `IInitiatorEnterData` (marker interface) to forward payloads between scenes without tight coupling.

## Addressables Loader Service

Source: `Services/AddressablesService/AddressablesLoaderService.cs`

- Caches `AsyncOperationHandle` instances to avoid reloading the same address repeatedly.
- Returns components directly when `LoadAsync<T>` is called for `MonoBehaviour`-derived types and the Addressable is a prefab.
- Provides `Release(address)` and `ReleaseAll()` helpers for cleanup.
- Logs errors with the `LogService` if loads fail due to missing assets or incorrect types.

Workflow:

```csharp
var enemy = await _addressables.LoadAsync<GameObject>("Enemies/Boss", cts);
Instantiate(enemy, spawnPoint, Quaternion.identity);
// Later...
_addressables.Release("Enemies/Boss");
```

## Data Persistence Service

Source: `Services/DataPresistanceService/PlayerPrefsPresistanceService.cs`

- Serializes objects with `JsonUtility`, encrypts payloads through `EncryptionUtils`, and stores them in `PlayerPrefs`.
- `SaveData<T>(key, data)` persists the encrypted JSON string.
- `LoadData<T>(key, defaultValue)` decrypts and deserializes, returning a fallback if the key is absent.
- `EncryptionUtils` currently uses a static key and AES with an IV prepended to the ciphertext. Rotate the key or plug in a new implementation for production.

## Logging Service

Source: `Services/LoggingService`

- `LogService` is a static façade used throughout the project.
- `LoggerBase` enforces registration with `LogService` via constructor call.
- `DarkmatterLogger` adds timestamps and a `[Darkmatter]` tag to every log.
- Methods:
  - `Log(string)` – standard messages.
  - `LogWarning(string)` – warnings.
  - `LogError(string)` – errors.
  - `LogException(Exception)` – exception summaries.
  - `LogTopic(string message)` – formats output as `Class.Method:: message`.

To route logs elsewhere (e.g., file logging or analytics), implement `ILogger` and register the new logger with VContainer.
