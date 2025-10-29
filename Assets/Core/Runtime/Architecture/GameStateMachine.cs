using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace hp55games.Mobile.Core.Architecture.States
{
    public sealed class GameStateMachine
    {
        // Singleton leggero (se già esiste in scena, rimuovi o adatta)
        private static GameStateMachine _instance;
        public static GameStateMachine Instance => _instance ??= new GameStateMachine();

        private readonly object _lock = new();
        private IGameState _current;
        private bool _isTransitioning;
        private CancellationTokenSource _cts;

        /// <summary>Stato attuale (può essere null all’avvio).</summary>
        public IGameState Current => _current;

        /// <summary>
        /// Cambia stato in modo sicuro. Se una transizione è in corso, quella nuova attende.
        /// </summary>
        public async Task ChangeStateAsync(IGameState next)
        {
            if (next == null) throw new ArgumentNullException(nameof(next));

            // Garantiamo una sola transizione alla volta
            lock (_lock)
            {
                if (_isTransitioning) throw new InvalidOperationException("State transition already in progress.");
                _isTransitioning = true;

                // cancella eventuale ciclo dello stato precedente
                _cts?.Cancel();
                _cts = new CancellationTokenSource();
            }

            try
            {
                var ct = _cts.Token;

                // 1) Exit del precedente
                if (_current != null)
                {
                    try { await _current.ExitAsync(ct); }
                    catch (OperationCanceledException) { /* ok */ }
                    catch (Exception ex) { Debug.LogException(ex); }
                }

                // 2) Enter del nuovo
                _current = next;
                try { await _current.EnterAsync(ct); }
                catch (OperationCanceledException) { /* ok */ }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    // rollback in caso di fallimento? a discrezione:
                    // _current = null;
                }
            }
            finally
            {
                lock (_lock)
                {
                    _isTransitioning = false;
                }
            }
        }

        /// <summary>Annulla lo stato corrente (utile a fine gioco o reload completo).</summary>
        public void CancelCurrent()
        {
            _cts?.Cancel();
        }
    }
}
