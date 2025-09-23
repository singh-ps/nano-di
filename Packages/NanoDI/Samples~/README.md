# NanoDI Samples

This folder contains comprehensive examples demonstrating all features of the NanoDI dependency injection framework.

> **UPM Note**: These samples are designed for Unity Package Manager. Import them via **Window → Package Manager → NanoDI → Samples**.

---

## GamePlay Sample

The **GamePlay** sample showcases a complete game architecture using NanoDI, demonstrating all core features through a practical game scenario with players, enemies, weapons, and input systems.

### Project Structure

```
GamePlay/
├── CompositionRoot/
│   └── GamePlayCR.cs              # Main composition root
├── Enemy/
│   ├── BaseEnemy.cs               # Base enemy with injection
│   ├── MeleeEnemy.cs              # Melee enemy variant
│   ├── RangedEnemy.cs             # Ranged enemy variant
│   └── EnemySystem.cs             # Enemy management system
├── Input/
│   ├── IInputProvider.cs          # Input abstraction interface
│   ├── KeyboardInput.cs           # Keyboard implementation
│   └── GamepadInput.cs            # Gamepad implementation
├── Prefab/
│   ├── Player.cs                  # Player controller
│   └── Weapon.cs                  # Weapon component
├── GameManager.cs                 # Main game coordinator
├── UIManager.cs                   # UI management service
├── GameAnalytics.cs               # Analytics service
├── PlayerStats.cs                 # Player statistics
├── WeaponStats.cs                 # Weapon statistics
└── EnemyStats.cs                  # Enemy statistics
```

### Features Demonstrated

#### 1. **All Binding Types**

- **`BindNew<T>()`** - Create and inject singletons
- **`Bind<T>(instance)`** - Bind existing instances with conditional logic
- **`BindFactory<T>(prefab)`** - Unity prefab factories
- **`BindFactory<T>()`** - Plain C# object factories

#### 2. **Field Injection Patterns**

```csharp
public class Player : MonoBehaviour, IInitializable
{
    [Inject] private PlayerStats playerStats;           // Service injection
    [Inject] private PrefabFactory<Weapon> weaponFactory; // Factory injection
    [Inject] private IInputProvider inputProvider;      // Interface injection
}
```

#### 3. **Interface-Based Dependency Injection**

```csharp
// Conditional binding based on configuration
IInputProvider inputProvider = useGamePad ? new GamepadInput() : new KeyboardInput();
container.Bind(inputProvider);
```

#### 4. **Inheritance Hierarchies**

```csharp
public class BaseEnemy : MonoBehaviour, IInitializable
{
    [Inject] protected EnemyStats stats;        // Protected field injection
    [Inject] private GameAnalytics analytics;   // Service injection
}

public class MeleeEnemy : BaseEnemy  // Inherits injection capabilities
{
    public override void Attack() { /* uses injected stats */ }
}
```

#### 5. **Factory Usage Patterns**

```csharp
public class EnemySystem : IInitializable
{
    [Inject] private PrefabFactory<MeleeEnemy> meleeEnemyFactory;
    [Inject] private PrefabFactory<RangedEnemy> rangedEnemyFactory;

    public void Initialize()
    {
        // Create enemies with injection
        var enemy = meleeEnemyFactory.Create(
            null,
            "MeleeEnemy[0]",
            Vector3.zero,
            Quaternion.identity
        );
    }
}
```

#### 6. **Initialization Lifecycle**

```csharp
public class GameManager : IInitializable
{
    [Inject] private UIManager uiManager;
    [Inject] private Factory<EnemySystem> enemySystemFactory;

    public void Initialize()
    {
        // All dependencies are injected and initialized
        uiManager.ShowUI("GameHUD");
        var enemySystem = enemySystemFactory.Create(); // Fully injected
    }
}
```

### Setup Instructions

#### **Import Sample via Package Manager**

1. Open **Window → Package Manager**
2. Select **NanoDI** package
3. Expand **Samples** section
4. Click **Import** next to "GamePlay Sample"
5. Sample will be imported to `Assets/Samples/NanoDI/[version]/GamePlay/`

#### **Manual Setup (if needed)**

