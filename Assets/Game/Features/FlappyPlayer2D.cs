using System.Collections;
using UnityEngine;
using hp55games.Mobile.Core.InputSystem;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.Gameplay.Events;

namespace hp55games.Mobile.Game.FlappyTest
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class FlappyPlayer2D : MonoBehaviour
    {
        [Header("Jump Settings")]
        [SerializeField] private float _jumpForce = 6f;

        private IInputService _input;
        private IEventBus _bus;
        private Rigidbody2D _rb;

        private bool _isDead;

        private System.IDisposable _deathSub;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _isDead = true;
            ServiceRegistry.TryResolve(out _input);
            ServiceRegistry.TryResolve(out _bus);

            // Smette di rispondere ai comandi quando muore
            if (_bus != null)
            {
                _deathSub = _bus.Subscribe<PlayerDeathEvent>(_ => OnPlayerDeath());
            }
        }

        private void Start()
        {
            StartCoroutine(EnableGravityDelayed());
        }

        private void OnDestroy()
        {
            _deathSub?.Dispose();
        }

        private void OnEnable()
        {
            if (_input != null)
            {
                // Ascoltiamo SOLO Tap → perfetto per Flappy
                _input.Tap += OnTap;
            }
        }

        private void OnDisable()
        {
            if (_input != null)
            {
                _input.Tap -= OnTap;
            }
            StopAllCoroutines();
        }

        private IEnumerator EnableGravityDelayed()
        {
            yield return new WaitForSeconds(3);
            _rb.gravityScale = 1f;
            _isDead = false;
        }

        private void OnTap(Vector2 screenPos)
        {
            if (_isDead)
                return;

            Jump();
        }

        private void Jump()
        {
            // Reset velocità verticale per avere un salto “secco”
            Vector2 vel = _rb.velocity;
            vel.y = 0;
            _rb.velocity = vel;

            // Forza verso l'alto
            _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }

        private void OnPlayerDeath()
        {
            _isDead = true;
            _rb.velocity = Vector2.zero;
        }
    }
}
