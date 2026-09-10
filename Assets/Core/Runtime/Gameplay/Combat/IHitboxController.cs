using System;
using UnityEngine;

namespace hp55games.Mobile.Core.Gameplay.Combat
{
    /// <summary>
    /// Toggles a hitbox on/off and reports contacts for both physics pipelines.
    /// No MonoBehaviour implementation ships in the template — concrete projects
    /// implement this against whichever collider(s) their hitbox uses.
    /// </summary>
    public interface IHitboxController
    {
        void Activate();
        void Deactivate();

        event Action<Collider> OnHitDetected;
        event Action<Collider2D> OnHitDetected2D;
    }
}
