using System;
using UnityEngine;

namespace hp55games.Mobile.Core.Gameplay.Movement
{
    /// <summary>
    /// Destination/path-following contract — distinct from ConstantMover's speed-based
    /// "translate forever" contract, so IMover does not retrofit onto ConstantMover/2D.
    /// Implementers know where they're going and when they've arrived; ConstantMover only
    /// knows a velocity and never stops on its own.
    /// </summary>
    public interface IMover
    {
        void SetPath(Vector3[] waypoints);

        void MoveTo(Vector3 destination, float speed);

        bool HasArrived { get; }

        event Action OnDestinationReached;
    }
}
