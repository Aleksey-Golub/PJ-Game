using Code.Services;
using UnityEngine;

public class AdsObjectView : MonoBehaviour
{
    [Tooltip("Can be null")]
    [SerializeField] protected Animation _animation;
    [SerializeField] private GameObject _cloud;
    [SerializeField] private GameObject _progressFg;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameObject _shadow;

    [Header("Settings")]
    [Tooltip("Can be null")]
    [SerializeField] private Sprite _diedSprite;
    [SerializeField] private Sprite _wholeSprite;
    [SerializeField] private AudioClip _rewardAcceptedClip;

    protected IAudioService Audio;

    internal void Construct(IAudioService audio)
    {
        Audio = audio;

        if (_animation != null)
            _animation.Play();
    }

    internal void ShowProgress(float passed, float total)
    {
        float t = passed / total;

        Vector3 newScale = _progressFg.transform.localScale;
        newScale.x = t;
        _progressFg.transform.localScale = newScale;

        _cloud.SetActive(passed > 0);
    }

    internal void HideProgress()
    {
        _cloud.SetActive(false);
    }

    internal virtual void ShowExhaust()
    {
        _spriteRenderer.sprite = _diedSprite;
        _shadow.SetActive(false);
    }

    internal virtual void ShowWhole()
    {
        _spriteRenderer.sprite = _wholeSprite;
        _shadow.SetActive(true);
    }

    internal void PlayRewardAcceptSound()
    {
        Audio.PlaySfxAtPosition(_rewardAcceptedClip, transform.position);
    }
}
