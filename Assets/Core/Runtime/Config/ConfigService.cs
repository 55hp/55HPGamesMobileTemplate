using UnityEngine;


namespace hp55games.Mobile.Core.Config
{
    [CreateAssetMenu(menuName="Config/GameConfig")]
    public class GameConfig : ScriptableObject {
        public string appVersion;
        public bool enableHaptics = true;
        public int defaultDifficulty = 1;
    }
}

namespace hp55games.Mobile.Core.Architecture
{
    public interface IConfigService
    {
        hp55games.Mobile.Core.Config.GameConfig Current { get; }
    }

    public sealed class ConfigService : IConfigService
    {
        public hp55games.Mobile.Core.Config.GameConfig Current { get; private set; }
        public ConfigService()
        {
            // Metti il tuo asset in: Assets/Resources/Configs/GameConfig.asset
            Current = Resources.Load<hp55games.Mobile.Core.Config.GameConfig>("Configs/GameConfig");
            if (Current == null)
            {
                Debug.LogWarning("GameConfig non trovato in Resources/Configs/. Creane uno dal menu.");
                Current = ScriptableObject.CreateInstance<hp55games.Mobile.Core.Config.GameConfig>();
            }
        }
    }
}