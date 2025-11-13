# GameFoundations

> Modular Unity 6 foundation project featuring DI-driven services, async command pattern, and production-ready system scaffolding.

GameFoundations provides a curated core for building games that rely on dependency injection (VContainer), the new Input System, Addressables, and UniTask-based asynchronous flows. The included scene, audio, logging, and data services are designed to be extended, keeping gameplay code clean and testable.

## Quick Start

1. **Requirements**
   - Unity **6000.2.6f2** (Unity 6). See `ProjectSettings/ProjectVersion.txt` for the exact revision.
   - Recommended IDE: Visual Studio Code or Rider with C# support.
2. **Clone & Open**
   - Clone the repository, then add the project in Unity Hub and open it with the required Unity version.
3. **Install Unity Packages**
   - *Required*:
     - Input System (`com.unity.inputsystem`)
     - TextMesh Pro (Window → TextMeshPro → Import TMP Essential Resources)
     - VContainer (`https://github.com/hadashiA/VContainer.git` or OpenUPM `com.hadashiA.vcontainer`)
   - *Included*: UniTask is embedded under `Assets/Darkmatter/Core/Plugins/UniTask`. Use the embedded package or install via UPM (`com.cysharp.unitask`) and remove the embedded folder to avoid duplication.
   - *Optional*: Addressables (`com.unity.addressables`) for streamed content.
   - Use the editor menu **Darkmatter → Check Required Packages** to auto-verify and install missing dependencies (`Assets/Darkmatter/Editor/Validators/DarkmatterPackageValidator.cs`).
4. **Open the Demo Scene**
   - Load `Assets/Darkmatter/Core/Assets/Scenes/CoreScene.unity`.
5. **Press Play**
   - `CoreLifetimeScope` wires up services and boots `CoreInitiator`, which configures inputs, audio, and scene management.

## Feature Highlights

- **Dependency Injection with VContainer** – Central lifetime scope (`Assets/Darkmatter/Core/Scripts/Scopes/CoreLifetimeScope.cs`) registers services and entry points.
- **Async Command Pattern** – `CommandFactory` instantiates commands that resolve dependencies via `IObjectResolver`.
- **Scene Orchestration** – `SceneLoaderService` works with scene-specific initiators to manage load/start/exit flows asynchronously.
- **Audio Pipeline** – Channel-based audio playback with scriptable objects for clip catalogs.
- **Addressables Loader** – Cached Addressables resolution with cancellation support and automatic cleanup.
- **Data Persistence** – Encrypted `PlayerPrefs` saves for lightweight persistence needs.
- **Logging Facade** – Centralized `LogService` with timestamped Unity console output.
- **Ability & State Machine Primitives** – Reusable abstractions for gameplay loops and stateful systems.
- **Editor Package Validator** – Ensures required UPM dependencies are present on project open.

## Architecture Overview

- **Lifetime Scope** (`CoreLifetimeScope`): The single entry point for DI registrations. Services are either `Singleton` (global) or `Scoped`, and Unity components are exposed via `RegisterComponent`.
- **Entry Point** (`CoreInitiator`): Runs startup logic on `Start()`, enabling inputs, wiring audio catalogs, and seeding scene loader state. All long-running tasks use `CancellationTokenSource.CreateLinkedTokenSource(Application.exitCancellationToken)` to shut down cleanly.
- **Services Layer** (`Assets/Darkmatter/Core/Scripts/Services`): Houses composable services for audio, scenes, logging, addressables, persistence, commands, initiators, and finite state machines.
- **Gameplay Utilities**: `Abilities`, `Utils`, and `Extensions` provide reusable building blocks.

## Core Systems

### Command Factory

`CommandFactory` (see `Assets/Darkmatter/Core/Scripts/Services/CommandFactory`) produces command instances that self-resolve dependencies before execution.

```csharp
public sealed class MovePlayerCommand : BaseCommand, ICommandAsync
{
    private PlayerController _controller;

    public override void ResolveDependencies()
    {
        _controller = ObjectResolver.Resolve<PlayerController>();
    }

    public async UniTask Execute(CancellationTokenSource cts)
    {
        await _controller.MoveAsync(cts.Token);
    }
}

// Usage
var command = commandFactory.CreateCommandAsync<MovePlayerCommand>();
await command.Execute(cts);
```

- `BaseCommand` stores the injected `IObjectResolver`.
- Implement one of `ICommandVoid`, `ICommandWithResult<T>`, `ICommandAsync`, or `ICommandAsyncWithResult<T>`.
- `ResolveDependencies()` should pull all collaborators from the resolver; avoid heavy logic in constructors.

