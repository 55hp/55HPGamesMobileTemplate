# 🎨 UI & Audio Architecture — Complete Guide

*55HP Games – Unity Mobile Template*

This document describes how the **UI System** and the **Audio System** work together inside the template.

It explains:

- UI structure and layers
- UI loading via Addressables
- Navigation (Pages)
- Popups
- Overlays (Fade & Loading)
- Toast notifications
- Options system (volume, mute, haptics, language)
- Audio Mixer integration
- Music Service (crossfade BGM)
- Setup requirements for UI and Audio scenes

---

# 🧱 **1. UI Root Architecture**

The UI Root lives in the additive scene:

```
Scenes/Additive/91_UI_Root

```

The structure:

```
UIRoot (UIRoot.cs)
  └─ Canvas
       ├─ Layer_HUD
       ├─ Layer_Pages
       ├─ Layer_Popups
       ├─ Layer_Overlay
       └─ Layer_Toasts

```

### UIRoot responsibilities

`UIRoot.cs` exposes the layer transforms so all UI services know **exactly where** to place elements.

Each service works exclusively inside its own layer:

- Pages → Layer_Pages
- Popups → Layer_Popups
- Toasts → Layer_Toasts
- Overlays → Layer_Overlay
- HUD widgets → Layer_HUD

This ensures perfect layering and no conflicts between systems.

---

# 📦 **2. UI Loading (Addressables)**

All UI elements are loaded using Addressables.

### Recommended Addressable folder structure:

```
content/ui/pages/<PageName>
content/ui/popups/<PopupName>
content/ui/toasts/<ToastName>
content/ui/overlays/<OverlayName>

```

Each prefab MUST be marked as Addressable.

### Example (Pages)

```
content/ui/pages/Main_Menu_Generic
content/ui/pages/Settings
content/ui/pages/LevelSelect

```

### Example (Popups)

```
content/ui/popups/Popup_Generic
content/ui/popups/Popup_Confirm

```

---

# 🧭 **3. Navigation Service (Pages)**

### Interface

```csharp
public interface IUINavigationService
{
    Task PushAsync(string address);
    Task PopAsync();
}

```

### Responsibilities

- Loads UI Pages (Addressable prefabs)
- Push = open new page (added to navigation stack)
- Pop = close current page and return to previous

### Example Usage

```csharp
var nav = ServiceRegistry.Resolve<IUINavigationService>();
await nav.PushAsync(Addr.Content.UI.Pages.Main_Menu_Generic);

```

### Result

The page prefab is instantiated under:

```
Layer_Pages

```

and managed by the page stack.

---

# 🪟 **4. Popup Service (Modal UI)**

### Interface

```csharp
Task OpenAsync(string address);
Task CloseLastAsync();
Task CloseAllAsync();

```

### Features

- Modal behavior (blocks input behind)
- Automatic scrim creation
- Clicking the scrim closes the top popup

### Example

```csharp
var popup = ServiceRegistry.Resolve<IUIPopupService>();
await popup.OpenAsync(Addr.Content.UI.Popups.Popup_Generic);

```

Popups are instantiated under:

```
Layer_Popups

```

---

# 🌫 **5. Overlay Service (Fade & Loading)**

Overlays are full-screen UI elements for:

- fade-in / fade-out transitions
- loading screen
- blocking input during scene transitions

### Interface

```csharp
Task FadeIn(float duration);
Task FadeOut(float duration);
Task ShowLoading();
Task HideLoading();

```

### Usage

```csharp
var overlay = ServiceRegistry.Resolve<IUIOverlayService>();
await overlay.FadeIn(0.3f);

```

Elements appear under:

```
Layer_Overlay

```

---

# 🔔 **6. Toast Service (Notifications)**

Quick HUD-style notifications like:

- "+10 Coins"
- "Level Up!"
- "New Item Unlocked"

### Interface

```csharp
Task ShowAsync(string address, string message, float duration = 2f);

```

### Example

```csharp
var toast = ServiceRegistry.Resolve<IUIToastService>();
await toast.ShowAsync(Addr.Content.UI.Toasts.Toast_Simple, "Achievement Unlocked!", 1.2f);

```

Toasts appear under:

```
Layer_Toasts

```

and automatically destroy/return to pool after duration expires.

