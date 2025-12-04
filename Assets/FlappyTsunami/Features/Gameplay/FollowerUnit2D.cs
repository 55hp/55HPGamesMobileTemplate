using System.Collections;
using UnityEngine;

namespace hp55games.FlappyTsunami.Features.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class FollowerUnit2D : MonoBehaviour
    {
        [Header("Swarm")]
        [SerializeField] private PlayerSwarmController2D swarm;

        [Header("Movement")]
        [SerializeField] private float verticalImpulseMultiplier = 1f;
        [SerializeField] private float tapDelay = 0f;

        private Rigidbody2D _rb;
        private bool _isAlive = true;

        public bool IsAlive => _isAlive;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        /// <summary>
        /// Chiamato dallo swarm quando il giocatore fa TAP.
        /// </summary>
        public void OnSwarmTap(Vector2 baseImpulse)
        {
            if (!_isAlive)
                return;

            StartCoroutine(ApplyTapDelayed(baseImpulse));
        }

        private IEnumerator ApplyTapDelayed(Vector2 baseImpulse)
        {
            if (tapDelay > 0f)
                yield return new WaitForSeconds(tapDelay);

            // Reset componente verticale prima di applicare l’impulso
            Vector2 velocity = _rb.velocity;
            velocity.y = 0f;
            _rb.velocity = velocity;

            _rb.AddForce(baseImpulse * verticalImpulseMultiplier, ForceMode2D.Impulse);
        }

        /// <summary>
        /// Da chiamare quando questo follower "muore" (collisione, ecc.).
        /// </summary>
        public void Kill()
        {
            if (!_isAlive)
                return;

            _isAlive = false;

            // Notifica allo swarm che questo follower è morto.
            if (swarm != null)
            {
                swarm.NotifyFollowerDied(this);
            }

            // Per ora semplicemente lo disattiviamo.
            gameObject.SetActive(false);
        }

        // Utility per settare lo swarm via script (se non fatto da Inspector).
        public void SetSwarm(PlayerSwarmController2D newSwarm)
        {
            swarm = newSwarm;
        }
    }
}
