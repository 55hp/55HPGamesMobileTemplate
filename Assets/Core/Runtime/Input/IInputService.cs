using System;
using UnityEngine;

namespace hp55games.Mobile.Core.InputSystem
{
    /// <summary>
    /// High-level input abstraction for mobile / pointer devices.
    /// It only exposes generic gestures, not gameplay-specific actions.
    /// </summary>
    public interface IInputService
    {
        /// <summary>
        /// Fired when the pointer is pressed down (finger down / mouse down).
        /// </summary>
        event Action<Vector2> PointerDown;

        /// <summary>
        /// Fired when the pointer is released (finger up / mouse up).
        /// </summary>
        event Action<Vector2> PointerUp;

        /// <summary>
        /// Fired when a tap is detected (short press, small movement).
        /// </summary>
        event Action<Vector2> Tap;

        /// <summary>
        /// Fired when a swipe is detected (press + move + release, above distance threshold).
        /// </summary>
        event Action<Vector2, Vector2> Swipe; // start, end

        /// <summary>
        /// Fired when the pointer is held down longer than a threshold, without large movement.
        /// </summary>
        event Action<Vector2> Hold;

        /// <summary>
        /// Called once per frame by the driver MonoBehaviour.
        /// You don't use this from gameplay code.
        /// </summary>
        void Tick(float deltaTime);
    }
}