### Scene Loading & Initiators

- `SceneLoaderService` provides additive scene loading with duplication guards and cancellation support.
- Scenes are keyed by the `SceneType` enum and must match the actual scene asset names.
- Implement `ISceneInitiator` per scene to run load/start/exit entry points.
- Register/unregister initiators with `SceneInitiatorsService`, and pass view models via `IInitiatorEnterData`.

```csharp
public sealed class GameplayInitiator : MonoBehaviour, ISceneInitiator
{
    public SceneType SceneType => SceneType.GamePlay;

    public void Initialize() { /* VContainer initialization */ }

    public async Awaitable LoadEntryPoint(IInitiatorEnterData enterData, CancellationTokenSource cts)
    {
        // Warm up assets, addressables, etc.
        await UniTask.CompletedTask;
    }
}
```

### Audio Service

- `AudioService` maps audio clips to named channels (`Music`, `SFX`, `Voice`, `Ambient`, `Master`).
- Populate `CoreAudioClipsSO` (inherits from `AudioClipsSO`) with `AudioData` entries.
- Call `AddAudioClipsSO` during bootstrap (`CoreInitiator` already adds the default catalog).
- Adjust volume per channel via `SetSoundVolume`. Use `PlayAudioAsync` when you need to wait for completion.

### Addressables Loader

- `AddressablesLoaderService` caches `AsyncOperationHandle` instances by address.
- `LoadAsync<T>` enforces cancellation tokens and handles component extraction when loading prefabs.
- Call `Release(address)` for targeted cleanup or `ReleaseAll()` on shutdown.

### Data Persistence

- `PlayerPrefsPresistanceService` (note the intentional encryption layer) serializes any `Serializable` object to JSON, encrypts it via `EncryptionUtils`, and stores it in `PlayerPrefs`.
- Use `LoadData<T>` with a default fallback to keep deserialization safe.

### Logging

- `DarkmatterLogger` extends `LoggerBase` and injects itself into `LogService`.
- Call `LogService.LogTopic("message")` for automatic `[Class.Method]` prefixes and timestamps.

### Gameplay Helpers

- `BaseAbilitySystem` tracks a collection of `IAbility` instances, calling `Tick()` only on active abilities.
- `BaseStateMachine` provides push/pop/replace semantics for `IState` implementations.
- `AnimatorExtensions` expose `UniTask` helpers such as `PlayAndWaitAsync` and `CrossFadeAndWaitAsync`.
- `InputUtils.IsPointerOverUI()` abstracts current input backend (supports both legacy and new Input System).

## Folder Structure

```
Assets/Darkmatter/Core/
  Assets/         # Shared scenes, audio mixer, scriptable objects
  Plugins/        # Embedded third-party packages (UniTask, InputSystem bindings, TMP assets)
  Scripts/
    Abilities/    # Ability system interfaces and base classes
    Audio/        # Audio scriptable objects
    Extensions/   # Animator/String extensions
    Initiators/   # Project entry point (CoreInitiator)
    Scopes/       # VContainer lifetime scope definitions
    Services/     # Command, audio, logging, scene, addressables, persistence, etc.
    Utils/        # Encryption and input helpers
Editor/           # Package validator tooling
```

## Building

1. Open **File → Build Settings…** and add the desired scenes.
2. If using Addressables, open **Window → Asset Management → Addressables → Groups** and `Build → New Build → Default Build Script`.
3. Choose the target platform and click **Build**. Rebuild Addressables whenever content changes.

## Documentation

Additional guides live under `docs/wiki`. Suggested GitHub Wiki structure:

- [Home](docs/wiki/Home.md) – quick start, architecture links.
- [Command Pattern](docs/wiki/Command-Pattern.md) – command factory walkthrough.
- [Core Services](docs/wiki/Core-Services.md) – audio, scene, addressables, persistence, logging.
- [Gameplay Framework](docs/wiki/Gameplay-Framework.md) – abilities, state machines, initiators, utilities.

## Contributing

- Follow Unity C# conventions and keep services decoupled from MonoBehaviours when possible.
- Prefer injecting dependencies via VContainer over direct `FindObjectOfType` calls.
- Validate new packages with the editor validator or update `Assets/Darkmatter/Editor/Validators/package.json`.

## License

This repository currently does not include a license file. Add one before distributing builds or source publicly.
