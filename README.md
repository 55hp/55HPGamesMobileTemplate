# 🧱 55HP Unity Mobile Template – Core Template Overview v1

# 📘 **55HP Core Template v1 — Overview & Architecture**

*A flexible, production-ready foundation for Unity mobile games.*

---

## **1. Introduction**

The **55HP Core Template** is a modular, scalable foundation for creating **99% of Unity mobile games**, from hypercasual prototypes to mid-core arcade titles and lightweight roguelites.

The template focuses on:

- **Consistency** – shared architecture across all projects
- **Reusability** – mechanics and systems reusable across genres
- **Simplicity** – low cognitive overhead, clean API surface
- **Extensibility** – every module can grow independently
- **Fast Iteration** – ideal for rapid prototyping, soft-launch testing, and mobile-friendly workflows

This document provides a complete overview of the template’s architecture and how each subsystem works together.

---

## **2. Project Structure**

A typical project using the template is divided into:

```
Assets/
 ├── Core/
 │    ├── Runtime/
 │    │    ├── Architecture/       (Services, FSM, SceneFlow)
 │    │    ├── Save/               (SaveService, SaveData, PlayerProgressData)
 │    │    ├── Input/              (InputService + Driver)
 │    │    ├── Events/             (Event Bus)
 │    │    ├── UI/                 (UIRoot, Navigation, Popups, Toasts)
 │    │    ├── Pooling/            (ObjectPoolService, PooledObject)
 │    │    ├── Gameplay/           (Generic components)
 │    │    └── Utils/              (Time, Logging, Helpers)
 │    └── ...
 │
 ├── Game/                          (Project-specific content)
 │    ├── Features/                 (Gameplay logic, controllers, systems)
 │    ├── Content/                  (Prefabs, VFX, Audio, UI Screens)
 │    └── ...
 │
 ├── UI/                            (Canvas, screens, pages, popups)
 ├── Addressables/
 └── Scenes/
      ├── 01_Menu
      ├── 02_Gameplay
      ├── 03_Results
      └── 91_UI_Root

```

---

## **3. Service Architecture**

The template uses a lightweight **Service Registry** to expose global systems without relying on singletons.

### **Key Principles**

- Services register themselves on startup.
- No hidden singletons, no static state.
- Consumers request dependencies via:

```csharp
ServiceRegistry.TryResolve(out ITimeService time);
ServiceRegistry.Resolve<IEventBus>().Publish(new X());
```

### **Why this approach?**

- Decouples systems
- Makes testing easier
- Avoids “god-objects”
- Provides a clean extensible API for future modules

### **Core services available**

- `IEventBus`
- `IInputService`
- `ITimeService`
- `ISaveService`
- `ISceneFlowService`
- `IObjectPoolService`
- `IUIPopupService`
- `IUINavigationService`
- `IUIToastService`

---

## **4. Game State Machine (FSM)**

The FSM organizes gameplay flow across scenes and UI states.

It supports async entering/exiting, scene transitions, and UI swap.

### **Core states included**

- `MainMenuState`
- `GameplayState`
- `PauseState`
- `ResultState`

### **How FSM interacts with SceneFlow**

- Each state defines:
    - which scene must be active
    - which UI screen should appear
    - which metadata to update in GameContext

The FSM is intentionally small and domain-agnostic, so each game can extend it with its own states.

---

## **5. Input System**

### **Components**

- **IInputService**
    
    High-level abstraction (`IsTap`, `IsPress`, `IsSwipe` if implemented).
    
- **InputServiceDriver**
    
    The MonoBehaviour that reads Unity’s native input and feeds the service.
    

### **Goals**

- Centralize input handling
- Allow easy swapping for virtual joysticks, gestures, UI input
- Decouple gameplay code from Unity APIs

### **Usage**

```csharp
if (_input.IsTap)
    Jump();
```

---

## **6. Event Bus**

The **Event Bus** is the communication backbone between gameplay systems, UI, and services.

### **Why an event bus?**

- Avoid direct references between UI ↔ gameplay
- Unidirectional communication
- Extremely cheap and predictable
- Works well with pooling and heavy object churn

