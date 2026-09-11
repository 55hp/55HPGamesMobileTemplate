using System;
using UnityEngine;

namespace hp55games.Mobile.Core.Gameplay.AI
{
    /// <summary>
    /// Base per-entity AI state machine. Ticks CurrentState every frame via Update() using
    /// Time.deltaTime directly rather than ITimeService: AI behavior is tied to the same
    /// per-frame cadence as Unity's own physics/animation loop, not to a service-resolved
    /// abstraction meant for gameplay-timer/pause concerns elsewhere in the template.
    /// </summary>
    public abstract class AIStateMachineBase : MonoBehaviour, IAIStateMachine
    {
        private IAIState _currentState;

        public IAIState CurrentState => _currentState;

        public event Action<IAIState> OnStateChanged;

        public void ChangeState(IAIState next)
        {
            if (next == null || next == _currentState)
                return;

            _currentState?.Exit();
            _currentState = next;
            _currentState.Enter();

            OnStateChanged?.Invoke(_currentState);
        }

        protected virtual void Update()
        {
            _currentState?.Tick(Time.deltaTime);
        }
    }
}
