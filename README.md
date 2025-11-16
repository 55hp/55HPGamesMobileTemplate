# 📘 55HP Games — Unity Mobile Template

A clean, modular and production-ready starter template for mobile games built with Unity 2022 LTS.

Designed to support **hypercasual**, **casual**, and **mid-core** mobile titles with solid foundations and reusable systems.

---

# 🚀 **1. Overview**

This template provides:

- A robust **bootstrap flow** with additive scenes
- A service-based architecture (Service Registry)
- A modular **UI framework** (Pages, Popups, Overlays, Toasts)
- A fully isolated **Options/Settings service** (volumes, mute, haptics, language)
- An **Audio pipeline** with mixer routing + BGM crossfade
- A **Game State Machine** with async transitions
- Addressables-based loading for UI, audio, and content
- A clean project structure ready to clone for new games

The entire system is built to *avoid spaghetti*, keep everything extensible, and make it easy to start multiple new projects from this single template.

---

# 🧱 **2. Project Structure**

```
Assets/
 ├─ Core/
 │   ├─ Architecture/     (State Machine, Service Registry, Interfaces)
 │   ├─ Runtime/
 │   │    ├─ Audio/       (AudioOptionsBinder, MusicService helpers)
 │   │    ├─ Content/     (Addressables loader)
 │   │    ├─ Config/      (Config + ConfigService)
 │   │    ├─ Save/        (Save & Load system)
 │   │    ├─ Pooling/     (ObjectPoolService)
 │   │    ├─ UI/          (UI Runtime helpers)
 │   └─ ...
 ├─ Game/
 │   ├─ Features/         (States, Menu UI controllers, gameplay features)
 │   └─ ...
 ├─ Scenes/
 │   ├─ 00_Bootstrap
 │   ├─ Additive/
 │   │    ├─ 90_Systems_Audio
 │   │    └─ 91_UI_Root
 │   ├─ 01_Menu
 │   └─ 02_Gameplay (or your game scenes)
 └─ ...

```

---

# 🧭 **3. Scene Flow (Additive Bootstrap)**

The game ALWAYS starts from:

```
Scenes/00_Bootstrap

```

Inside this scene, the `GameBootstrap` component:

1. Installs core services
2. Loads additive scenes in order:
    - `90_Systems_Audio`
    - `91_UI_Root`
    - `01_Menu`
3. Waits for UI services to initialize
4. Starts the initial game state (`MainMenuState`)

This ensures:

- All services are registered
- UI layers exist in scene
- MusicPlayer exists
- State machine transitions are safe
- No timing hacks in UI or States

---

# 🧩 **4. Service Architecture**

The template uses a simple and powerful **Service Registry**, similar to a mini-IoC container:

```csharp
ServiceRegistry.Register<T>(instance);
var service = ServiceRegistry.Resolve<T>();

```

### Core services (installed in `InstallDefaults()`):

| Service Type | Responsibility |
| --- | --- |
| `ILog` | Logging abstraction |
| `IEventBus` | Messaging/event system |
| `IConfigService` | Global SO config |
| `ISaveService` | JSON save/load |
| `IContentLoader` | Addressables wrapper |
| `IGameStateMachine` | Async FSM |
| `IUIOptionsService` | Master settings (volumes, mute, haptics, language) |

### UI services (installed in `UIServiceInstaller`, inside `91_UI_Root`):

| Service | Responsibility |
| --- | --- |
| `IUINavigationService` | UI Pages (open/close) |
| `IUIPopupService` | Popups with scrim |
| `IUIOverlayService` | Fade & loading overlay |
| `IUIToastService` | Toast messages |
| `IMusicService` | Background music crossfades |

---

# 🖥️ **5. UI Structure**

The UI Root scene (`91_UI_Root`) contains:

```
UIRoot (with UIRoot.cs)
  └─ Canvas
       ├─ Layer_HUD
       ├─ Layer_Pages
       ├─ Layer_Popups
       ├─ Layer_Overlay
       └─ Layer_Toasts

```

You load UI elements via Addressables using:

- Pages → Navigation Service
- Popups → Popup Service
- Overlays → Overlay Service
- Toasts → Toast Service

Each UI prefab is placed inside Addressables under:

```
content/ui/pages/<PageName>
content/ui/popups/<PopupName>
content/ui/toasts/<ToastName>
content/ui/overlays/<OverlayName>

```

---

# ⏯️ **6. Audio Pipeline**

### Music System (BGM)

- Handled by `IMusicService` / `UIMusicService`
- Uses a dual-AudioSource “crossfade” system
- Prefab `MusicPlayer` lives in `90_Systems_Audio`

### Mixer & Options

- Music volume
- SFX volume
- Music/SFX mute
- Bound to UIOptions via `AudioOptionsBinder`

### Addressables

BGM tracks live under:

```
content/audio/bgm/<TrackName>

```

---

# 🎮 **7. Game State Machine (FSM)**

The FSM uses async safe transitions:

```csharp
await fsm.ChangeStateAsync(new MainMenuState());

```

Every state implements:

```csharp
Task EnterAsync(CancellationToken ct);
Task ExitAsync(CancellationToken ct);

```

### Initial State

`GameBootstrap` starts:

```
MainMenuState

```

From there, you can push a UI page:

```csharp
await _nav.PushAsync(Addr.Content.UI.Pages.Main_Menu_Generic);

```

Or switch to gameplay:

```csharp
await fsm.ChangeStateAsync(new GameplayState());

```

---

# 🎯 **8. Creating a New Game With This Template**

To start a new project:

1. **Duplicate** the entire template folder or repo
2. Change:
    - Project name
    - Company name
    - Bundle ID
3. Create your own gameplay in `/Game/Features/`
4. Add new UI Pages/Popups in Addressables
5. Add new states (Menu → Gameplay → Results, etc.)
6. Replace demo assets with your own

Everything else (UI, options, music, FSM, services, audio mixing, scene flow) is already set up for you.

---

# 📦 **9. Addressables Conventions**

### UI

```
content/ui/pages/...
content/ui/popups/...
content/ui/toasts/...
content/ui/overlays/...

```

### Audio

```
content/audio/bgm/...
content/audio/sfx/...

```

### Config

```
config/main

```

### Content

Game-specific items, prefabs, data, etc.

---

# 🧪 **10. Tests**

All test scripts live under:

```
Assets/Tests/

```

Keep game scenes and test scenes **separated**.

---

# 🏁 **Done**

This template now provides:

- A stable and documented bootstrap
- A clean layered UI system
- A flexible Audio + Music pipeline
- A powerful async-FSM
- A reusable options/settings system
- A reusable object pool
- An architecture ready for any mobile game

You can now build your game directly on top of this structure without rewriting fundamentals.