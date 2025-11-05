// hp55games.Ui
using UnityEngine;
using TMPro;

namespace hp55games.Ui
{
    public class UIToast : MonoBehaviour
    {
        public CanvasGroup cg;
        public TMP_Text text;
        public float duration = 2.0f;

        public void Show(string msg)
        {
            text.text = msg;
            cg.alpha = 1; cg.blocksRaycasts = false; cg.interactable = false;
            CancelInvoke(); Invoke(nameof(Hide), duration);
        }
        public void Hide() { cg.alpha = 0; }
    }
}