using UnityEngine;

namespace hp55games.Mobile.Game.FlappyTest
{
    public sealed class TestDeathZone : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            var controller = FindObjectOfType<FlappyGameController>();
            if (controller != null)
                controller.OnPlayerDeath();
        }
    }
}