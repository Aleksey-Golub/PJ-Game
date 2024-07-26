using UnityEngine;

internal class ResourceSourceView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [SerializeField] private Sprite _diedSprite;
    [SerializeField] private Sprite _wholeSprite;

    internal void ShowExhaust()
    {
        _spriteRenderer.sprite = _diedSprite;
    }

    internal void ShowWhole()
    {
        _spriteRenderer.sprite = _wholeSprite;
    }
}
