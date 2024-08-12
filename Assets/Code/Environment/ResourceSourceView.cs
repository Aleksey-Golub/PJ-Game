using UnityEngine;

internal class ResourceSourceView : ResourceSourceViewBase
{
    [Header("Self")]
    [SerializeField] private GameObject _hpBg;
    [SerializeField] private GameObject _hpFg;

    internal override void ShowHP(int currentHitPoints, int totalHitPoints)
    {
        Vector3 scale = _hpFg.transform.localScale;
        scale.x = (float)currentHitPoints / totalHitPoints;
        _hpFg.transform.localScale = scale;

        if (currentHitPoints <= 0 || currentHitPoints == totalHitPoints)
            _hpBg.SetActive(false);
        else
            _hpBg.SetActive(true);
    }
}

