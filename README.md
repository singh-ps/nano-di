# NanoDI

[![Unity Version](https://img.shields.io/badge/Unity-2022.3+-57b9d3.svg?style=flat&logo=unity)](https://unity3d.com/get-unity/download)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![UPM Package](https://img.shields.io/badge/UPM-com.singh--ps.nanodi-blue)](https://github.com/singh-ps/nano-di)

A minimal, blazing-fast dependency injection framework for Unity.  
**Field injection only.** **No service locator.** **Factory-first creation.**

> **Production-ready** • **Zero allocations** • **IL2CPP safe** • **UPM compatible**

---

## Features

- **Field injection only** — Clean, simple approach with no property/method/constructor injection complexity
- **Non‑recursive container** — The container never auto-constructs dependency graphs
- **Fail‑fast validation** — Duplicate binds and missing dependencies throw immediately with clear error messages
- **Factory‑first creation** — Use `Factory<T>` for plain objects and `PrefabFactory<T>` for Unity prefabs
- **Deterministic lifecycle** — `BindNew<T>()` guarantees `Initialize()` is called after injection
- **High performance** — Per‑type reflection cache, compiled injectors, and zero‑allocation prefab scanning

---

## Design Principles

- **Simple by default**: Keep DI lean; push complexity into factories and composition
- **Explicit composition**: All bindings declared in `CompositionRoot` assets executed by `DIContext` before scene load
- **No service locator**: You don't "get" instances from the container; you **bind** and **inject** via factories
- **Strict wiring**: If something isn't bound, that's a bug—fail early and clearly

---

## How It Works

### Container

**Public APIs:**

- `Bind<T>(instance)` — Store an _already constructed_ instance with **no injection**. Duplicate binds are illegal
- `BindNew<T>()` — `new T()` → **field inject** → **`Initialize()` if implemented** → store. Throws on missing dependencies or duplicate types
- `BindFactory<T>()` — Register a `Factory<T>` for creating injectable plain objects
- `BindFactory<T>(prefab)` — Register a `PrefabFactory<T>` for creating injectable Unity prefabs

**Internal APIs** (used by factories only):

- `Inject(object)` — Field‑inject dependencies into an object
- `InjectGameObject(GameObject, ...)` — Field‑inject across a hierarchy; calls `Initialize()` on components that implement it

> **Important**: Injection scans **only fields declared on the concrete type** (uses `BindingFlags.DeclaredOnly`). Private fields declared in base classes are **not** injected.

### CompositionRoot + DIContext

1. Create one or more `CompositionRoot` assets (ScriptableObjects) and implement `Compose(Container container)`
2. Create a **single enabled** `DIContext` asset in `Resources/`
3. At startup (`BeforeSceneLoad`), it constructs a `Container` and runs all `Compose(...)` methods in dependency order

### Factories

**Factory<T>** — For plain objects (`where T : IInitializable, new()`):

- `Create()` → `new T()` → inject → `Initialize()` → return

**PrefabFactory<T>** — For prefabs (`where T : Component, IInitializable`):

- `Create(parent, name, position, rotation)` → `Instantiate(prefab)` → inject whole hierarchy → `Initialize()` components that implement it → return `T` on the root

> **Pro tip**: To guarantee injection before `Awake()`, save DI'd prefabs **inactive** in your project. The factory inherits the inactive state, injects dependencies, then you can activate safely.

---

## Installation

### Unity Package Manager (Recommended)

1. **Add via Git URL**:

   - Open **Window → Package Manager**
   - Click **+** → **Add package from git URL**
   - Enter: `https://github.com/singh-ps/nano-di.git?path=Packages/NanoDI`

2. **Add via manifest.json**:
   ```json
   {
     "dependencies": {
       "com.singh-ps.nanodi": "https://github.com/singh-ps/nano-di.git?path=Packages/NanoDI"
     }
   }
   ```

### Manual Installation

1. Download the latest [release](https://github.com/singh-ps/nano-di/releases)
2. Extract and copy the `NanoDI` folder to your project's `Packages/` directory

### Import Samples

After installation, import the comprehensive GamePlay sample:

- **Window → Package Manager** → **NanoDI** → **Samples** → **Import**

---

## Quick Start

### 1. Setup DIContext

After installation, create the DI context:

1. Create a `Resources/` folder in your project
2. **Right-click** → **Create** → **NanoDI** → **DIContext**
3. Check **enabled**
4. Add your `CompositionRoot` assets to the list

### 2. Define Your Services

```csharp
using NanoDI;

public class UIManager : IInitializable
{
    public void Initialize() => Debug.Log("UI Manager initialized!");
    public void ShowHUD() => Debug.Log("Showing HUD");
}

public class GameManager : IInitializable
{
    [Inject] private UIManager _ui;  // Field injection

    public void Initialize()
    {
        _ui.ShowHUD();  // Dependencies are ready!
    }
}
```

### 3. Create a CompositionRoot

```csharp
[CreateAssetMenu(menuName = "Game/Composition Root")]
public class GameComposition : CompositionRoot
{
    public override void Compose(Container container)
    {
        container.BindNew<UIManager>();     // Creates and injects UIManager
        container.BindNew<GameManager>();   // Creates and injects GameManager
    }
}
```

### 4. Add CompositionRoot to DIContext

1. **Right-click** → **Create** → **Game** → **Composition Root**
2. Open your `DIContext` asset in `Resources/`
3. Add the `GameComposition` asset to the compositions list

## Advanced Usage

### Factory Pattern for Dynamic Objects

Perfect for spawning enemies, projectiles, or any runtime objects:

```csharp
public class Enemy : IInitializable
{
    [Inject] private GameAnalytics _analytics;

    public void Initialize()
    {
        _analytics.TrackEnemySpawned();
    }
}

public class EnemySpawner : MonoBehaviour, IInitializable
{
    [Inject] private Factory<Enemy> _enemyFactory;

    public void Initialize() { }

    public void SpawnEnemy()
    {
        var enemy = _enemyFactory.Create();  // Fully injected!
    }
}

// In your CompositionRoot:
public override void Compose(Container container)
{
    container.BindNew<GameAnalytics>();
    container.BindFactory<Enemy>();        // Registers Factory<Enemy>
}
```

### PrefabFactory for Unity GameObjects

Handle complex prefabs with full dependency injection:

```csharp
public class PlayerController : MonoBehaviour, IInitializable
{
    [Inject] private IInputProvider _input;
    [Inject] private GameManager _gameManager;

    public void Initialize()
    {
        Debug.Log("Player ready with all dependencies!");
    }
}

public class GameComposition : CompositionRoot
{
    [SerializeField] private PlayerController playerPrefab;  // Assign in inspector

    public override void Compose(Container container)
    {
        container.Bind<IInputProvider>(new KeyboardInput());
        container.BindNew<GameManager>();
        container.BindFactory(playerPrefab);  // Registers PrefabFactory<PlayerController>
    }
}

// Usage:
public class LevelManager : MonoBehaviour, IInitializable
{
    [Inject] private PrefabFactory<PlayerController> _playerFactory;

    public void Initialize()
    {
        var player = _playerFactory.Create(
            parent: transform,
            name: "Player",
            position: Vector3.zero,
            rotation: Quaternion.identity
        );
        // Player and ALL child components are injected!
    }
}
```

### Interface-Based Dependencies

Use interfaces for flexible, testable code:

```csharp
public interface IInputProvider
{
    Vector2 GetMovementInput();
}

public class KeyboardInput : IInputProvider
{
    public Vector2 GetMovementInput() =>
        new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
}

public class TouchInput : IInputProvider
{
    public Vector2 GetMovementInput() => /* touch logic */;
}

// In CompositionRoot - easy to swap implementations:
public override void Compose(Container container)
{
    IInputProvider input = Application.isMobilePlatform
        ? new TouchInput()
        : new KeyboardInput();

    container.Bind(input);  // Bind interface to implementation
}
```

---

## Rules & Guarantees

- **Only fields** marked with `[Inject]` are injected
- **Field discovery** is cached per type for optimal performance
- **`BindNew<T>()`** always calls `Initialize()` after injection
- **Compiled injectors** provide fast, IL2CPP-safe injection
- **Duplicate binds** throw `InvalidOperationException`
- **Missing dependencies** throw with clear error messages
- **No service locator** — no `Get<T>()`/`TryGet<T>()` methods
- **No base class fields** — only declared fields are injected

---

## Performance Notes

- **Reflection cache**: Field discovery is cached per type; subsequent injections skip attribute scans
- **Compiled injection**: Uses compiled delegates for lightning-fast repeat injections (IL2CPP‑safe)
- **Zero-alloc prefabs**: `PrefabFactory` uses `GetComponentsInChildren(true, List<T>)` with reused buffers
- **Minimal overhead**: Container operations are optimized for game development performance requirements

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
