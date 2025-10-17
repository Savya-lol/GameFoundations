# GameFoundations (Unity)

A modular Unity project with async utilities (UniTask), input bindings, UI/text rendering (TextMesh Pro), and service patterns (Command Factory, Logging).

- Unity version: 6000.2.6f2 (see [ProjectSettings/ProjectVersion.txt](ProjectSettings/ProjectVersion.txt))
- IDE: Visual Studio Code, Unity editor

## Requirements & Dependencies

- Unity: 6000.2.6f2 (Unity 6)
- Packages:
  - Input System (com.unity.inputsystem) — required
  - TextMesh Pro — required (import TMP Essentials on first use)
  - UniTask — included (embedded under Assets/Darkmatter/Core/Plugins/UniTask)
    - Optional: install via UPM instead of embedded: com.cysharp.unitask
  - VContainer — optional (recommended for DI)
    - Install via Git URL: https://github.com/hadashiA/VContainer.git
    - Or via OpenUPM: com.hadashiA.vcontainer
  - Addressables (com.unity.addressables) — optional (recommended for content management)
    - Install via Package Manager (Unity Registry → “Addressables”)
    - Opens via Window → Asset Management → Addressables → Groups
- IDE: Visual Studio Code or Unity editor
- Notes:
  - If you switch to UPM UniTask, remove the embedded Assets/Darkmatter/Core/Plugins/UniTask to avoid duplicates.
  - Enable the Input System in Project Settings when prompted (or set Active Input Handling to Both/New).
  - Addressables: choose an Editor Play Mode Script in the Addressables Groups window (Use Asset Database for iteration).

## Project structure

- Assets/
  - Darkmatter/Core
    - Scripts
      - Extensions
        - [AnimatorExtensions.cs](Assets/Darkmatter/Core/Scripts/Extensions/AnimatorExtensions.cs)
      - Services
        - CommandFactory
          - [CommandFactory.cs](Assets/Darkmatter/Core/Scripts/Services/CommandFactory/CommandFactory.cs)
          - [Interfaces/ICommandFactory.cs](Assets/Darkmatter/Core/Scripts/Services/CommandFactory/Interfaces/ICommandFactory.cs)
        - LoggingService
          - [DarkmatterLogger.cs](Assets/Darkmatter/Core/Scripts/Services/LoggingService/DarkmatterLogger.cs)
        - AudioService
          - Enums
            - [AudioPlayMode.cs](Assets/Darkmatter/Core/Scripts/Services/AudioService/Enums/AudioPlayMode.cs)
      - Utils
        - [EncryptionUtils.cs](Assets/Darkmatter/Core/Scripts/Utils/EncryptionUtils.cs)
    - Plugins
      - UniTask
        - Runtime
          - [UniTask.cs](Assets/Darkmatter/Core/Plugins/UniTask/Runtime/UniTask.cs)
          - [Channel.cs](Assets/Darkmatter/Core/Plugins/UniTask/Runtime/Channel.cs)
      - InputSystem
        - [GameInputs.cs](Assets/Darkmatter/Core/Plugins/InputSystem/GameInputs.cs)
      - TextMesh Pro
        - Shaders, Fonts, Sprites
        - Fonts license: [LiberationSans - OFL.txt](Assets/Darkmatter/Core/Plugins/TextMesh%20Pro/Fonts/LiberationSans%20-%20OFL.txt)

## Getting started

1. Open the folder in Unity Hub with Unity 6000.2.6f2.
2. Packages:
   - Input System: Window → Package Manager → Unity Registry → “Input System” → Install.
     - Project Settings → Player → Active Input Handling → “Input System Package (New)” or “Both” → restart editor when prompted.
   - TextMesh Pro: Window → TextMeshPro → Import TMP Essential Resources.
   - VContainer (optional for DI): Window → Package Manager → + → Add package from git URL… → https://github.com/hadashiA/VContainer.git
   - Addressables (optional but recommended): Window → Package Manager → Unity Registry → “Addressables” → Install.
     - Window → Asset Management → Addressables → Groups → Play Mode Script: “Use Asset Database (fast)” for iteration.
     - Mark assets as Addressable (Inspector → Addressable checkbox) and assign addresses/labels.
3. Open the main scene (create or select from Assets).
4. Enter Play Mode.

