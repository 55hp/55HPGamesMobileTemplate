using UnityEngine;

namespace hp55games.Mobile.Core.Config
{
    [CreateAssetMenu(menuName = "Config/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public string appVersion;
        public bool enableHaptics = true;
        public int defaultDifficulty = 1;
    }
}