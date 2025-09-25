// Assets/Core/Runtime/Util/AsyncOperationAwaiter.cs
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace hp55games.Mobile.Core.Runtime.Util
{
    /// <summary>
    /// Rende awaitable UnityEngine.AsyncOperation (SceneManager, Resources ecc.).
    /// </summary>
    public static class AsyncOperationAwaiter
    {
        public static TaskAwaiter GetAwaiter(this AsyncOperation op)
        {
            var tcs = new TaskCompletionSource<object>();
            // Se l'operazione è già completata, risolvi subito
            if (op.isDone)
            {
                tcs.SetResult(null);
            }
            else
            {
                op.completed += _ => tcs.SetResult(null);
            }
            return ((Task)tcs.Task).GetAwaiter();
        }
    }
}