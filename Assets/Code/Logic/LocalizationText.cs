using Code.Services;
using UnityEngine;

public class LocalizationText : MonoBehaviour
{
    [SerializeField] private string _key;
    [SerializeField] private TMPro.TMP_Text _text;

    internal void SetKey(string key) => _key = key;

    private void Start()
    {
        LService.LanguageChanged += SetText;
        SetText();
    }

    private void OnDestroy()
    {
        LService.LanguageChanged -= SetText;
    }

    private void SetText()
    {
        _text.text = LService.Localize(_key);
    }
}
