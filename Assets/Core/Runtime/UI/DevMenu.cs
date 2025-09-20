using UnityEngine;


namespace Core.UI {
    public class DevMenu : MonoBehaviour {
        [SerializeField] GameObject panel;
        int _touchCount;
        void Update(){
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.BackQuote)) Toggle();
#endif
            if (Input.touchCount >= 3){ _touchCount++; if (_touchCount > 10) Toggle(); }
            else _touchCount = 0;
        }
        public void Toggle(){ panel.SetActive(!panel.activeSelf); }
    }
}
