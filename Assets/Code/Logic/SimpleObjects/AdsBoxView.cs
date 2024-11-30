using TMPro;
using UnityEngine;

public class AdsBoxView : AdsObjectView
{
    [Space]
    [SerializeField] private AudioClip _dropResourceAudioClip;
    [SerializeField] private GameObject _dropCloud;
    [SerializeField] private SpriteRenderer _dropResourceRenderer;
    [SerializeField] private TMP_Text _dropCounText;

    internal void Init(Sprite dropResourceSprite, int dropResourceCount)
    {
        _dropResourceRenderer.sprite = dropResourceSprite;
        _dropCounText.text = $"+{dropResourceCount}";
    }

    internal void PlayDropResourceSound()
    {
        Audio.PlaySfxAtPosition(_dropResourceAudioClip, transform.position);
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
