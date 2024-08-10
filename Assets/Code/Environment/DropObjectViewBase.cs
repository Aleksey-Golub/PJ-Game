using UnityEngine;

internal abstract class DropObjectViewBase : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected SpriteRenderer _shadowRenderer;
    [SerializeField] protected Animation _animation;

    [Header("Settings")]
    [SerializeField] private AnimationCurve _spriteMovementDropCurve;
    [SerializeField] protected float _startSpritePositionY = 0.1f;
    [SerializeField] protected float _minAlpha = 0f;
    [SerializeField] protected float _maxAlpha = 1f;
    [SerializeField] protected Vector3 _minSize = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField] protected Vector3 _maxSize = new Vector3(1f, 1f, 1f);

    internal void Init(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
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

    internal void ShowStartDrop()
    {
        _animation.Stop();

        _spriteRenderer.SetAlphaTo(_minAlpha);
        _shadowRenderer.SetAlphaTo(_minAlpha);

        transform.localScale = _minSize;
    }
}
