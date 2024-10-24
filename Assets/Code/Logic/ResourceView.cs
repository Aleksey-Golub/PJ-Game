using TMPro;
using UnityEngine;

internal class ResourceView : DropObjectViewBase
{
    [Header("Self")]
    [SerializeField] private TextMeshPro _countText;
    [SerializeField] private int _countTextSortingOrder = 1;

#if UNITY_EDITOR
    private void OnValidate()
    {
        _countText.GetComponent<MeshRenderer>().sortingOrder = _countTextSortingOrder;
    }
#endif

    internal void ShowCount(int count)
    {
        _countText.gameObject.SetActive(count > 1);
        _countText.text = $"x{count}";
    }

    internal void ShowStartMerge()
    {
        _animation.Stop();

        _spriteRenderer.SetAlphaTo(_maxAlpha);
        _shadowRenderer.SetAlphaTo(_maxAlpha);

        transform.localScale = _maxSize;
    }

    internal void ShowMerging(float t)
    {
        float alpha = Mathf.Lerp(_maxAlpha, _minAlpha, t);
        _spriteRenderer.SetAlphaTo(alpha);
        _shadowRenderer.SetAlphaTo(alpha);

        transform.localScale = Vector3.Lerp(_maxSize, _minSize, t);

        var startSpriteLocalPos = _spriteRenderer.transform.localPosition;
        var endSpriteLocalPos = _spriteRenderer.transform.localPosition;
        endSpriteLocalPos.y = _startSpritePositionY;

        _spriteRenderer.transform.localPosition = Vector3.Lerp(startSpriteLocalPos, endSpriteLocalPos, t * t);
    }

    internal void ShowEndMerge()
    {
        _spriteRenderer.SetAlphaTo(_minAlpha);
        _shadowRenderer.SetAlphaTo(_minAlpha);

        transform.localScale = _minSize;
    }
}
