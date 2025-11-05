using UnityEngine;
using TMPro; // se usi TextMeshPro, opzionale

public sealed class UILoadingView : MonoBehaviour
{
    [SerializeField] private TMP_Text noteText; // opzionale

    public void SetNote(string note)
    {
        if (noteText == null) return;
        noteText.text = string.IsNullOrEmpty(note) ? "" : note;
    }
}