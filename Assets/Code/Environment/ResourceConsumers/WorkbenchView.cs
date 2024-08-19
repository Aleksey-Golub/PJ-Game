using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal class WorkbenchView : MonoBehaviour
{
    [SerializeField] private GameObject _cloud;
    [SerializeField] private Image _resourceNeedImage;
    [SerializeField] private Image _generateObjImage;
    [SerializeField] private TextMeshProUGUI _needText;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animation _animation;
    [SerializeField] private Effect _hitEffect;

    [Header("Settings")]
    [SerializeField] private Sprite _diedSprite;
    [SerializeField] private Sprite _wholeSprite;
    [SerializeField] private bool _changeSortingOrderWhenExhaust = false;
    [SerializeField] private int _exhaustSortingOrder;
    [SerializeField] private AudioClip _dropResourceAudioClip;

    private int _oldSortingOrder;

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
            AudioSource.PlayClipAtPoint(_dropResourceAudioClip, transform.position);
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
        if (_hitEffect != null)
            _hitEffect.Play();
    }
}
