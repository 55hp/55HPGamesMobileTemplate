using System.Threading.Tasks;

namespace hp55games.Mobile.Core.UI
{
    public interface IUIOverlayService
    {
        // Fade nero sopra tutto (Overlays)
        Task FadeInAsync(float duration = 0.2f);   // alpha 0 -> 1
        Task FadeOutAsync(float duration = 0.2f);  // alpha 1 -> 0

        // Loading globale (spinner + nota opzionale)
        Task ShowLoadingAsync(string note = null);
        void HideLoading();

        // Blocca input globale (invisibile, solo raycast blocker)
        void BlockInput(bool on);
    }
}