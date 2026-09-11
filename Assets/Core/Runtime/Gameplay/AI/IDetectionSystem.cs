using System;
using UnityEngine;

namespace hp55games.Mobile.Core.Gameplay.AI
{
    /// <summary>
    /// Line-of-sight + field-of-view target detection for AI entities. Decoupled from
    /// IAIStateMachine: a state (e.g. Patrol) reacts to OnTargetDetected/OnTargetLost to
    /// decide its own transitions, rather than this system owning any state logic itself.
    /// </summary>
    public interface IDetectionSystem
    {
        bool CanSeeTarget(Transform target);

        float DetectionRadius { get; }

        float FieldOfViewAngle { get; }

        event Action<Transform> OnTargetDetected;

        event Action<Transform> OnTargetLost;
    }
}
