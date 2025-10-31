using System.Threading.Tasks;

namespace hp55games.Mobile.Core.UI
{
    public interface IUINavigationService {
        Task PushAsync(string address);     // aggiunge una Page
        Task PopAsync();                    // torna indietro
        Task ReplaceAsync(string address);  // rimpiazza la corrente
        bool CanGoBack { get; }
    }
}