### **Example**

```csharp
_bus.Subscribe<ScoreChangedEvent>(OnScoreChanged);
_bus.Publish(new PlayerDeathEvent());
```

### **Best use cases**

- Score updates
- Player death
- Level events
- Power-up events
- UI notifications

---

## **7. Save System**

The save system uses JSON serialization with a structured `SaveData` model.

### **Key elements**

- `SaveService` handles loading/saving, versioning, and disk I/O.
- `SaveData` is the root object holding all persistent data.
- `PlayerProgressData` stores generic long-term progress:
    - `bestScore`
    - `highestLevel`
    - `lifetimeCoins`
    - (extensible for any future game)

### **Reading & writing**

```csharp
var best = save.Data.progress.bestScore;
save.Data.progress.bestScore = newBest;
save.Save();
```

The system is intentionally minimal to keep overhead low and extensibility high.

---

## **8. Scene Flow Architecture**

The **SceneFlowService** coordinates transitions between scenes and UI states.

### **Responsibilities**

- Loading target scenes
- Ensuring UI Root always exists
- Showing overlays during loads
- Triggering FSM state changes

### **Benefits**

- Centralized navigation logic
- Predictable transitions
- Works seamlessly with Addressables
- Clean separation between “gameplay logic” and “scene management”

---

## **9. UI System**

The UI is designed to be **page-based**, flexible, and fully decoupled from gameplay code.

### **Components**

- **UIRoot**: top-level canvas, persistent across scenes
- **IUINavigationService**: push/replace UI screens
- **IUIPopupService**: modal windows
- **IUIToastService**: transient notifications
- **UILocalizedText**: supports prefix/suffix + localization keys

### **UI Page Lifecycle**

Each screen follows:

```csharp
OnNavigationIn()
OnNavigationOut()
OnNavigationReplaced()
```

This makes transitions predictable and allows animations or cross-fades.

---

## **10. Object Pooling System**

Designed for heavy mobile workloads, where instantiating/destroying objects is expensive.

### **Components**

- `PooledObject`
- `ObjectPoolService`
- **Core gameplay helpers:**
    - `TimedPooledSpawner2D`
    - `DespawnWhenOutOfBounds2D`
    - `DespawnAfterSeconds`
    - `ConstantMover2D`

### **Typical usage**

```csharp
var pipe = _pool.Get(pipePrefab);
pipe.transform.position = spawnPosition;

```

### **Strengths**

- Zero-garbage spawning
- Works automatically with despawn components
- Ideal for endless runners, bullet hells, VFX-heavy games

---

## **11. Core Gameplay Components**

Standard reusable building blocks:

### ✔️ TimedPooledSpawner2D

Periodic spawn of pooled objects (pipes, enemies, coins).

### ✔️ DespawnWhenOutOfBounds2D

Automatic cleanup when objects leave the playable area.

### ✔️ DespawnAfterSeconds

Time-based despawn (ideal for VFX, projectiles).

### ✔️ ConstantMover2D

Uniform translation (endless runner, projectile, conveyor belts).

All designed to be game-agnostic and flexible.

---

## **12. Mobile Build & Display Configuration**

### Default orientation

Most games should lock either **portrait** or **landscape** at the project level.

### Recommended mobile settings

- VSync off
- TargetFrameRate = 60
- Landscape Left / Right (depending on the game)
- Flat Colors or 2D URP minimal for performance
- Safe Area handling via UI root (optional future module)

---

## **13. Creating a New Game Using the Template**

1. Clone the template
2. Choose orientation (portrait/landscape)
3. Set up scenes:
    - Menu
    - Gameplay
    - Results
    - UI Root
4. Create main `GameController`
5. Use core systems:
    - InputService
    - TimeService
    - EventBus
    - SaveService
    - Pooling
    - FSM & SceneFlow
6. Build UI pages via NavigationService
7. Implement gameplay loop
8. Add results & progression
9. Polish and publish

This workflow allows you to create new games extremely quickly while maintaining clean, reusable architecture.

---

# 🎉 **End of Document – Main Overview Ready**