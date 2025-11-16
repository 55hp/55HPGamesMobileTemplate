# 📘 55HP Games – Core Architecture Manual

# # 🧭 1. Overview

This document explains the **core runtime architecture** of the 55HP Games Mobile Template.

It covers the four fundamental systems that define how the entire framework works:

1. **Service Registry** (dependency injection / service locator)
2. **Game State Machine** (async game flow)
3. **Addressables Architecture** (content loading)
4. **Gameplay Flow & Boot Sequence** (from app launch to menu to gameplay)

These modules together form the “core engine” of your template.

They are intentionally framework-agnostic, lightweight, and suitable for any mobile game genre.

---

# # 🧱 2. Service Registry Architecture

The template uses a simple and powerful **Service Registry**, equivalent to a minimal dependency injection container.

All services are singletons, resolved and registered through:

```csharp
ServiceRegistry.Register<T>(instance);
ServiceRegistry.Resolve<T>();
ServiceRegistry.TryResolve<T>(out T service);

```

### ## 2.1 Purpose of the Service Registry

- Decouples systems (UI, Audio, FSM, Gameplay)
- Avoids monolithic singletons
- Ensures consistent lifetime and initialization order
- Allows services to depend on each other without scene dependencies
- Allows PlayMode tests to mock services easily

### ## 2.2 Core vs UI Services

The template distinguishes between:

### **Core Services**

Registered in:

```
ServiceRegistry.InstallDefaults()

```

These services exist independently of the UI, and are loaded before any scenes:

| Service | Responsibility |
| --- | --- |
| `ILog` | Logging abstraction |
| `IEventBus` | Event dispatcher |
| `IConfigService` | ScriptableObject game configuration |
| `ISaveService` | JSON save system |
| `IContentLoader` | Addressables wrapper |
| `IGameStateMachine` | Async FSM |
| `IUIOptionsService` | Master settings service |

### **UI Services**

Registered inside:

```
91_UI_Root → UIServiceInstaller

```

| Service | Responsibility |
| --- | --- |
| `IUINavigationService` | UI Pages |
| `IUIPopupService` | Popups & scrim |
| `IUIOverlayService` | Fade & loading |
| `IUIToastService` | Toasts |
| `IMusicService` | BGM crossfades |

### ## 2.3 Lifetime and Ownership

- Core services live for the entire application run.
- UI services live as long as `91_UI_Root` is loaded.
- Services do **not** live inside scenes.
- They are not destroyed when changing states or loading gameplay.

### ## 2.4 Dependency Rules

- **Core services must NOT depend on UI services.**
- UI services may depend on:
    - Core services
    - Config
    - Save
- Gameplay code may depend on both.

---

# # 🎮 3. Game State Machine (FSM)

The template uses an **async-safe FSM** to manage game flow.

Each state implements:

```csharp
public interface IGameState
{
    Task EnterAsync(CancellationToken ct);
    Task ExitAsync(CancellationToken ct);
}

```

The state machine:

```csharp
public interface IGameStateMachine
{
    IGameState Current { get; }
    Task ChangeStateAsync(IGameState next);
    void CancelCurrent();
}

```

### ## 3.1 Why async FSM?

- Allows safe UI transitions
- Prevents scene race conditions
- Makes it easy to use `await` inside states
- Enables loading screens and fade transitions naturally

### ## 3.2 Standard flow

Example: switching to gameplay:

```csharp
await fsm.ChangeStateAsync(new GameplayState());

```

States always run in sequence:

```
ExitAsync(old)
EnterAsync(new)

```

Cancellation is supported via `CancellationToken`.

---

# # 📦 4. Addressables Architecture

The template uses Addressables for:

- UI Pages
- Popups
- Toasts
- Overlays
- Audio (BGM & SFX)
- Content data
- Config assets

### ## 4.1 Addressables Loader

All loading goes through:

```csharp
public interface IContentLoader
{
    Task<T> LoadAsync<T>(string address);
    Task ReleaseAsync(object handle);
}

```

The template supplies:

- `BasicContentLoader` (Resources)
- `AddressablesContentLoader` (default, full Addressables)

### ## 4.2 UI Content Convention

UI elements are organized by type:

```
content/ui/pages/<PageName>
content/ui/popups/<PopupName>
content/ui/toasts/<ToastName>
content/ui/overlays/<OverlayName>

```

### ## 4.3 Audio Convention

```
content/audio/bgm/<Track>
content/audio/sfx/<Clip>

```

### ## 4.4 Gameplay Content Examples

```
content/game/enemies/<Name>
content/game/items/<Name>
content/game/levels/<Name>

```

---

# # 🚀 5. Gameplay Flow & Boot Sequence

This is the **heart** of the template.

### ## 5.1 The game always starts from

```
Scenes/00_Bootstrap

```

This scene contains:

- `GameBootstrap.cs`
- No camera (to avoid multiple listeners)
- No UI
- No audio

### ## 5.2 Additive Scene Loading Order

`GameBootstrap` performs:

1. Install core services
2. Load `90_Systems_Audio`
3. Load `91_UI_Root`
4. Load `01_Menu`
5. Wait for UI services init
6. Start the FSM with `MainMenuState`

### ## 5.3 Boot pseudocode

```csharp
await LoadSceneAdditive("90_Systems_Audio");
await LoadSceneAdditive("91_UI_Root");
await LoadSceneAdditive("01_Menu");

while (!UIRuntime.ServicesReady)
    await Task.Yield();

await fsm.ChangeStateAsync(new MainMenuState());

```

### ## 5.4 Why additive?

Because:

- audio exists separately
- UI is layered independently
- gameplay scenes can be swapped without killing UI
- menu scene remains clean
- states manage logic, not scene loading

### ## 5.5 Typical Game Flow

```
Bootstrap
  ↓
MainMenuState
  ↓ play
GameplayState
  ↓ victory
ResultsState
  ↓ continue
MainMenuState

```

### ## 5.6 Recommended Folder Structure for States

```
Assets/Game/Features/States/
  MainMenuState.cs
  GameplayState.cs
  ResultsState.cs

```

---

# # 🧪 6. Testing Architecture (Optional but Recommended)

The template supports:

### PlayMode Tests

- UI services
- content loading
- audio behavior
- FSM transitions

### EditMode Tests

- Save data
- Config validation
- Utility functions

---

# # 📘 7. Summary

This Core Architecture provides:

- deterministic startup
- async-safe transitions
- clean service separation
- modular UI
- robust audio pipeline
- scalable gameplay structure
- reusable content loading patterns

It is designed to work for:

- hypercasual
- casual
- midcore
- roguelike
- narrative
- puzzle
- RPG-lite
- mobile action games

You now have a stable, extensible, professional-grade core suitable for multiple new projects.