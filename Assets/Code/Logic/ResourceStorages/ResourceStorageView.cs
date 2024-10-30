using Code.Services;
using TMPro;
using UnityEngine;

internal class ResourceStorageView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animation _animation;
    [SerializeField] private EffectId _hitEffectType;
    [SerializeField] private Transform _effectTemplate;
    [SerializeField] private TextMeshPro _countText;
    [SerializeField] private int _countTextSortingOrder = 1;

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

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_countText)
            _countText.GetComponent<MeshRenderer>().sortingOrder = _countTextSortingOrder;
    }
#endif

    internal void Construct(IAudioService audio, IEffectFactory effectFactory)
    {
        _audio = audio;
        _effectFactory = effectFactory;
    }

    internal void PlayDropResourceSound()
    {
        if (_dropResourceAudioClip != null)
            _audio.PlaySfxAtPosition(_dropResourceAudioClip, transform.position);
    }

    internal void PlayHitSound()
    {
        if (_hitAudioClip != null)
            _audio.PlaySfxAtPosition(_hitAudioClip, transform.position);
    }

    internal void ShowHitAnimation()
    {
        if (_animation != null)
            _animation.Play();
    }

    internal void ShowHitEffect()
    {
        if (_hitEffectType != EffectId.None)
            _effectFactory.Get(_hitEffectType, _effectTemplate).Play();
    }

    internal void ShowResourceCount(int currentResourceCount, int dropResourceCount)
    {
        if (_countText)
            _countText.text = $"{currentResourceCount}/{dropResourceCount}";
    }

    internal void ShowExhaust()
    {
        _spriteRenderer.sprite = _diedSprite;

        if (_changeSortingOrderWhenExhaust)
        {
            _oldSortingOrder = _spriteRenderer.sortingOrder;
            _spriteRenderer.sortingOrder = _exhaustSortingOrder;
        }
    }

    internal void ShowWhole()
    {
        _spriteRenderer.sprite = _wholeSprite;

        if (_changeSortingOrderWhenExhaust)
        {
            _spriteRenderer.sortingOrder = _oldSortingOrder;
        }
    }
}
