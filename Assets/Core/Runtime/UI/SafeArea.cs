using UnityEngine;
using UnityEngine.UI;


namespace Core.UI {
    [RequireComponent(typeof(RectTransform))]
    public class SafeArea : MonoBehaviour {
        void OnEnable() => Apply();
        void Apply(){
            var rt = (RectTransform)transform;
            var a = Screen.safeArea;
            var min = a.position; var max = a.position + a.size;
            min.x /= Screen.width; min.y /= Screen.height;
            max.x /= Screen.width; max.y /= Screen.height;
            rt.anchorMin = min; rt.anchorMax = max;
        }
    }
}