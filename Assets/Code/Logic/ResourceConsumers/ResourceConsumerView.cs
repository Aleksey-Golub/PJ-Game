using Code.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ResourceConsumerView : MonoBehaviour
{
    [SerializeField] protected GameObject _cloud;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animation _animation;
    [SerializeField] private EffectId _hitEffectType;
    [SerializeField] private Transform _effectTemplate;

    [SerializeField] private SpriteRenderer _resourceNeedImage;
    [SerializeField] protected TextMeshPro _needText;
    [SerializeField] private SpriteRenderer _generateObjImage;

    [Header("Needs")]
    [SerializeField] protected ResourceConsumerNeedView[] _needViews;
    [Header("Drop")]
    [SerializeField] protected ResourceConsumerDropView[] _dropViews;

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
    protected Dictionary<ResourceType, ResourceConsumerNeedView> _needViewsCached;
    protected Dictionary<ResourceType, ResourceConsumerDropView> _dropViewsCached;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_needText)
            _needText.GetComponent<MeshRenderer>().sortingOrder = _countTextSortingOrder;
    }
#endif

    internal virtual void Construct(IAudioService audio, IEffectFactory effectFactory)
    {
        _needViewsCached = _needViews.ToDictionary(nView => nView.ResourceType, nView => nView);
        _dropViewsCached = _dropViews.ToDictionary(dView => dView.ResourceType, dView => dView);

        _audio = audio;
        _effectFactory = effectFactory;
    }

    internal void Init(Sprite needResourceSprite, int initialNeedResourceCount, Sprite generateObjSprite)
    {
        _resourceNeedImage.sprite = needResourceSprite;
        _needText.text = initialNeedResourceCount.ToString();
        _generateObjImage.sprite = generateObjSprite;
    }

    internal virtual void ShowNeeds(int currentNeedResourceCount, int totalNeedResourceCount, bool isAvailable)
    {
        _needText.text = currentNeedResourceCount.ToString();

        _cloud.SetActive(isAvailable && currentNeedResourceCount != 0);
    }

    internal void ShowNeeds(ResourceType type, int currentNeedResourceCount, int totalNeedResourceCount, bool isAvailable, int totalCurrentNeedResourcesCount)
    {
        _needViewsCached[type].NeedText.text = currentNeedResourceCount.ToString();

        _cloud.SetActive(isAvailable && totalCurrentNeedResourcesCount != 0);
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

[Serializable]
public class ResourceConsumerNeedView
{
    [field: SerializeField] public ResourceType ResourceType { get; private set; }
    [field: SerializeField] public SpriteRenderer ResourceNeedImage { get; private set; }
    [field: SerializeField] public TextMeshPro NeedText { get; private set; }
}

[Serializable]
public class ResourceConsumerDropView
{
    [field: SerializeField] public ResourceType ResourceType { get; private set; }
    [field: SerializeField] public SpriteRenderer GenerateObjImage { get; private set; }
}
