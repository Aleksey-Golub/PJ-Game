using UnityEngine;

internal abstract class ResourceSourceViewBase : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animation _animation;
    [SerializeField] private Effect _hitEffect;

    [Header("Settings")]
    [SerializeField] private Sprite _diedSprite;
    [SerializeField] private Sprite _wholeSprite;
    [SerializeField] private bool _changeSortingOrderWhenExhaust = false;
    [SerializeField] private int _exhaustSortingOrder;
    [SerializeField] private AudioClip _hitAudioClip;
    [SerializeField] private AudioClip _dropResourceAudioClip;

    private int _oldSortingOrder;
    private AudioService _audio;

    internal void Construct(AudioService audio)
    {
        _audio = audio;
    }

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
        _audio.PlaySfxAtPosition(_hitAudioClip, transform.position);
    }

    internal void PlayDropResourceSound()
    {
        _audio.PlaySfxAtPosition(_dropResourceAudioClip, transform.position);
    }

    internal abstract void ShowHP(int currentHitPoints, int totalHitPoints);
}

