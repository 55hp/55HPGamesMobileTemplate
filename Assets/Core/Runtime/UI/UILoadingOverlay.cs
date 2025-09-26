// hp55games.Ui
using UnityEngine;

namespace hp55games.Ui
{
    public class UILoadingOverlay : MonoBehaviour
    {
        public CanvasGroup cg;
        public void Show() { cg.alpha = 1; cg.blocksRaycasts = true; cg.interactable = true; }
        public void Hide() { cg.alpha = 0; cg.blocksRaycasts = false; cg.interactable = false; }
    }
}