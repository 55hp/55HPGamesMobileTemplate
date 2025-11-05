using System.Threading.Tasks;

namespace hp55games.Mobile.Core.UI
{
    public interface IUINavigationService
    {
        bool CanGoBack { get; }
        Task PushAsync(string address);
        Task ReplaceAsync(string address);
        Task PopAsync();
    }
}