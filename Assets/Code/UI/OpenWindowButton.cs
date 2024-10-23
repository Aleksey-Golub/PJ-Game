using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.UI
{
    internal class OpenWindowButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private WindowId _windowId;
        [SerializeField] private AudioClip _clip;

        private UIService _uiService;
        private AudioService _audio;

        public void Construct(UIService uiService, AudioService audio)
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