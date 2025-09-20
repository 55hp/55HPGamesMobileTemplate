// Assets/Core/Runtime/Architecture/Logging/UnityLog.cs
using UnityEngine;

namespace AF55HP.Mobile.Core.Architecture
{
    public sealed class UnityLog : ILog
    {
        public void Info(string msg)  => Debug.Log(msg);
        public void Warn(string msg)  => Debug.LogWarning(msg);
        public void Error(string msg) => Debug.LogError(msg);
    }
}