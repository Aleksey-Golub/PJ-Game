using Code.Services;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    internal class SkinCardView : MonoBehaviour
    {
        [SerializeField] private Image _skinImage;

        [SerializeField] private Image _buttonImage;
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _buttonText;
        [SerializeField] private AudioClip _buttonClickedClip;

        private SkinId _skinId;
        private IAudioService _audio;
        private string _localizeKey = string.Empty;

        internal event Action<SkinId> ButtonClicked;

        internal void Construct(IAudioService audio)
        {
            _audio = audio;

            LService.LanguageChanged += RefreshUI;
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDestroy()
        {
            LService.LanguageChanged -= RefreshUI;
            _button.onClick.RemoveListener(OnButtonClicked);
        }

        internal void SetData(SkinId skinId, Sprite skinSprite, (string locKey, Color color, bool interactable) props)
        {
            _skinId = skinId;
            _skinImage.sprite = skinSprite;
            _localizeKey = props.locKey;
            _buttonImage.color = props.color;
            _button.interactable = props.interactable;
            RefreshUI();
        }

        private void OnButtonClicked()
        {
            _audio.PlaySfxAtUI(_buttonClickedClip);
            ButtonClicked?.Invoke(_skinId);
        }

        private void RefreshUI()
        {
            _buttonText.text = LService.Localize(_localizeKey);
        }
    }
}