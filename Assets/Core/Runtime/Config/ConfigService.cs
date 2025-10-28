using UnityEngine;

namespace hp55games.Mobile.Core.Architecture
{
    public interface IConfigService
    {
        hp55games.Mobile.Core.Config.GameConfig Current { get; }
    }

    public sealed class ConfigService : IConfigService
    {
        public hp55games.Mobile.Core.Config.GameConfig Current { get; private set; }

        // Per ora referenza serializzata da Inspector, niente Resources
        public ConfigService(hp55games.Mobile.Core.Config.GameConfig cfg)
        {
            if (cfg == null)
            {
                Debug.LogError("[ConfigService] GameConfig mancante.");
            }
            Current = cfg;
        }
    }
}