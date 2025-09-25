// Assets/Core/Runtime/Content/ContentLoader.cs
using System.Threading.Tasks;
using UnityEngine;

namespace hp55games.Mobile.Core.Architecture
{
    public interface IContentLoader
    {
        Task<T> LoadAsync<T>(string path) where T : Object;
    }

    /// <summary>
    /// Loader minimale basato su Resources. In seguito potrai sostituirlo con Addressables.
    /// </summary>
    public sealed class BasicContentLoader : IContentLoader
    {
        public async Task<T> LoadAsync<T>(string path) where T : Object
        {
            // Simula async per coerenza con Addressables
            await System.Threading.Tasks.Task.Yield();
            return Resources.Load<T>(path);
        }
    }
}