---

# 🎚 **7. Options System (Audio, Haptics, Language)**

The template includes a complete, persistent **Options/Settings** service:

### Interface

```csharp
float MusicVolume { get; set; }
float SfxVolume { get; set; }
bool MusicMute { get; set; }
bool SfxMute { get; set; }
bool Haptics { get; set; }
string Language { get; set; }

void Load();
void Save();
void ResetToDefaults();
event Action Changed;

```

### Storage

Options are saved through `ISaveService` into the JSON save file.

### UI Binding

Handled by `UIOptionsPage.cs`, which synchronizes:

- sliders
- toggles
- dropdown language
- mute switches

### Automatic Audio Binding

`AudioOptionsBinder.cs` connects Options → AudioMixer.

---

# 🎧 **8. Audio System (Mixer + Settings)**

The scene:

```
Scenes/Additive/90_Systems_Audio

```

contains:

```
MusicPlayer
 ├─ AudioSource A
 └─ AudioSource B
AudioOptionsBinder
Audio Mixer (Music / SFX groups)

```

### Responsibilities

- Handles BGM smoothly (crossfade)
- Applies volume/mute settings
- Routes SFX through SFX mixer group
- Keeps audio globally consistent

---

# 🎵 **9. Music Service (Crossfade BGM)**

The `IMusicService` allows:

- play track
- crossfade to new track
- stop track

### Interface

```csharp
Task PlayAsync(string address, float fadeIn = 0.5f);
Task CrossfadeToAsync(string address, float duration = 0.75f);
Task StopAsync(float fadeOut = 0.5f);

```

### Example (in a GameState)

```csharp
_music.CrossfadeToAsync(
    Addr.Content.Audio.Bgm.MenuTheme,
    0.5f
);

```

### Addressables convention

```
content/audio/bgm/<TrackName>

```

### Required Setup

`MusicPlayer` in `90_Systems_Audio` must have:

- Two AudioSources
- Both looping
- Both playOnAwake = false

---

# 🔧 **10. UI Initialization Sequence**

The UI is ready only after:

1. `91_UI_Root` is loaded
2. `UIServiceInstaller.Awake()` registers UI services
3. `UIRuntime.MarkServicesReady()` is called

The bootstrap waits for this flag before launching the first state.

```csharp
while (!UIRuntime.ServicesReady)
    await Task.Yield();

```

This ensures:

- NavigationService is ready
- PopupService is ready
- OverlayService is ready
- ToastService is ready
- MusicService is ready

No timing issues, no race conditions.

---

# 🧪 **11. Creating New UI Elements**

### Pages

1. Create a prefab under:
    
    ```
    Assets/Game/UI/Pages/<Name>
    
    ```
    
2. Mark it as Addressable:
    
    ```
    content/ui/pages/<Name>
    
    ```
    
3. Load it via:
    
    ```csharp
    await _nav.PushAsync(Addr.Content.UI.Pages.<Name>);
    
    ```
    

### Popups

Same process, but under:

```
content/ui/popups/<Name>

```

### Toasts

Under:

```
content/ui/toasts/<Name>

```

### Overlays

Under:

```
content/ui/overlays/<Name>

```

---

# 🧭 **12. UI + FSM — Typical Flow**

Example: **Menu → Gameplay**

```csharp
await _music.CrossfadeToAsync(Addr.Content.Audio.Bgm.Menu, 0.5f);

await _nav.PushAsync(Addr.Content.UI.Pages.Main_Menu_Generic);

if (playButtonPressed)
{
    await fsm.ChangeStateAsync(new GameplayState());
}

```

In `GameplayState`:

```csharp
await _overlay.FadeIn(0.3f);
await LoadGameplayScene();
await _overlay.FadeOut(0.3f);

```

---

# 🏁 **13. Summary**

The UI + Audio architecture provides:

- Clean scene layering
- Async-safe state transitions
- Centralized UI services
- Smooth BGM transitions
- Persistent user settings
- Full Addressables workflow
- Modular, reusable UI prefabs
- Extensible audio & UI pipeline

This system is designed to work across:

- Hypercasual
- Casual
- Mid-core
- UI-driven games
- Narrative-based titles

You now have a **complete and documented UI & Audio foundation** to build any mobile game on top of it.