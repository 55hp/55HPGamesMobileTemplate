using System.Threading.Tasks;

namespace hp55games.Mobile.Core.UI
{
    public interface IUIToastService {
        Task ShowAsync(string message, float seconds=2f);
    }
}