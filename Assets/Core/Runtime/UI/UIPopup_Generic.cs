// hp55games.Ui
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace hp55games.Ui
{
    public class UIPopup_Generic : MonoBehaviour
    {
        public CanvasGroup cg;
        public TMP_Text title;
        public TMP_Text body;
        public Button confirm;
        public Button cancel;

        public void Open(string t, string b, System.Action onConfirm, System.Action onCancel = null)
        {
            title.text = t; body.text = b;
            confirm.onClick.RemoveAllListeners();
            confirm.onClick.AddListener(() => { onConfirm?.Invoke(); Close(); });
            if (cancel != null)
            {
                cancel.onClick.RemoveAllListeners();
                cancel.onClick.AddListener(() => { onCancel?.Invoke(); Close(); });
            }
            cg.alpha = 1; cg.blocksRaycasts = true; cg.interactable = true;
        }

        public void Close()
        {
            cg.alpha = 0; cg.blocksRaycasts = false; cg.interactable = false;
        }
    }
}