using System;

namespace hp55games.Mobile.Core.Gameplay.Combat
{
    /// <summary>
    /// Drives an entity's attack flow. No MonoBehaviour implementation ships in the
    /// template — concrete projects implement this against their own animation/input stack.
    /// </summary>
    public interface ICombatController
    {
        bool CanAttack { get; }

        void PerformAttack(AttackData data);
        void CancelCurrentAction();

        event Action<AttackData> OnAttackStarted;
        event Action OnAttackFinished;
    }
}
