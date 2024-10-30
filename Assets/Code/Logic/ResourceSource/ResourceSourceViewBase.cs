using Code.Services;
using UnityEngine;

internal abstract class ResourceSourceViewBase : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animation _animation;
    [SerializeField] private EffectId _hitEffectType;
    [SerializeField] private Transform _effectTemplate;

    [Header("Settings")]
    [SerializeField] private Sprite _diedSprite;
    [SerializeField] private Sprite _wholeSprite;
    [SerializeField] private bool _changeSortingOrderWhenExhaust = false;
    [SerializeField] private int _exhaustSortingOrder;
    [SerializeField] private AudioClip _hitAudioClip;
    [SerializeField] private AudioClip _dropResourceAudioClip;

    private int _oldSortingOrder;
    private IAudioService _audio;
    private IEffectFactory _effectFactory;

    internal void Construct(IAudioService audio, IEffectFactory effectFactory)
    {
        _audio = audio;
        _effectFactory = effectFactory;
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
        if (_hitEffectType != EffectId.None)
            _effectFactory.Get(_hitEffectType, _effectTemplate).Play();
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

