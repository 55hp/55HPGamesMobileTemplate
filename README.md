🧱 55HP Unity Mobile Template – Core Template Overview v1

A Universal Mobile Starter Framework for Unity 2022 LTS+

📌 1. Mission & Philosophy

The 55HP Unity Mobile Template is a practical, flexible and production-ready starter framework designed to support 99% of mobile games, from hypercasual to midcore roguelites (e.g., Slay the Spire–like), as well as offline single-player experiences.

The goal is simple:

Avoid rewriting the same architecture for every new project.
Enable fast prototyping AND stable production.
Stay simple, readable, and game-agnostic.

This template is tailored for a solo dev / small-team workflow, especially one that values clarity, modularity, and the ability to integrate features back into the template over time.

Core principles:

Practical > Perfect — Real-world utility over academic purity.

Simple > Clever — Clarity and readability come first.

Reusable > Specific — Avoid game-specific logic inside the core.

Extensible > Rigid — Easy to customize for multiple genres.

Consistent Architecture — Always the same bootstrap, UI, audio, services, and flow.

📍 2. High-Level Architecture

The template is built around:

Additive scene structure

Async Finite State Machine (FSM) controlling high-level game flow

Service Registry (simple DI)

Save, Time, Config, Localization services

Universal UI Root (navigation, popups, overlays, toasts)

Audio system (mixer, BGM crossfade, SFX routing)

Addressables loader (centralized asset/content loading)

Object pooling (lightweight, highly reusable)

Input Service (generic pointer/touch gestures)

Game Context Service (runtime session info)

The template boot sequence always follows this flow:

00_Bootstrap (startup)
 ├─ Install Services
 ├─ Load Additive Scenes:
 │    90_Systems_Audio
 │    91_UI_Root
 ├─ Load Initial Game Scene (01_Menu)
 └─ Enter MainMenuState (FSM)

🧩 3. Service Registry

A lightweight service-locator providing:

Global registration at startup

Resolve<T>() for runtime access

No hidden magic, no external dependencies

Core Services Registered:

ILog (UnityLog)

IEventBus

ISaveService

ITimeService

IConfigService

ILocalizationService

IUIOptionsService

IContentLoader

IGameStateMachine

IObjectPoolService

IUINavigationService

IUIPopupService

IUIOverlayService

IUIToastService

IGameContextService

IInputService

🎮 4. Async Game State Machine (FSM)

A robust async FSM manages high-level game flow.

Each state implements:

Task EnterAsync(CancellationToken ct);
Task ExitAsync(CancellationToken ct);


Core states included:

MainMenuState

GameplayState

PauseState

ResultsState

LoadingState

States are pure logic orchestration:
no monobehaviour, no heavy dependencies, and interchangeable across games.

🗺️ 5. Scene Architecture
Required Scenes:

00_Bootstrap (entrypoint)

90_Systems_Audio (BGM, SFX, Input driver)

91_UI_Root (main UI system)

01_Menu

02_Gameplay

03_Results

SceneFlowService

Handles transitions:

fade-in/out using UIOverlay

asynchronous loading

FSM state changes

safe transitions between Menu → Gameplay → Results

Supports expandability (e.g., more states or scenes).

🖥️ 6. UI Architecture (Complete, Unified)

A powerful but simple UI system supporting:

Screens (pages) via Navigation service

Popups (Addressables instantiable)

Toasts

Overlays (fade, loading, input blocker)

Localization integration

Options integration (audio, language, haptics)

UI Root Responsibilities:

Controller for:

Navigation stack

Popup stack

Toast queue

Overlay panel

Global event binding for:

Language change

Options change

Initialization of UI layers and containers

UI Services:

IUINavigationService

IUIPopupService

IUIOverlayService

IUIToastService

Addressables paths follow structure:

ui/screens/...
ui/popups/...
ui/elements/...


This system works for:

hypercasual menus

card-game interfaces

RPG screens

shop / inventory UIs

roguelike map UIs

cinematic overlays

🔊 7. Audio Architecture (Complete)
Core Systems:

90_Systems_Audio contains:

