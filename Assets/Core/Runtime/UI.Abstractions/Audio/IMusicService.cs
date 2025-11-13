using System.Threading.Tasks;

namespace hp55games.Mobile.Core.UI
{
    /// <summary>
    /// Gestore musica di sottofondo (BGM).
    /// Usa string address (Addressables) per le tracce.
    /// </summary>
    public interface IMusicService
    {
        string CurrentTrackAddress { get; }
        bool IsPlaying { get; }

        Task PlayAsync(string address, float fadeIn = 0.5f);
        Task CrossfadeToAsync(string address, float duration = 0.75f);
        Task StopAsync(float fadeOut = 0.5f);
    }
}