using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace hp55games.Mobile.Core.Architecture
{
    public interface IContentLoader
    {
        Task<T> LoadAsync<T>(string address) where T : class;
        void Release<T>(T asset);

        // 👇 nuove API per istanze (consigliate per UI/FX)
        Task<GameObject> InstantiateAsync(string address, Transform parent = null, bool worldPositionStays = false);
        void ReleaseInstance(GameObject instance);
    }

    /// <summary>
    /// Loader basato su Addressables: carica per "address" (stringa).
    /// Esempi di address:
    ///   "content/weapons/table"
    ///   "content/ui/popup_generic"
    /// </summary>
    public sealed class AddressablesContentLoader : IContentLoader
    {
        public async Task<T> LoadAsync<T>(string address) where T : class
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);
            T result = await handle.Task;

            if (result == null)
                Debug.LogError($"[ContentLoader] Asset null. Address='{address}' Tipo={typeof(T).Name}");

            // Nota: restituisco l'oggetto, ma NON rilascio l'handle qui.
            // Addressables gestisce reference-counting tramite l'oggetto; useremo Release(obj) quando non serve più.
            return result;
        }
        
        public async Task<GameObject> InstantiateAsync(string address, Transform parent = null, bool worldPositionStays = false)
        {
            // parent null = instanzia in root; per UI passa il Canvas.transform
            var handle = Addressables.InstantiateAsync(address, parent);
            var instance = await handle.Task;

            if (instance == null)
                Debug.LogError($"[ContentLoader] InstantiateAsync null. Address='{address}'");

            // Se vuoi forzare gli ancoraggi UI:
            if (!worldPositionStays && instance != null)
            {
                var rt = instance.transform as RectTransform;
                if (rt != null && parent != null) rt.SetParent(parent, false);
            }

            return instance;
        }

        public void ReleaseInstance(GameObject instance)
        {
            if (instance != null)
                Addressables.ReleaseInstance(instance);
        }

        public void Release<T>(T asset)
        {
            if (asset != null)
                Addressables.Release(asset);
        }
    }
}