using Code.Services;
using UnityEngine;

internal class FinalPrizeView : MonoBehaviour
{
    [SerializeField] private UniqueId _uniqueId;
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private AudioClip _interactClip;

    private IAudioService _audio;

    private string _interactAudioSourceId;

    private string Id => _uniqueId.Id;

    internal void Construct(IAudioService audio)
    {
        _audio = audio;
    }

    internal void PlayInteractSound()
    {
        if (_audio.IsSfxPlaying(_interactClip, _interactAudioSourceId, Id))
            return;

        _interactAudioSourceId = _audio.PlaySfxAtPosition(_interactClip, transform.position, Id, looping: true);
    }

    internal void StopInteractSound()
    {
        _audio.StopSfx(_interactClip, _interactAudioSourceId, Id);
    }

    internal void ShowInteract()
    {
        _particles.Play();
    }

    internal void HideInteract()
    {
        _particles.Stop();
    }
}
