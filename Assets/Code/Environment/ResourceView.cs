using UnityEngine;

internal class ResourceView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SpriteRenderer _shadowRenderer;
    [SerializeField] private Animation _animation;

    private const float MIN_ALPHA = 0f;
    private const float MAX_ALPHA = 1f;

    internal void Init(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
        //
        _animation.Play();



        //_animation.Stop();

        //_spriteRenderer.SetAlphaTo(MIN_ALPHA);
        //_shadowRenderer.SetAlphaTo(MIN_ALPHA);
    }
}

