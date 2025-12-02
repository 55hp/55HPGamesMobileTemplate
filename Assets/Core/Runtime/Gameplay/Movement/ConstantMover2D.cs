using UnityEngine;

namespace hp55games.Mobile.Core.Gameplay
{
    /// <summary>
    /// Simple constant mover, ideal for pipes, bullets, obstacles, etc.
    /// </summary>
    public sealed class ConstantMover2D : MonoBehaviour
    {
        [SerializeField] private Vector2 _speed = new Vector2(-5f, 0f);
        [SerializeField] private Space _space = Space.World;

        private void Update()
        {
            var delta = _speed * UnityEngine.Time.deltaTime;
            if (_space == Space.World)
                transform.Translate(delta.x, delta.y, 0f, Space.World);
            else
                transform.Translate(delta.x, delta.y, 0f, Space.Self);
        }

        public void SetSpeed(Vector2 speed) => _speed = speed;
    }
}