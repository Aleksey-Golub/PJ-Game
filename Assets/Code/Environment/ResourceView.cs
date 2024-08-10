using System;
using TMPro;
using UnityEngine;

internal class ResourceView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SpriteRenderer _shadowRenderer;
    [SerializeField] private Animation _animation;
    [SerializeField] private TextMeshPro _countText;
    [SerializeField] private int _countTextSortingOrder = 1;

    [Header("Settings")]
    [SerializeField] private AnimationCurve _spriteMovementDropCurve;
    [SerializeField] private float _startSpritePositionY = 0.1f;
    [SerializeField] private float _minAlpha = 0f;
    [SerializeField] private float _maxAlpha = 1f;
    [SerializeField] private Vector3 _minSize = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField] private Vector3 _maxSize = new Vector3(1f, 1f, 1f);


#if UNITY_EDITOR
    private void OnValidate()
    {
        _countText.GetComponent<MeshRenderer>().sortingOrder = _countTextSortingOrder;
    }
#endif

    internal void Init(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
    }

    internal void ShowCount(int count)
    {
        _countText.gameObject.SetActive(count > 1);
        _countText.text = $"x{count}";
    }

    internal void ShowStartDrop()
    {
        _animation.Stop();

        _spriteRenderer.SetAlphaTo(_minAlpha);
        _shadowRenderer.SetAlphaTo(_minAlpha);

        transform.localScale = _minSize;
    }

    internal void ShowDropping(float t)
    {
        float alpha = Mathf.Lerp(_minAlpha, _maxAlpha, t);
        _spriteRenderer.SetAlphaTo(alpha);
        _shadowRenderer.SetAlphaTo(alpha);

        transform.localScale = Vector3.Lerp(_minSize, _maxSize, t);

        var pos = _spriteRenderer.transform.localPosition;
        pos.y = _startSpritePositionY + _spriteMovementDropCurve.Evaluate(t);
        _spriteRenderer.transform.localPosition = pos;
    }

    internal void ShowEndDrop()
    {
        _animation.Play();

        _spriteRenderer.SetAlphaTo(_maxAlpha);
        _shadowRenderer.SetAlphaTo(_maxAlpha);

        transform.localScale = _maxSize;
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

