# 🎮 hp55games – Scene Flow Overview
_Last updated: 2025-10-29_

---

## 🔧 Bootstrap (Core Layer)
**Scene:** `00_Bootstrap.unity`

**Responsabilità**
- Imposta `targetFrameRate` e `vSync`.
- Chiama `ServiceRegistry.InstallDefaults()` (Logger, EventBus, Config, Save, UI, Content, StateMachine…).
- Inizializza i servizi base (`ConfigService`, ecc.).
- Carica la scena principale `01_Menu` in modalità **Single**.
- Aggiunge le scene additive di sistema:
  - `Scenes/Additive/91_UI_Root`
  - `Scenes/Additive/90_Systems_Audio`

> Tutti i sistemi globali (UIRoot, AudioManager, ecc.) restano attivi per tutto il ciclo di vita dell’app.

---

## 🧩 Game Layer – Initial State
**Script:** `InitialStateInstaller.cs`  
**Presente in:** `01_Menu.unity`

Responsabile di impostare lo stato iniziale del gioco:
```csharp
await ServiceRegistry
    .Resolve<IGameStateMachine>()
    .ChangeStateAsync(new MainMenuState());
```

> Questo script appartiene al layer “Game” (asmdef `Game.Features`)
> e quindi può vedere gli stati concreti (`MainMenuState`, `Game2DState`, `Game3DState`, …).

---

## 🗺️ Scene Hierarchy Summary

| Tipo | Scena | Caricamento | Descrizione |
|------|--------|-------------|--------------|
| Core | 00_Bootstrap | **Single** | Entry point dell’app; avvia tutti i servizi. |
| Gameplay | 01_Menu | **Single** | Contiene l’installer iniziale e la UI del menu. |
| System | 91_UI_Root | **Additive** | Canvas principale, `Screen Space – Overlay`. |
| System | 90_Systems_Audio | **Additive** | Mixer e sistemi audio condivisi. |

---

## 💡 Best Practices
- Tutti i sistemi globali vivono in scene additive sotto `00_Bootstrap`.
- Gli stati (`MainMenuState`, `GameplayState`, ecc.) **non** caricano le additive.
- Le scene di gameplay (`02_Game_2D`, `02_Game_3D`) vanno caricate **Single**.
- Evitare `DontDestroyOnLoad` nei singoli manager: centralizza tutto in `00_Bootstrap`.

---

## 🧱 File principali

| Scopo | File | Namespace |
|-------|------|-----------|
| Entry point | `GameBootstrap.cs` | `hp55games.Mobile.Core.Bootstrap` |
| Service container | `ServiceRegistry.cs` | `hp55games.Mobile.Core.Architecture` |
| Config | `ConfigService.cs` | `hp55games.Mobile.Core.Architecture` |
| State machine | `GameStateMachine.cs` | `hp55games.Mobile.Core.Architecture.States` |
| UI Root prefab | `UIRoot.prefab` | — |
| Audio systems | `90_Systems_Audio.unity` | — |

---

## ✅ TODO futuri
- [ ] Aggiungere prefab camera di default per 2D e 3D.  
- [ ] Passare a `Addressables.InstantiateAsync` per UI dinamica.  
- [ ] Automatizzare la creazione delle scene base via Setup Wizard.

---
