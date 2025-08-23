using Code.Services;
using Code.UI.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    internal class OpenWindowButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private WindowId _windowId;
        [SerializeField] private AudioClip _clip;
        [SerializeField] private bool _closeWindowToo;

        [Header ("Localization. Can be null")]
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private string _textLocalizationKey;

        private IUIMediator _uiService;
        private IAudioService _audio;

        public void Construct(IUIMediator uiService, IAudioService audio)
        {
            _uiService = uiService;
            _audio = audio;
        }

        private void Awake()
        {
            _button.onClick.AddListener(SwitchWindow);
            LService.LanguageChanged += RefreshUI;

            RefreshUI();
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(SwitchWindow);
            LService.LanguageChanged -= RefreshUI;
        }

        private void SwitchWindow()
        {
            _audio.PlaySfxAtUI(_clip);

            if (_uiService.IsOpened(_windowId))
            {
                if (_closeWindowToo)
                    _uiService.Close(_windowId);
            }
            else
            {
                _uiService.Open(_windowId);
            }
        }

        private void RefreshUI()
        {
            if (!_text)
                return;

            _text.text = LService.Localize(_textLocalizationKey);
        }
    }
}