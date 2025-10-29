using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace hp55games.Mobile.Core.Architecture.States
{
    public sealed class GameStateMachine : IGameStateMachine
    {
        private readonly object _lock = new();
        private IGameState _current;
        private bool _isTransitioning;
        private CancellationTokenSource _cts;

        public IGameState Current => _current;

        public async Task ChangeStateAsync(IGameState next)
        {
            if (next == null) throw new ArgumentNullException(nameof(next));

            lock (_lock)
            {
                if (_isTransitioning) throw new InvalidOperationException("State transition already in progress.");
                _isTransitioning = true;
                _cts?.Cancel();
                _cts = new CancellationTokenSource();
            }

            try
            {
                var ct = _cts.Token;

                if (_current != null)
                {
                    try { await _current.ExitAsync(ct); }
                    catch (OperationCanceledException) { }
                    catch (Exception ex) { Debug.LogException(ex); }
                }

                _current = next;

                try { await _current.EnterAsync(ct); }
                catch (OperationCanceledException) { }
                catch (Exception ex) { Debug.LogException(ex); }
            }
            finally
            {
                lock (_lock) { _isTransitioning = false; }
            }
        }

        public void CancelCurrent() => _cts?.Cancel();
    }
}