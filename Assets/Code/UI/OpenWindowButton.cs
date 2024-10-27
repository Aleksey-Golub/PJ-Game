using Code.Services;
using Code.UI.Services;
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
    }
}