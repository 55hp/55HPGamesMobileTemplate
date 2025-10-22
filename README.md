# hp55games – Mobile Unity Template

A clean and modular **Unity template for mobile games**, designed to be cloned and reused across different gameplay projects.  
Includes a complete bootstrap flow, UI system, service architecture, and basic test coverage.

---

## 🧩 Requirements
- **Unity 2022.3 LTS** (URP)
- Android SDK & NDK installed via Unity Hub
- Core packages:
  - Input System
  - TextMeshPro
  - Universal Render Pipeline (URP)
  - Localization
  - Test Framework
  - Addressables

---

## 📂 Project Structure
| Path | Description |
|------|--------------|
| `Assets/Scripts/Core` | Core services: Logger, EventBus, ConfigService, SaveService |
| `Assets/Scripts/UI` | UI layer: UIRoot, UIPopup_Generic, UIToast, UILoadingOverlay, UIManager |
| `Assets/Scenes` | Scene setup: 00_Bootstrap, 90_Systems_Audio, 91_UI_Root, 01_Menu, 02_Game_* |
| `Assets/Tests` | EditMode & PlayMode test assemblies |
| `Packages/manifest.json` | Package configuration |
| `ProjectSettings/` | Build and quality settings |

---

## ▶️ Play Mode Boot Sequence
1. Open scene **`00_Bootstrap`**.  
2. Press **Play** — the bootstrap system will automatically:
   - Load `90_Systems_Audio` (Additive)
   - Load `91_UI_Root` (Additive)
   - Load `01_Menu` (Additive, set as Active Scene)

This architecture allows for seamless transitions between scenes and a persistent service layer.

---

## 🧱 Core Systems Overview
- **Logger** — unified logging interface (`ILog`), bound to `UnityLog` via ServiceRegistry.
- **ConfigService** — handles local/remote config data (JSON-based).
- **SaveService** — manages save data, slots, and versioning.
- **EventBus** — decoupled communication between systems (Publish/Subscribe pattern).
- **UIManager** — central entry point for showing popups, toasts, and loading overlays.

---

## 🧪 Tests
- Open **Window → General → Test Runner**  
- Run both **EditMode** and **PlayMode** tests  
- PlayMode smoke test checks scene loading order and UI root integrity.

---

## 📱 Android Build
1. **Build Settings → Android**
2. Recommended Player Settings:
   - Scripting Backend: **IL2CPP**
   - Architectures: **ARMv7 + ARM64**
   - Managed Stripping Level: **Medium / High**
   - Minify (R8): **Enabled for Release builds**
3. Build in **Development Mode** for debugging

---

## 🎯 Addressables (optional but supported)
If your project requires dynamic content or runtime loading:
1. Open **Window → Asset Management → Addressables → Groups**
2. Mark assets as *Addressable* (e.g., `Configs/AppConfig`)
3. Build Addressables via  
   **Build → New Build → Default Build Script**
4. Initialize via `Addressables.InitializeAsync()` at startup.

---

## 🧭 Architecture Notes
- `GameBootstrap` and `GameStateMachine` are controlled singletons (`DontDestroyOnLoad`).  
  They are initialized once in `00_Bootstrap` and persist for the entire session.  
  Do **not** manually instantiate duplicates.
- `UIManager` locates `UIRoot` automatically after the `91_UI_Root` scene loads.
- The template uses **additive scene loading** for modularity and improved iteration speed.

---

## 🚀 Quick Start for a New Project
1. **Clone** this repository.  
2. Rename the folder and the Unity project in **ProjectSettings → Product Name**.  
3. Update `hp55games` namespaces to match your new project name (optional).  
4. Run the tests to confirm integrity.  
5. Start prototyping your gameplay scene inside `02_Game_YourFeature`.

---

## 📘 License
This template is free to use and modify for personal or commercial projects.  
Attribution is appreciated but not required.

---

*Maintained by Francesco Avitabile (hp55games).*