Build:
- File → Build Settings… → select target platform → Build.
- If using Addressables for content:
  - Window → Asset Management → Addressables → Groups → Build → New Build → Default Build Script (rebuild when groups change).
  - For testing bundles in Editor: Play Mode Script → “Use Existing Build (requires built groups)”.

## Key components

- Animator extensions:
  - [`AnimatorExtensions`](Assets/Darkmatter/Core/Scripts/Extensions/AnimatorExtensions.cs)
    - [`AnimatorExtensions.PlayAndWaitAsync`](Assets/Darkmatter/Core/Scripts/Extensions/AnimatorExtensions.cs)
    - [`AnimatorExtensions.CrossFadeAndWaitAsync`](Assets/Darkmatter/Core/Scripts/Extensions/AnimatorExtensions.cs)
    - [`AnimatorExtensions.WaitCurrentStateAsync`](Assets/Darkmatter/Core/Scripts/Extensions/AnimatorExtensions.cs)

- Command Factory:
  - [`Darkmatter.Core.Services.CommandFactory.CommandFactory`](Assets/Darkmatter/Core/Scripts/Services/CommandFactory/CommandFactory.cs)
  - [`Darkmatter.Core.Services.CommandFactory.Interfaces.ICommandFactory`](Assets/Darkmatter/Core/Scripts/Services/CommandFactory/Interfaces/ICommandFactory.cs)

- Input:
  - [`GameInputs`](Assets/Darkmatter/Core/Plugins/InputSystem/GameInputs.cs)

- Logging:
  - [`Darkmatter.Core.Services.LoggingService.DarkmatterLogger`](Assets/Darkmatter/Core/Scripts/Services/LoggingService/DarkmatterLogger.cs)

- Utilities:
  - [`Darkmatter.Core.Utils.EncryptionUtils`](Assets/Darkmatter/Core/Scripts/Utils/EncryptionUtils.cs)

- Async (UniTask):
  - [`Cysharp.Threading.Tasks.UniTask`](Assets/Darkmatter/Core/Plugins/UniTask/Runtime/UniTask.cs)
  - [`Cysharp.Threading.Tasks.Channel`](Assets/Darkmatter/Core/Plugins/UniTask/Runtime/Channel.cs)

## Usage examples

Animator async helpers:
```csharp
using UnityEngine;
using Cysharp.Threading.Tasks;

public class AnimExample : MonoBehaviour
{
    [SerializeField] Animator anim;
    static readonly int Attack = Animator.StringToHash("Attack");

    async UniTaskVoid Start()
    {
        // Play by state hash and await completion
        await anim.PlayAndWaitAsync(Attack);

        // Crossfade to state and await completion
        await anim.CrossFadeAndWaitAsync(Attack, transitionDuration: 0.2f);

        // Await current state's completion
        await anim.WaitCurrentStateAsync();
    }
}
```

Command Factory (DI via VContainer):
```csharp
using VContainer;
using Darkmatter.Core.Services.CommandFactory;
using Darkmatter.Core.Services.CommandFactory.Interfaces;

public class Bootstrap
{
    readonly IObjectResolver resolver;

    public Bootstrap(IObjectResolver resolver)
    {
        this.resolver = resolver;
    }

    public void CreateCommands()
    {
        var factory = new Darkmatter.Core.Services.CommandFactory.CommandFactory(resolver);
        // Examples (define your command types implementing the respective interfaces):
        // var cmd = factory.CreateCommandVoid<MyCommand>();
        // var asyncCmd = factory.CreateCommandAsync<MyAsyncCommand>();
    }
}
```

Input actions:
- Use [`GameInputs`](Assets/Darkmatter/Core/Plugins/InputSystem/GameInputs.cs) to wire player/UI actions generated by the Input System.

Logging:
- Use [`Darkmatter.Core.Services.LoggingService.DarkmatterLogger`](Assets/Darkmatter/Core/Scripts/Services/LoggingService/DarkmatterLogger.cs) for timestamped logs.

Audio:
- Playback mode enum: [`Darkmatter.Core.Services.AudioService.AudioPlayMode`](Assets/Darkmatter/Core/Scripts/Services/AudioService/Enums/AudioPlayMode.cs).

## UniTask details

- Location: Assets/Darkmatter/Core/Plugins/UniTask (embedded).
- Usage:
  - using Cysharp.Threading.Tasks;
  - Prefer UniTask/UniTaskVoid over Task in gameplay code for performance.
