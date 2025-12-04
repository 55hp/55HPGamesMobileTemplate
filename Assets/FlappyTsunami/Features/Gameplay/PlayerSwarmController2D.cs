using System.Collections.Generic;
using UnityEngine;

namespace hp55games.FlappyTsunami.Features.Gameplay
{
    public class PlayerSwarmController2D : MonoBehaviour
    {
        [Header("Followers")]
        [SerializeField] private List<FollowerUnit2D> followers = new List<FollowerUnit2D>();

        [Header("Movement")]
        [SerializeField] private float tapImpulse = 5f;

        private void Update()
        {
            // TEMP: input grezzo Unity. Più avanti lo sostituiremo con IInputService del template.
            if (Input.GetMouseButtonDown(0))
            {
                BroadcastTap();
            }
        }

        private void BroadcastTap()
        {
            Vector2 baseImpulse = Vector2.up * tapImpulse;

            foreach (var follower in followers)
            {
                if (follower != null && follower.IsAlive)
                {
                    follower.OnSwarmTap(baseImpulse);
                }
            }
        }

        /// <summary>
        /// Chiamato da un FollowerUnit2D quando muore.
        /// </summary>
        public void NotifyFollowerDied(FollowerUnit2D unit)
        {
            // Controlla se è rimasto almeno un follower vivo.
            bool anyAlive = false;

            foreach (var follower in followers)
            {
                if (follower != null && follower.IsAlive)
                {
                    anyAlive = true;
                    break;
                }
            }

            if (!anyAlive)
            {
                // TODO: qui più avanti scatterà il GAME OVER (event, FSM, ecc.)
                Debug.Log("[PlayerSwarmController2D] Tutti i follower sono morti -> Game Over.");
            }
        }

        // Opzionale: per debug rapido, così puoi vedere quanti sono vivi.
        public int GetAliveFollowersCount()
        {
            int count = 0;

            foreach (var follower in followers)
            {
                if (follower != null && follower.IsAlive)
                    count++;
            }

            return count;
        }
    }
}
