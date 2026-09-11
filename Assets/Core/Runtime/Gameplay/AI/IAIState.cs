namespace hp55games.Mobile.Core.Gameplay.AI
{
    /// <summary>
    /// A single state in a per-entity AI state machine (patrol, chase, attack, ...).
    /// Synchronous and per-frame-ticked by design: AI behavior reacts to the world
    /// every frame, unlike the game-level FSM (Architecture.States), which is async
    /// and only transitions on explicit, infrequent triggers (menu/gameplay/pause/result).
    /// </summary>
    public interface IAIState
    {
        void Enter();

        void Tick(float deltaTime);

        void Exit();
    }
}