- If you prefer UPM:
  1) Delete Assets/Darkmatter/Core/Plugins/UniTask
  2) Install com.cysharp.unitask from Package Manager (or Git URL https://github.com/Cysharp/UniTask.git)
- Common patterns:
  - Use async UniTask methods for gameplay flows.
  - Use CancellationToken (e.g., this.GetCancellationTokenOnDestroy()) to cancel on object destruction.

## VContainer details (optional)

- Install via Git URL (recommended): https://github.com/hadashiA/VContainer.git
- Basic registration:
  ```csharp
  using VContainer;
  using VContainer.Unity;
  using Darkmatter.Core.Services.CommandFactory;

  public class AppScope : LifetimeScope
  {
      protected override void Configure(IContainerBuilder builder)
      {
          // Register services
          builder.Register<Darkmatter.Core.Services.LoggingService.DarkmatterLogger>(Lifetime.Singleton);
          builder.Register<CommandFactory>(Lifetime.Singleton);
          // ...register your commands and other services...
      }
  }
  ```
- Resolving:
  - Inject IObjectResolver into entry points (MonoBehaviours via LifetimeScope) or constructor-injected classes.
  - CommandFactory expects an IObjectResolver; VContainer provides this automatically when resolved/constructed.

## Addressables details (optional)

- Install: Package Manager → Addressables. Open Groups window (Window → Asset Management → Addressables → Groups).
- Setup:
  - Mark assets Addressable via Inspector or right-click in Project → Addressables → “Make Addressable”.
  - Use labels for bulk load (e.g., “ui”, “environment”).
  - Profiles: switch Build/Load paths per target (Groups window → Profiles). Default local paths are fine for most use.
  - Editor iteration: Play Mode Script → “Use Asset Database (fast)”; bundle testing: “Use Existing Build”.
- Build for content:
  - Groups window → Build → New Build → Default Build Script.
  - Rebuild groups after adding/removing Addressable assets or changing groups/labels.
- Usage examples (with UniTask):
  ```csharp
  using UnityEngine;
  using UnityEngine.AddressableAssets;
  using UnityEngine.ResourceManagement.AsyncOperations;
  using Cysharp.Threading.Tasks;

  // Load a prefab by address and instantiate
  public static class AddressablesExamples
  {
      public static async UniTask<GameObject> LoadPrefabAsync(string address)
      {
          var handle = Addressables.LoadAssetAsync<GameObject>(address);
          GameObject prefab = await UniTask.FromTask(handle.Task);
          return prefab; // Remember to Addressables.Release(handle) when prefab no longer needed
      }

      public static async UniTask<GameObject> InstantiateAsync(string address, Vector3 pos, Quaternion rot, Transform parent = null)
      {
          var instHandle = Addressables.InstantiateAsync(address, pos, rot, parent);
          var instance = await UniTask.FromTask(instHandle.Task);
          return instance; // Later: Addressables.ReleaseInstance(instance);
      }

      public static async UniTask PreloadByLabelAsync(string label)
      {
          var loadHandle = Addressables.LoadAssetsAsync<Object>(label, _ => { /* optional per-asset callback */ });
          await UniTask.FromTask(loadHandle.Task);
          // Keep handle or track assets if you need to release later: Addressables.Release(loadHandle);
      }

      public static void ReleaseAssetHandle<T>(AsyncOperationHandle<T> handle)
      {
          if (handle.IsValid()) Addressables.Release(handle);
      }

      public static void ReleaseInstance(GameObject instance)
      {
          if (instance != null) Addressables.ReleaseInstance(instance);
      }
  }
  ```
- Tips:
  - Keep and release the specific handles you created (asset handles vs instance handles).
  - Avoid mixing Resources and Addressables for the same asset.
  - For remote content, configure Build/Load paths in Profiles and host built bundles (ServerData).

## Input System setup

- Generate input actions with the Input System (see Assets/Darkmatter/Core/Plugins/InputSystem/GameInputs.cs).
- Ensure Project Settings → Player → Active Input Handling includes the Input System.
- Bind actions in your components by creating and enabling a GameInputs instance or through PlayerInput.

## Notes

- This project includes third-party assets:
  - UniTask by Cysharp (embedded under Assets/Darkmatter/Core/Plugins/UniTask)
  - Unity Input System generated bindings
  - TextMesh Pro shaders and fonts (see included licenses)
  - Addressables by Unity (package com.unity.addressables)
