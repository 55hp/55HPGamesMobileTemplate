using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Game.Events;

namespace hp55games.Mobile.Game.FlappyTest
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class PipeScoreZone : MonoBehaviour
    {
        [SerializeField] private LayerMask playerLayer;

        private IEventBus _bus;
        private bool _triggered;

        private void Awake()
        {
            ServiceRegistry.TryResolve(out _bus);
        }

        private void OnEnable()
        {
            _triggered = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_triggered)
                return;

            if (!IsPlayer(other.gameObject))
                return;

            _triggered = true;

            _bus?.Publish(new PipePassedEvent());
        }

        private bool IsPlayer(GameObject go)
        {
            return (playerLayer.value & (1 << go.layer)) != 0;
        }
    }
}