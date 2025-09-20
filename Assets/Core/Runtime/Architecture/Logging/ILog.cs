// Assets/Core/Runtime/Architecture/Logging/ILog.cs
namespace AF55HP.Mobile.Core.Architecture
{
    public interface ILog
    {
        void Info(string msg);
        void Warn(string msg);
        void Error(string msg);
    }
}