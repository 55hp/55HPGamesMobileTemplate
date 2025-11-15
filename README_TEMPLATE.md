✅ README #1 — README_Template.md

(root of the project — for anyone cloning the template)

📘 hp55games Unity Mobile Template

A modular, production-ready Unity template for mobile games.

This template provides a clean and scalable foundation for mobile game development using Unity 2022.3 LTS, with a strong focus on:

Service-based architecture

Addressables-driven content loading

Modular UI (Pages, Popups, Overlays, Toasts)

Audio system (Mixer + Music crossfading)

Navigation & State Machine

Config + Save System

Scene bootstrapping via additive loading

The goal is to make it easy to start new mobile projects with minimal setup and consistent quality.

🚀 Getting Started
1. Clone or Copy the Template

Choose either:

Clone the repository

Or copy the entire project folder into your new game

Then rename your project:

Project Settings → Player

Set your new:

Company Name

Product Name

Default Icon / Bundle ID

2. Entry Scene: 00_Bootstrap

Always start the game from:

Assets/Scenes/00_Bootstrap.unity


This scene:

Initializes core services

Loads:

90_Systems_Audio

91_UI_Root

The initial game scene (01_Menu)

Starts the State Machine

Never start the game from 01_Menu or gameplay scenes.

🗂 Scene Structure

The template uses additive loading.

00_Bootstrap

Entry point

Loads systems/UI

Starts FSM

90_Systems_Audio

Contains:

MusicPlayer (2 AudioSources)

AudioMixer routing

91_UI_Root

Contains:

The root UI Canvas

All UI layers (HUD, Pages, Popups, Toasts, Overlay)

UIServiceInstaller (registers UI services)

01_Menu

Initial playable scene

Runs InitialStateInstaller → MainMenuState

📦 Addressables — Folder & Naming Conventions

All UI + BGM + game content is loaded via Addressables.

UI:
content/ui/pages/<PageName>
content/ui/popups/<PopupName>
content/ui/toasts/<ToastName>
content/ui/overlays/<OverlayName>

Audio:
content/audio/bgm/<TrackName>
content/audio/sfx/<ClipName>

Config:
config/main


All UI prefabs must be Addressable.

🧩 Core Services Overview
Service	Responsibility
IConfigService	Loads game config (ScriptableObject)
ISaveService	JSON save/load
IContentLoader	Addressables wrapper
IGameStateMachine	Async game states
IUIOptionsService	Volumes, mute, haptics, language
IUINavigationService	UI Pages navigation
IUIPopupService	Modal dialogs
IUIOverlayService	Fade, loading
IUIToastService	Toast messages
IMusicService	Crossfaded background music

All are registered automatically by:

ServiceRegistry.InstallDefaults()

UIServiceInstaller (UI services)

🧪 Tests

Tests must live under:

Assets/Tests/


Never place test scripts inside game scenes.

🧱 Creating Your Game

To start a new game using the template:

Duplicate the project

Rename Company/Product in Player Settings

Replace assets in Assets/Game

Add new states (FSM) for your game flow

Add new UI pages/popups under Addressables

Replace BGM tracks with your game’s music

Add gameplay scenes and use the template’s UI/navigation/music system

🎯 Philosophy

This template is built around:

Separation of concerns

High reusability

Minimal coupling between modules

Addressables-first workflow

Mobile-optimized architecture

Async/non-blocking transitions

It is intentionally minimal and production-friendly.

🏁 You’re Ready

Once you understand:

Bootstrap flow

UI services

Music service

Addressables

…you can build a full mobile game on top of this with zero friction.

Happy developing!
— hp55games
