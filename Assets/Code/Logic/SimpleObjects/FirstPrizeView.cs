using Code.Services;
using System.Collections;
using UnityEngine;

internal class FirstPrizeView : MonoBehaviour
{
    [SerializeField] private UniqueId _uniqueId;
    [SerializeField] private AudioClip _explosionSound;
    [SerializeField] private EffectId _spawnPortalEffectType;
    [SerializeField] private Transform _spawnPortalEffectTemplate;
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private AudioClip _catClip;

    [Header("Sequence settings")]
    [SerializeField] private float _slimeAppearanceDelay = 1f;
    [SerializeField] private float _slimeDisappearanceDelay = 1f;
    [SerializeField] private float _slimeAfterDisappearanceDelay = 0.5f;
    [SerializeField] private GameObject _pointer;
    [SerializeField] private EffectId _slimeAppearanceEffectType;
    [SerializeField] private Transform _slimeAppearanceEffectTemplate;
    [SerializeField] private AudioClip _slimeAppearanceSound;
    [SerializeField] private Transform _slimeGO;
    [SerializeField] private Transform _catGO;
    [SerializeField] private EffectId _slimeDisappearanceEffectType;
    [SerializeField] private Transform _slimeDisappearanceEffectTemplate;
    [SerializeField] private AudioClip _slimeDisappearanceSound;

    private IAudioService _audio;
    private IEffectFactory _effectFactory;

    private string _interactAudioSourceId;

    private string Id => _uniqueId.Id;

    internal void Construct(IAudioService audio, IEffectFactory effectFactory)
    {
        _audio = audio;
        _effectFactory = effectFactory;
    }

    internal void ShowSpawnPortalEffect()
    {
        if (_spawnPortalEffectType != EffectId.None)
            _effectFactory.Get(_spawnPortalEffectType, _spawnPortalEffectTemplate).Play();
    }

    internal void PlaySpawnPortalSound()
    {
        if (_explosionSound != null)
            _audio.PlaySfxAtPosition(_explosionSound, _spawnPortalEffectTemplate.position);
    }

    internal IEnumerator PlayCutScene()
    {
        _pointer.SetActive(false);
        yield return new WaitForSeconds(_slimeAppearanceDelay);
        PlaySlimeAppearance();
        yield return new WaitForSeconds(_slimeDisappearanceDelay);
        PlaySlimeDisappearance();
        yield return new WaitForSeconds(_slimeAfterDisappearanceDelay);
    }

    private void PlaySlimeAppearance()
    {
        // взрыв, звук взрыва, звук хохота, появление гигантского слайма
        if (_slimeAppearanceEffectType != EffectId.None)
            _effectFactory.Get(_slimeAppearanceEffectType, _slimeAppearanceEffectTemplate).Play();
        if (_explosionSound != null)
            _audio.PlaySfxAtPosition(_explosionSound, _slimeAppearanceEffectTemplate.position);
        if (_slimeAppearanceSound != null)
            _audio.PlaySfxAtPosition(_slimeAppearanceSound, _slimeAppearanceEffectTemplate.position);

        _slimeGO.gameObject.SetActive(true);
    }

    private void PlaySlimeDisappearance()
    {
        // взрыв, кот и Слайм пропадают
        if (_slimeDisappearanceEffectType != EffectId.None)
            _effectFactory.Get(_slimeDisappearanceEffectType, _slimeDisappearanceEffectTemplate).Play();
        if (_explosionSound != null)
            _audio.PlaySfxAtPosition(_explosionSound, _slimeDisappearanceEffectTemplate.position);
        if (_slimeDisappearanceSound != null)
            _audio.PlaySfxAtPosition(_slimeDisappearanceSound, _slimeAppearanceEffectTemplate.position);

        _slimeGO.gameObject.SetActive(false);
        _catGO.gameObject.SetActive(false);
    }

    //[UsedImplicitly]
    //internal void PlayExplosionSound()
    //{
    //    if (_explosionSound != null)
    //        _audio.PlaySfxAtPosition(_explosionSound, transform.position);
    //}

    internal void PlayInteractSound()
    {
        if (_audio.IsSfxPlaying(_catClip, _interactAudioSourceId, Id))
            return;

        _interactAudioSourceId = _audio.PlaySfxAtPosition(_catClip, transform.position, Id, looping: true);
    }

    internal void StopInteractSound()
    {
        _audio.StopSfx(_catClip, _interactAudioSourceId, Id);
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
