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

        private IUIMediator _uiService;
        private IAudioService _audio;

        public void Construct(IUIMediator uiService, IAudioService audio)
        {
            _uiService = uiService;
            _audio = audio;
        }

        private void Awake()
        {
            _button.onClick.AddListener(Open);
        }

        private void Open()
        {
            _audio.PlaySfxAtUI(_clip);
            _uiService.Open(_windowId);
        }
    }
}