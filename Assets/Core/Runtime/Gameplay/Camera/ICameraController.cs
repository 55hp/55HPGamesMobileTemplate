using UnityEngine;

namespace hp55games.Mobile.Core.Gameplay.Camera
{
    /// <summary>
    /// Contract for a follow-camera rig. Kept separate from the Juice feedback pipeline —
    /// Shake() is a thin pass-through to a CameraShakeFeedback, not a reimplementation — so
    /// gameplay code has one stable seam to ask "follow this" / "shake" regardless of which
    /// smoothing/shake implementation a project wires up.
    /// </summary>
    public interface ICameraController
    {
        Transform Target { get; }

        Vector3 Offset { get; set; }

        float SmoothSpeed { get; set; }

        void SetTarget(Transform target);

        void Shake();

        void SnapToTarget();
    }
}
