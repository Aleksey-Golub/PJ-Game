using UnityEngine;

internal class ResourceSourceView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animation _animation;
    [SerializeField] private GameObject _hpBg;
    [SerializeField] private GameObject _hpFg;
    [SerializeField] private Effect _hitEffect;

    [Header("Settings")]
    [SerializeField] private Sprite _diedSprite;
    [SerializeField] private Sprite _wholeSprite;
    [SerializeField] private bool _changeSortingOrderWhenExhaust = false;
    [SerializeField] private int _exhaustSortingOrder;
    [SerializeField] private AudioClip _hitAudioClip;

    private int _oldSortingOrder;

    [ContextMenu(nameof(ShowExhaust))]
    internal void ShowExhaust()
    {
        _spriteRenderer.sprite = _diedSprite;

        if (_changeSortingOrderWhenExhaust)
        {
            _oldSortingOrder = _spriteRenderer.sortingOrder;
            _spriteRenderer.sortingOrder = _exhaustSortingOrder;
        }
    }
    [ContextMenu(nameof(ShowWhole))]
    internal void ShowWhole()
    {
        _spriteRenderer.sprite = _wholeSprite;

        if (_changeSortingOrderWhenExhaust)
        {
            _spriteRenderer.sortingOrder = _oldSortingOrder;
        }
    }

    internal void ShowHitAnimation()
    {
        _animation.Play();
    }

    internal void ShowHitEffect()
    {
        _hitEffect.Play();
    }

    internal void PlayHitSound()
    {
        AudioSource.PlayClipAtPoint(_hitAudioClip, transform.position);
    }

    internal void ShowHP(int currentHitPoints, int totalHitPoints)
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
