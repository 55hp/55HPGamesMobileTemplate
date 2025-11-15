✅ README #2 — README_UI_Audio.md

(placed inside Assets/Core/Runtime/UI/README_UI_Audio.md)

🎨 UI + 🎵 Audio System — Documentation

hp55games Mobile Template

This document explains how to use:

UI Navigation (Pages)

Popups

Toasts

Overlays (Fade/Loading)

Options (Volumes, mute, language)

Background Music (BGM) crossfade system

Addressables conventions

📂 UI Structure

All UI lives under:

91_UI_Root


The prefab contains:

Canvas

UIRoot script

Layers (RectTransforms)

HUD

Pages

Popups

Toasts

Overlay

The UI uses a service-driven architecture.

🧭 Navigation — Pages
Interface
Task PushAsync(string address);
Task PopAsync();

Usage
var nav = ServiceRegistry.Resolve<IUINavigationService>();
await nav.PushAsync("content/ui/pages/menu_main");


Pages are Addressable prefabs loaded into the Pages layer.

🪟 Popups
Interface
Task OpenAsync(string address);
Task CloseLastAsync();
Task CloseAllAsync();

Usage
var popup = ServiceRegistry.Resolve<IUIPopupService>();
await popup.OpenAsync("content/ui/popups/popup_generic");


Popups include an auto-generated scrim that closes the topmost popup on click.

🌫 Overlays (Fade & Loading)
Interface
Task FadeIn(float duration);
Task FadeOut(float duration);
Task ShowLoading();
Task HideLoading();


Used for transitions and loading screens.

Usage
var overlay = ServiceRegistry.Resolve<IUIOverlayService>();
await overlay.FadeIn(0.3f);

🔔 Toasts
Interface
Task ShowAsync(string address, string message, float duration = 2f);

Usage
var toast = ServiceRegistry.Resolve<IUIToastService>();
await toast.ShowAsync("content/ui/toasts/simple", "Level Up!", 1.2f);

⚙ Options (Volume, Mute, Haptics, Language)
Interface
float MusicVolume { get; set; }
float SfxVolume { get; set; }
bool MusicMute { get; set; }
bool SfxMute { get; set; }
bool Haptics { get; set; }
string Language { get; set; }

void Load();
void Save();
void ResetToDefaults();


Connected to:

Audio Mixer

AudioOptionsBinder

Options Page UI

🎵 Background Music — Crossfade System
Interface
Task PlayAsync(string address, float fadeIn = 0.5f);
Task CrossfadeToAsync(string address, float duration = 0.75f);
Task StopAsync(float fadeOut = 0.5f);

Usage (in states)
var music = ServiceRegistry.Resolve<IMusicService>();
await music.CrossfadeToAsync(Addr.Content.Audio.Bgm.Menu, 0.5f);

Addressable Convention
content/audio/bgm/menu_theme
content/audio/bgm/gameplay_theme

Requirements

Scene 90_Systems_Audio must contain:

MusicPlayer
 ├─ AudioSource (loop ON, playOnAwake OFF)
 └─ AudioSource (loop ON, playOnAwake OFF)

🧱 Addressable Paths — Summary
UI
content/ui/pages/<PageName>
content/ui/popups/<PopupName>
content/ui/toasts/<ToastName>
content/ui/overlays/<OverlayName>

Audio
content/audio/bgm/<TrackName>
content/audio/sfx/<SfxName>

Config
config/main

🔧 UIServiceInstaller

Located in 91_UI_Root.

Registers:

Popups

Navigation

Overlays

Toasts

Options

Music

Do not remove it.

🧪 Testing

All test scripts stay under:

Assets/Tests/


Never add test runners to gameplay scenes.

🎯 Done

With these systems:

Pages

Popups

Toasts

Overlays

Options

Music

…you have a complete, modern UI system ready for any mobile game you build.