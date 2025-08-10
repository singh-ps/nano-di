# NanoDI

A lightweight dependency injection framework for Unity.  
Supports constructor, field, and property injection, prefab factories, and a simple composition root for binding services before scene load.

## Features

- Constructor, field, and property injection via `[Inject]`
- Composition using `DIContext` and `CompositionRoot`
- Prefab instantiation with automatic injection using `PrefabFactory<T>`
- Optional post-injection initialization via `IInitializable`
- Single container created before scene load without static references

## How It Works

1. `DIContext` (ScriptableObject) runs before scene load, creates a `Container`, and calls `Compose(container)` on all registered `CompositionRoot` assets.
2. `Container` resolves dependencies by:
   - Selecting a constructor (prefers `[Inject]`, else the longest)
   - Resolving constructor parameters recursively
   - Invoking the constructor
   - Injecting fields and properties marked `[Inject]`
   - Calling `Initialize()` if the instance implements `IInitializable`
3. `PrefabFactory<T>` instantiates a prefab, injects all components in the hierarchy, and calls `Initialize()` where applicable.

## Installation

1. Add the `NanoDI` scripts to your Unity project.
2. Create a **Resources** folder and add a `DIContext` asset:
   - `Create > NanoDI > GlobalDIContext`
   - Enable the asset
   - Assign one or more `CompositionRoot` assets

## Key Types

### InjectAttribute
```csharp
public class Foo
{
    [Inject] public Foo(Bar bar) { }

    [Inject] private Baz _baz;
    [Inject] public Qux Qux { get; set; }
}
```

### IInitializable
```csharp
public interface IInitializable
{
    void Initialize();
}
```

### Container
- `Bind<T>(T instance)` — bind an existing instance
- `BindNew<T>()` — create, inject, and bind a new instance
- `Get<T>()` — resolve or create an instance
- `InjectGameObject(GameObject go)` — inject all components in a hierarchy

### DIContext
- Holds a list of `CompositionRoot` assets
- Initializes a single container before scene load

### CompositionRoot
```csharp
public class GameComposition : CompositionRoot
{
    public override void Compose(Container c)
    {
        c.BindNew<UIManager>();
        c.BindNew<PlayerController>();
        c.BindNew<GameManager>();
    }
}
```

### IPrefabFactory<T> and PrefabFactory<T>
```csharp
public interface IPrefabFactory<T> where T : Component
{
    T Create(Transform parent, Vector3 position, Quaternion rotation);
}

public sealed class PrefabFactory<T> : IPrefabFactory<T> where T : Component
{
    private readonly Container _container;
    private readonly T _prefab;

    public PrefabFactory(Container container, T prefab)
    {
        _container = container;
        _prefab = prefab;
    }

    public T Create(Transform parent, Vector3 position, Quaternion rotation)
    {
        var go = Object.Instantiate(_prefab.gameObject, position, rotation, parent);
        _container.InjectGameObject(go);
        return go.GetComponent<T>();
    }
}
```

## Quick Start

1. Create a `DIContext` asset.
2. Add a `CompositionRoot` to bind services.
```csharp
public class AppComposition : CompositionRoot
{
    public override void Compose(Container c)
    {
        c.BindNew<UIManager>();
        c.BindNew<PlayerController>();
    }
}
```
3. Inject dependencies into classes.
```csharp
public class GameManager : IInitializable
{
    [Inject] private PlayerController _player;
    [Inject] public UIManager UI { get; set; }

    public void Initialize()
    {
        UI.ShowHUD();
        _player.Spawn("Player One");
    }
}
```
4. Use a prefab factory if needed.
```csharp
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyController enemyPrefab;
    private IPrefabFactory<EnemyController> _factory;

    public void Init(Container c)
    {
        _factory = new PrefabFactory<EnemyController>(c, enemyPrefab);
    }

    public void Spawn(Vector3 pos)
    {
        _factory.Create(transform, pos, Quaternion.identity);
    }
}
```

## Known Limitations

- Reflection used for each resolve and injection
- No support for named bindings or scopes
- `Bind<T>(instance)` does not allow duplicate bindings
- `PrefabFactory<T>` requires a `Component` prefab

## Next Steps

- Add caching for reflection data
- Use compiled activators for object creation
- Remove LINQ from critical code paths
- Add Unity Package Manager (UPM) support
