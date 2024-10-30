using Code.Services;
using TMPro;
using UnityEngine;

internal class ResourceConsumerView : MonoBehaviour
{
    [SerializeField] private GameObject _cloud;
    [SerializeField] private SpriteRenderer _resourceNeedImage;
    [SerializeField] private SpriteRenderer _generateObjImage;
    [SerializeField] private TextMeshPro _needText;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animation _animation;
    [SerializeField] private EffectId _hitEffectType;
    [SerializeField] private Transform _effectTemplate;

    [Header("Settings")]
    [SerializeField] private Sprite _diedSprite;
    [SerializeField] private Sprite _wholeSprite;
    [SerializeField] private bool _changeSortingOrderWhenExhaust = false;
    [SerializeField] private int _exhaustSortingOrder;
    [SerializeField] private AudioClip _dropResourceAudioClip;
    [SerializeField] private int _countTextSortingOrder = 11;

    private int _oldSortingOrder;
    private IAudioService _audio;
    private IEffectFactory _effectFactory;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_needText)
            _needText.GetComponent<MeshRenderer>().sortingOrder = _countTextSortingOrder;
    }
#endif

    internal void Construct(IAudioService audio, IEffectFactory effectFactory)
    {
        _audio = audio;
        _effectFactory = effectFactory;
    }

    internal void Init(Sprite needResourceSprite, int initialNeedResourceCount, Sprite generateObjSprite)
    {
        _resourceNeedImage.sprite = needResourceSprite;
        _needText.text = initialNeedResourceCount.ToString();
        _generateObjImage.sprite = generateObjSprite;
    }

    internal void ShowNeeds(int currentNeedResourceCount)
    {
        _needText.text = currentNeedResourceCount.ToString();

        _cloud.SetActive(currentNeedResourceCount != 0);
    }

    internal void PlayDropResourceSound()
    {
        if (_dropResourceAudioClip != null)
            _audio.PlaySfxAtPosition(_dropResourceAudioClip, transform.position);
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
}
