# GameFoundations Wiki

Welcome to the GameFoundations knowledge base. These pages expand on the concepts introduced in the repository `README.md` and explain how each subsystem works together.

## Table of Contents

- [Quick Start](#quick-start)
- [Project Pillars](#project-pillars)
- [Architecture Map](#architecture-map)
- [Deep Dives](#deep-dives)

## Quick Start

1. Install **Unity 6000.2.6f2** (Unity 6) and open the project through Unity Hub.
2. Run **Darkmatter → Check Required Packages** to verify VContainer, Addressables, and the Input System.
3. Import TextMesh Pro essentials (`Window → TextMeshPro → Import TMP Essential Resources`).
4. Load `Assets/Darkmatter/Core/Assets/Scenes/CoreScene.unity` and press Play.
5. Review `Assets/Darkmatter/Core/Scripts/Scopes/CoreLifetimeScope.cs` to learn how services are registered.

## Project Pillars

- **Dependency Injection** – VContainer supplies all services and entry points. MonoBehaviours should request dependencies via constructors or `[Inject]` methods.
- **Asynchronous Workflows** – UniTask powers async scene loading, command execution, and animation helpers.
- **Service-Oriented Architecture** – Reusable services (audio, logging, addressables, persistence) keep gameplay code lean.
- **Extensibility** – Command interfaces, ability/state abstractions, and initiator hooks make it straight-forward to add new game systems.

## Architecture Map

- `CoreLifetimeScope` wires up services and DI entry points.
- `CoreInitiator` orchestrates initial setup (inputs, audio catalog, scene state).
- Services live under `Assets/Darkmatter/Core/Scripts/Services` and are grouped by domain.
- Utilities and gameplay helpers (abilities, extensions, state machines) sit alongside the services layer.
- Editor tooling under `Assets/Darkmatter/Editor` enforces project dependencies.

## Deep Dives

- [Command Pattern](Command-Pattern.md) – Building and executing asynchronous commands.
- [Core Services](Core-Services.md) – Audio, scene management, addressables, persistence, logging.
- [Gameplay Framework](Gameplay-Framework.md) – Ability system, state machines, initiators, utility helpers.
