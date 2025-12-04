using UnityEngine;

namespace hp55games.FlappyTsunami.Features.Gameplay
{
    /// <summary>
    /// Qualsiasi FollowerUnit2D che entra in questo trigger viene ucciso.
    /// Può essere usato come dead zone superiore o inferiore.
    /// </summary>
    public class DeadZone2D : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Se l'oggetto che entra è un follower, lo uccidiamo.
            if (other.TryGetComponent(out FollowerUnit2D follower))
            {
                follower.Kill();
            }
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.TryGetComponent(out FollowerUnit2D follower))
            {
                follower.Kill();
            }
        }
    }
}