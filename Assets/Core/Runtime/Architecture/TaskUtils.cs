using System;
using System.Threading.Tasks;
using UnityEngine;

namespace hp55games.Mobile.Core.Architecture
{
    public static class TaskUtils
    {
        // Esegue Task “fire-and-forget” ma con cattura/log delle eccezioni.
        public static async void SafeFireAndForget(this Task task, Action<Exception> onException = null)
        {
            try { await task; }
            catch (Exception ex)
            {
                onException?.Invoke(ex);
                Debug.LogException(ex);
            }
        }
    }
}