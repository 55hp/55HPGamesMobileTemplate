using System;
using UnityEngine;

namespace hp55games.Mobile.Core.InputSystem
{
    /// <summary>
    /// Default mobile/desktop implementation for IInputService.
    /// It supports single-pointer gestures: tap, swipe, hold.
    /// This class is ticked from a MonoBehaviour driver (InputServiceDriver).
    /// </summary>
    public sealed class InputService : IInputService
    {
        public event Action<Vector2> PointerDown;
        public event Action<Vector2> PointerUp;
        public event Action<Vector2> Tap;
        public event Action<Vector2, Vector2> Swipe;
        public event Action<Vector2> Hold;

        // Tunable thresholds
        const float TapMaxDuration      = 0.25f; // seconds
        const float TapMaxDistanceSqr  = 25f;   // pixels^2 (5 px)
        const float HoldMinDuration    = 0.5f;  // seconds
        const float SwipeMinDistanceSqr = 1600f; // pixels^2 (40 px)

        bool   _isDown;
        bool   _holdFired;
        float  _downTime;
        Vector2 _downPos;
        Vector2 _lastPos;

        public void Tick(float deltaTime)
        {
            // Single-pointer abstraction:
            // - If touch is supported -> use first touch
            // - Else -> use mouse left button as pointer
            bool isPressed;
            Vector2 currentPos;

            if (UnityEngine.Input.touchSupported && UnityEngine.Input.touchCount > 0)
            {
                var t = UnityEngine.Input.GetTouch(0);
                currentPos = t.position;
                isPressed  = t.phase == TouchPhase.Began ||
                             t.phase == TouchPhase.Moved ||
                             t.phase == TouchPhase.Stationary;
            }
            else
            {
                isPressed  = UnityEngine.Input.GetMouseButton(0);
                currentPos = UnityEngine.Input.mousePosition;
            }

            if (isPressed)
            {
                if (!_isDown)
                {
                    // Pointer just pressed
                    _isDown    = true;
                    _holdFired = false;
                    _downTime  = UnityEngine.Time.unscaledTime;
                    _downPos   = currentPos;
                    _lastPos   = currentPos;

                    PointerDown?.Invoke(currentPos);
                }
                else
                {
                    // Pointer is held
                    _lastPos = currentPos;

                    var heldFor = UnityEngine.Time.unscaledTime - _downTime;
                    var distSqr = (currentPos - _downPos).sqrMagnitude;

                    if (!_holdFired && heldFor >= HoldMinDuration && distSqr <= TapMaxDistanceSqr)
                    {
                        _holdFired = true;
                        Hold?.Invoke(currentPos);
                    }
                }
            }
            else
            {
                if (_isDown)
                {
                    // Pointer just released
                    _isDown = false;
                    PointerUp?.Invoke(_lastPos);

                    var totalTime   = UnityEngine.Time.unscaledTime - _downTime;
                    var distanceSqr = (_lastPos - _downPos).sqrMagnitude;

                    if (distanceSqr <= TapMaxDistanceSqr && totalTime <= TapMaxDuration)
                    {
                        Tap?.Invoke(_lastPos);
                    }
                    else if (distanceSqr >= SwipeMinDistanceSqr)
                    {
                        Swipe?.Invoke(_downPos, _lastPos);
                    }
                }
            }
        }
    }
}