1. Create a `Resources/` folder in your project
2. Right-click → **Create → NanoDI → DIContext**
3. Name it `GamePlayDIContext`
4. Check **"enabled"** checkbox
5. Add the `GamePlayCR` composition root to the compositions list

#### **Configure Composition Root**

1. Locate the imported `GamePlayCR` asset
2. Assign prefab references:
   - `meleeEnemyPrefab` - MeleeEnemy prefab
   - `rangedEnemyPrefab` - RangedEnemy prefab
   - `playerPrefab` - Player prefab
   - `weaponPrefab` - Weapon prefab
3. Configure `useGamePad` toggle for input switching

#### **Prefab Configuration**

- The sample includes pre-configured prefabs
- Ensure all prefabs have their respective components attached
- **Important**: Prefabs are saved as **inactive** to guarantee injection happens before `Awake()`

### Key Classes Overview

#### **Services (Singletons)**

- **`GameManager`** - Main game coordinator, demonstrates initialization order
- **`UIManager`** - UI management service
- **`GameAnalytics`** - Analytics logging service
- **`PlayerStats`** - Player statistics data
- **`WeaponStats`** - Weapon statistics data
- **`EnemyStats`** - Enemy statistics data

#### **Systems**

- **`EnemySystem`** - Enemy spawning and management, demonstrates object factories
- **`IInputProvider`** - Input abstraction with multiple implementations

#### **Prefab Components**

- **`Player`** - Player controller with weapon factory usage
- **`Weapon`** - Weapon component with stats injection
- **`BaseEnemy`** - Base enemy class with protected field injection
- **`MeleeEnemy/RangedEnemy`** - Enemy variants demonstrating inheritance

### Runtime Flow

1. **Startup** - `DIContext` initializes before scene load
2. **Composition** - `GamePlayCR.Compose()` binds all dependencies
3. **Game Start** - `GameManager.Initialize()` coordinates game startup
4. **Factory Usage** - Systems create players, enemies, weapons with full injection
5. **Runtime** - All objects have their dependencies satisfied

### Configuration Options

#### **Input System Toggle**

```csharp
[SerializeField] private bool useGamePad; // In GamePlayCR inspector
```

- `false` - Uses KeyboardInput implementation
- `true` - Uses GamepadInput implementation

#### **Error Demonstration**

Uncomment these lines in `GamePlayCR.cs` to see duplicate binding errors:

```csharp
// DEMO: This would throw - duplicate binding (uncomment to test)
// container.BindNew<PlayerStats>();
// container.BindNew<PlayerStats>(); // InvalidOperationException
```

### Learning Outcomes

After studying this sample, you'll understand:

- **Complete NanoDI feature set** - All binding types and injection patterns
- **Real-world architecture** - Practical game development patterns
- **Interface-based design** - Polymorphic dependency injection
- **Factory patterns** - Both prefab and object creation
- **Initialization lifecycle** - Proper startup sequencing
- **Error handling** - What happens when bindings are missing or duplicated
- **Performance considerations** - Compiled injection and caching

### Advanced Patterns

#### **Conditional Binding**

The sample demonstrates runtime configuration-based binding:

```csharp
IInputProvider inputProvider = useGamePad ? new GamepadInput() : new KeyboardInput();
container.Bind(inputProvider);
```

#### **Mixed Factory Types**

Shows both prefab factories (Unity components) and object factories (plain C#):

```csharp
container.BindFactory(playerPrefab);        // PrefabFactory<Player>
container.BindFactory<EnemySystem>();       // Factory<EnemySystem>
```

#### **Hierarchical Injection**

Demonstrates injection across inheritance hierarchies:

```csharp
BaseEnemy → MeleeEnemy/RangedEnemy  // Protected field inheritance
```

---

## Package Manager Integration

This sample is designed for Unity Package Manager distribution:

- **Automatic Import** - Import via Package Manager UI
- **Version Management** - Sample updates with package versions
- **Isolated Structure** - Samples don't interfere with your project
- **Easy Cleanup** - Remove samples without affecting the core package

---

## Next Steps

- **Experiment** with the configuration options
- **Modify** the binding order to see initialization dependencies
- **Add** new systems following the established patterns
- **Test** error scenarios by commenting out bindings
- **Profile** the injection performance with the Unity Profiler

This sample serves as both a learning resource and a template for structuring your own projects with NanoDI.