AudioMixer (Groups: Master, Music, SFX)

MusicPlayer with crossfade

Sound playback helper

Features:

Crossfade BGM (automatic or manual)

Mute/Volume tied to UIOptionsService

Addressables for loading music & SFX assets

Haptics support integrated via UIOptions

This architecture is enough to support:

hypercasual loops

roguelite ambience layers

card-game UI SFX

SFX bursts with pooling

adaptive music systems (future-ready)

💾 8. Save System (JSON)

Simple, predictable, file-based save:

Persistent path: save.json

Classes:

SaveData

OptionsData

TimeStampEntry

Includes:

coins

profile

options (music, sfx, hapt, lang, mute)

timestamps (for cooldowns, login times, etc.)

⏳ 9. Time Service

Provides:

UtcNow, LocalNow

monotonic time (Time.realtimeSinceStartupAsDouble)

timestamp logic:

cooldown

time since last

daily checks

clock-back detection (anti-cheat safeguard)

⚙️ 10. Config Service

Loads GameConfig from Addressables path:

hp55games.Addr.Config.Main


Allows:

game version

default difficulty

default haptics

any future config values

📦 11. Content Loader (Addressables)

Uniform entry point for Addressables:

Load<T>(address)

InstantiateAsync(address)

Release(instance)

ReleaseAsset(asset)

Ensures consistent behavior across UI, FX, gameplay.

🔁 12. Object Pooling

Global pooling system:

prefab → queue of inactive instances

instance → original prefab

Spawn() and Despawn()

avoids GC allocs

supports UI, FX, bullets, VFX bursts

🎮 13. Input Service (NEW, Core)

A generic pointer/touch abstraction with these events:

PointerDown

PointerUp

Tap

Hold

Swipe

Driven by InputServiceDriver placed in:

90_Systems_Audio


Supports:

hypercasual gesture gameplay

roguelike grid interaction

card games

puzzle/tap games

offline single-player interactions

No gameplay specifics inside:
just pure gesture abstraction.

🌐 14. Game Context Service (NEW, Core)

Stores runtime-only information:

ProfileId

CurrentRunSeed

CurrentLevelId

IsDebug

ResetRun()

Useful for:

procedural games (Map Game, roguelikes)

games with sessions/runs

multi-profile setups

debugging helpers

avoiding global statics or SaveService abuse

Not persisted automatically; you decide what to save.

🧩 15. Extending the Core

Patterns:

Add new states → register via FSM

Add new services → ServiceRegistry.Register<T>

Add new UI screens → Addressables + UI Navigation

Create new reusable features during games → backport to template in branches

📁 16. Recommended Folder Structure
Assets/
  Core/
    Architecture/
    FSM/
    Save/
    Time/
    Config/
    Context/
    Input/
    UI/
      Navigation/
      Popup/
      Overlay/
      Toast/
      UIRoot/
    Audio/
    Addressables/
    Pool/
  Content/ (game-specific)
  UI_Resources/ (localization)
  Scenes/
    00_Bootstrap
    90_Systems_Audio
    91_UI_Root
    01_Menu
    02_Gameplay
    03_Results

📝 17. Best Practices

Keep Core free from gameplay logic

Keep UI generic

Use Addressables for everything non-code

Prefer async (FSM, scene flow, loading)

Keep SaveData small and meaningful

Use GameContext to avoid global chaos

Keep InputService minimal but consistent

Always test states with cancellation tokens

📌 18. Summary

The 55HP Unity Mobile Template is a fully-featured, production-ready foundation to build any mobile game.
It includes every necessary architectural system:

Bootstrap

Services

FSM

UI

Audio

Save

Time

Context

Input

Pooling

Addressables

No overengineering.
No coupling with game-specific logic.
Just a clean, extensible, universal mobile foundation.

🎉 Ready to Build Games

With this template, each new game becomes:

“Clone → Rename → Implement gameplay → Ship.”

And any generic feature you create while building games can be merged back into the template through branches like:

feature/liveops

feature/economy-lite

feature/metaprogression

feature/ads

etc.