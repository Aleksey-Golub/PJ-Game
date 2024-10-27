using Code.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public abstract class WindowBase : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private AudioClip _closeButtonClip;

        protected IAudioService Audio;

        internal virtual bool IsOpened => gameObject.activeInHierarchy;

        protected void Construct(IAudioService audio)
        {
            Audio = audio;
        }

        private void Awake() => OnAwake();

        private void Start()
        {
            Initialize();
            SubscribeUpdates();
        }

        private void OnDestroy()
        {
            _closeButton.onClick.RemoveListener(OnCloseButtonClicked);
            Cleanup();
        }

        public abstract void Close();

        protected virtual void OnAwake()
        {
            _closeButton.onClick.AddListener(OnCloseButtonClicked);
        }

        protected virtual void Initialize() { }
        protected virtual void SubscribeUpdates() { }
        protected virtual void Cleanup() { }
        protected virtual void OnCloseButtonClicked()
        {
            if (_closeButtonClip)
                Audio.PlaySfxAtUI(_closeButtonClip);
        }
    }
}