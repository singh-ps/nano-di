# NanoDI

A minimal, fast dependency injection framework for Unity.  
**Field injection only.** **No service locator.** **Factory-first creation.**

---

## Features

- **Field injection only** — no property, method, or constructor injection.
- **Non‑recursive container** — the container never auto-constructs graphs.
- **Fail‑fast** — duplicate binds throw; missing dependencies during injection throw with clear messages.
- **Factory‑first** — use `Factory<T>` (plain objects) and `PrefabFactory<T>` (Unity prefabs).
- **Deterministic lifecycle** — `BindNew<T>()` always calls `Initialize()` (if implemented) after injection.
- **Performance‑minded** — per‑type reflection cache and compiled injectors; zero‑alloc prefab scanning.

---

## Design Principles

- **Simple by default**: keep DI lean; push complexity into factories and composition.
- **Explicit composition**: all bindings are declared in `CompositionRoot` assets executed by `DIContext` before scene load.
- **No service locator**: you don’t “get” instances from the container; you **bind** and **inject** via factories.
- **Strict wiring**: if something isn’t bound, that’s a bug—throw early.

---

## How It Works

### Container

- `Bind<T>(instance)` — store an _already constructed_ instance; **no injection** happens here. Duplicate binds are illegal.
- `BindNew<T>()` — `new T()` → **field inject** → **`Initialize()` if implemented** → store. Throws on missing deps or duplicate type.
- Internal APIs (used by factories only):
  - `Inject(object)` — field‑injects dependencies into an object.
  - `InjectGameObject(GameObject, ...)` — field‑injects across a hierarchy; calls `Initialize()` on components that implement it.

> Injection scans **only fields declared on the concrete type** (uses `BindingFlags.DeclaredOnly`). Private fields declared in base classes are **not** injected.

### CompositionRoot + DIContext

- Create one or more `CompositionRoot` assets (ScriptableObjects) and implement `Compose(Container container)`.
- Create a single **enabled** `DIContext` asset in `Resources/`. At startup (BeforeSceneLoad), it constructs a `Container` and runs all `Compose(...)` methods in order.

### Factories

- **Factory<T>** — for plain objects (`where T : IInitializable, new()`):
  - `Create()` → `new T()` → inject → `Initialize()` → return.
- **PrefabFactory<T>** — for prefabs (`where T : Component, IInitializable`):
  - `Create(parent, name, position, rotation)` → `Instantiate(prefab)` → inject whole hierarchy → `Initialize()` components that implement it → return `T` on the root.

> To guarantee injection before `Awake()`, save DI’d prefabs **inactive** in your project. Instantiate will inherit the inactive state, then the factory injects and finally you may activate.

---

## Installation

1. Copy the `NanoDI` scripts into your Unity project (any assembly definition is fine).
2. Create a `Resources/` folder and add a **DIContext** asset:
   - `Create → NanoDI → DIContext`
   - Check **enabled**
   - Add your `CompositionRoot` assets to its list
3. Ensure exactly **one** enabled `DIContext` exists in `Resources/`.

---

## Usage

### 1) Define your services

```csharp
public class UIManager : IInitializable
{
    public void Initialize() { /* setup ui */ }
    public void Show(string id) { /* ... */ }
}

public class GameManager : IInitializable
{
    [Inject] private UIManager _ui;

    public void Initialize()
    {
        _ui.Show("HUD");
    }
}
```

### 2) Bind them in a CompositionRoot

```csharp
public class GameComposition : CompositionRoot
{
    public override void Compose(Container c)
    {
        c.BindNew<UIManager>();
        c.BindNew<GameManager>();
    }
}
```

### 3) Plain object creation via Factory<T>

```csharp
public class Enemy : IInitializable
{
    [Inject] private UIManager _ui;
    public void Initialize() { /* uses _ui */ }
}

public class GameplayComposition : CompositionRoot
{
    public override void Compose(Container c)
    {
        c.BindNew<UIManager>();
        c.BindFactory<Enemy>(); // binds Factory<Enemy>
    }
}
```

Now inject the factory where you need it:

```csharp
public class Spawner : MonoBehaviour, IInitializable
{
    [Inject] private Factory<Enemy> _enemyFactory;

    public void Initialize()
    {
        var e = _enemyFactory.Create(); // injected & initialized
    }
}
```

### 4) Prefab creation via PrefabFactory<T>

```csharp
public class EnemyController : MonoBehaviour, IInitializable
{
    [Inject] private UIManager _ui;
    public void Initialize() { /* uses _ui */ }
}

public class PrefabComposition : CompositionRoot
{
    public EnemyController enemyPrefab; // assign in the asset

    public override void Compose(Container c)
    {
        c.BindNew<UIManager>();
        c.BindFactory(enemyPrefab);      // binds PrefabFactory<EnemyController>
    }
}
```

Use it:

```csharp
public class EnemyWave : MonoBehaviour, IInitializable
{
    [Inject] private PrefabFactory<EnemyController> _enemyFactory;

    public void Initialize()
    {
        var enemy = _enemyFactory.Create(transform, "Enemy_A", Vector3.zero, Quaternion.identity);
        // enemy and its hierarchy are injected; IInitializable components have run Initialize()
    }
}
```

---

## Rules & Guarantees

- Only **fields** marked with `[Inject]` are injected.
- Injection reads **only fields declared on the concrete type** (no base‑type private fields).
- `BindNew<T>()` always calls `Initialize()` (if implemented) after injection.
- Duplicate binds (same `T`) are illegal and throw.
- No service locator: there is **no** `Get<T>()`/`TryGet<T>()` API.
- Factories are the only public way to construct and inject new objects/prefabs.

---

## Performance Notes

- Field discovery is cached per type; subsequent injections skip attribute scans.
- Injection uses a **compiled delegate** per type for fast repeat calls (IL2CPP‑safe).
- `PrefabFactory` uses `GetComponentsInChildren(true, List<T>)` with a **reused buffer** (no per‑spawn arrays).

---

## Next Steps

- Unity Package Manager (UPM) distribution.
- Child scopes (scene/subsystem containers).
- Editor tooling to bake prefab “injection manifests” for zero‑scan instantiation.

---

## License

MIT
