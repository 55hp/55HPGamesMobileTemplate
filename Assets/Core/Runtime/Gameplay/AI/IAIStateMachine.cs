using System;

namespace hp55games.Mobile.Core.Gameplay.AI
{
    /// <summary>
    /// Drives a single AI entity through a sequence of IAIState instances. Deliberately
    /// synchronous (no Task/CancellationToken) — unlike IGameStateMachine, transitions here
    /// happen many times per second in response to gameplay (e.g. detection losing a target)
    /// and must complete within the same frame that triggered them.
    /// </summary>
    public interface IAIStateMachine
    {
        IAIState CurrentState { get; }

        void ChangeState(IAIState next);

        event Action<IAIState> OnStateChanged;
    }
}
