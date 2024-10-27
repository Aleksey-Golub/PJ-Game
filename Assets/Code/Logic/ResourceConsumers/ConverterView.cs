using TMPro;
using UnityEngine;

internal class ConverterView : ResourceConsumerView
{
    [SerializeField] private TextMeshPro _uploadText;
    [SerializeField] private GameObject _progress;
    [SerializeField] private GameObject _progressFg;

    internal void ShowUpload(int currentUpload, int maxUpload)
    {
        _uploadText.text = $"{currentUpload}/{maxUpload}";
    }

    internal void ShowProgress(float timer, float converTime)
    {
        float t = timer / converTime;

        Vector3 newScale = _progressFg.transform.localScale;
        newScale.x = t;
        _progressFg.transform.localScale = newScale;

        _progress.SetActive(timer > 0);
    }
}
