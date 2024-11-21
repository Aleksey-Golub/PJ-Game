using Code.Services;
using TMPro;
using UnityEngine;

public class AdsBoxView : AdsObjectView
{
    [Space]
    [SerializeField] private AudioClip _dropResourceAudioClip;
    [SerializeField] private GameObject _dropCloud;
    [SerializeField] private SpriteRenderer _dropResourceRenderer;
    [SerializeField] private TMP_Text _dropCounText;

    private IAudioService _audio;

    internal void Construct(IAudioService audio)
    {
        _audio = audio;
    }

    internal void Init(Sprite dropResourceSprite, int dropResourceCount)
    {
        _dropResourceRenderer.sprite = dropResourceSprite;
        _dropCounText.text = $"+{dropResourceCount}";
    }

    internal void PlayDropResourceSound()
    {
        _audio.PlaySfxAtPosition(_dropResourceAudioClip, transform.position);
    }

    internal override void ShowWhole()
    {
        base.ShowWhole();

        _dropCloud.SetActive(true);
    }

    internal override void ShowExhaust()
    {
        base.ShowExhaust();

        _dropCloud.SetActive(false);
    }
}
