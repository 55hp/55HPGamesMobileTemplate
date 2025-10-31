using System.Threading.Tasks;

namespace hp55games.Mobile.Core.UI
{
    public interface IUiOverlayService {
        void ShowLoading(string note=null);
        void HideLoading();
        Task FadeIn(float dur=0.2f);
        Task FadeOut(float dur=0.2f);
        void BlockInput(bool on);
    }
}