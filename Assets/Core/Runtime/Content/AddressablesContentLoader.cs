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

        public void Release<T>(T asset)
        {
            if (asset != null)
                Addressables.Release(asset);
        }
    }
}