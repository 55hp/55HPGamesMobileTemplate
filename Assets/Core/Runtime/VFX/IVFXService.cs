using UnityEngine;

namespace hp55games.Mobile.Core.VFX
{
    /// <summary>
    /// Contract for playing pooled VFX. Per the Notion plan, this consumes IObjectPoolService
    /// internally — a concrete implementation resolves it via
    /// ServiceRegistry.Resolve&lt;IObjectPoolService&gt;() and calls Get/Release on
    /// VFXData.Prefab. That dependency is a stated integration point, not hidden behind this
    /// interface. Pure interface, no base class — VFX playback (particle systems, shaders,
    /// Timeline-driven effects) varies too much per project to share a MonoBehaviour skeleton.
    /// </summary>
    public interface IVFXService
    {
        void PlayEffect(VFXData data, Vector3 position, Quaternion rotation);

        void PlayEffectAttached(VFXData data, Transform parent);

        /// <summary>Only meaningful for looping/persistent effects. Most VFX are one-shot
        /// particle bursts that self-terminate — StopEffect is a no-op for those by design,
        /// not a missing implementation.</summary>
        void StopEffect(VFXData data);
    }
}
