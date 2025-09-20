using UnityEngine;


namespace AF55HP.Mobile.Core.Config
{
    [CreateAssetMenu(menuName="Config/GameConfig")]
    public class GameConfig : ScriptableObject {
        public string appVersion;
        public bool enableHaptics = true;
        public int defaultDifficulty = 1;
    }
}

namespace AF55HP.Mobile.Core.Architecture
{
    public interface IConfigService
    {
        AF55HP.Mobile.Core.Config.GameConfig Current { get; }
    }

    public sealed class LocalConfigService : IConfigService
    {
        public AF55HP.Mobile.Core.Config.GameConfig Current { get; private set; }
        public LocalConfigService()
        {
            // Metti il tuo asset in: Assets/Resources/Configs/GameConfig.asset
            Current = Resources.Load<AF55HP.Mobile.Core.Config.GameConfig>("Configs/GameConfig");
            if (Current == null)
            {
                Debug.LogWarning("GameConfig non trovato in Resources/Configs/. Creane uno dal menu.");
                Current = ScriptableObject.CreateInstance<AF55HP.Mobile.Core.Config.GameConfig>();
            }
        }
    }
}