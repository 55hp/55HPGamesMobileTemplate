using System.Threading.Tasks;
using UnityEngine;

namespace hp55games.Mobile.Core.UI
{
    public interface IUIPopupService
    {
        Task<GameObject> OpenAsync(string address);
        Task<T> OpenAsync<T>(string address) where T : Component;
        void Close(GameObject popup);
        void CloseAll();
    }
}