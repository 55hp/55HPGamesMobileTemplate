using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.SceneFlow;

namespace hp55games.Mobile.Game.SceneFlow
{
    /// <summary>
    /// Registers the SceneFlowService when the menu scene loads.
    /// </summary>
    public sealed class SceneFlowServiceInstaller : MonoBehaviour
    {
        private void Awake()
        {
            ServiceRegistry.Register<ISceneFlowService>(new SceneFlowService());
            Debug.Log("[SceneFlow] SceneFlowService registered.");
        }
    }
}