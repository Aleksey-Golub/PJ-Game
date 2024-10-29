using Code.Services;
using UnityEngine;

public class DungeonEntranceView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _inProgressSprite;
    [SerializeField] private Sprite _readySprite;
    [SerializeField] private GameObject _marker;
    [SerializeField] private GameObject _progress;
    [SerializeField] private GameObject _progressFg;
    [Tooltip("Can be null")]
    [SerializeField] private Effect _openEffect;
    [Tooltip("Can be null")]
    [SerializeField] private AudioClip _openAudioClip;

    private IAudioService _audio;

    internal void Construct(IAudioService audio)
    {
        _audio = audio;
    }

    internal void ShowProgress(float current, float total)
    {
        float t = current / total;

        Vector3 newScale = _progressFg.transform.localScale;
        newScale.x = t;
        _progressFg.transform.localScale = newScale;

        bool done = current >= total;
        _spriteRenderer.sprite = done ? _readySprite : _inProgressSprite;

        _progress.SetActive(!done);
        _marker.SetActive(done);

        if (done)
        {
            ShowOpenEffect();
            PlayOpenSound();
        }
    }

    internal void ShowForceOpened()
    {
        _spriteRenderer.sprite = _readySprite;

        _progress.SetActive(false);
        _marker.SetActive(true);

        ShowOpenEffect();
        PlayOpenSound();
    }

    internal void ShowForceClosed()
    {
        _spriteRenderer.sprite = _inProgressSprite;

        _progress.SetActive(false);
        _marker.SetActive(false);
    }

    private void ShowOpenEffect()
    {
        if (_openEffect != null)
            _openEffect.Play();
    }

    private void PlayOpenSound()
    {
        if (_openAudioClip != null)
            _audio.PlaySfxAtPosition(_openAudioClip, transform.position);
    }
}
