using Code.Services;
using UnityEngine;

namespace Code.UI
{
    internal class SettingsWindow : WindowBase
    {
        [SerializeField] private ButtonSwitcher _soundsButton;
        [SerializeField] private ButtonSwitcher _musicButton;

        internal new void Construct(IAudioService audio)
        {
            base.Construct(audio);
        }

        internal void Open()
        {
            gameObject.SetActive(true);
            RefreshUI();
        }

        protected override void SubscribeUpdates()
        {
            _soundsButton.SubscribeUpdates();
            _musicButton.SubscribeUpdates();

            _soundsButton.OnClicked += OnSoundButtonClicked;
            _musicButton.OnClicked += OnMusicButtonClicked;
        }

        protected override void Cleanup()
        {
            base.Cleanup();

            _soundsButton.OnClicked -= OnSoundButtonClicked;
            _musicButton.OnClicked -= OnMusicButtonClicked;

            _soundsButton.Cleanup();
            _musicButton.Cleanup();
        }

        protected override void OnCloseButtonClicked()
        {
            base.OnCloseButtonClicked();

            gameObject.SetActive(false);
        }

        private void OnSoundButtonClicked(ButtonSwitcher button)
        {
            Audio.SwitchMute(AudioService.SFX);
            RefreshUI();
        }

        private void OnMusicButtonClicked(ButtonSwitcher button)
        {
            Audio.SwitchMute(AudioService.MUSIC);
            RefreshUI();
        }

        private void RefreshUI()
        {
            bool sfxOn = !Audio.IsMuted(AudioService.SFX);
            string sfxStateText = sfxOn ? "on" : "off";
            _soundsButton.Set(sfxOn, $"Sounds {sfxStateText}");

            bool musicOn = !Audio.IsMuted(AudioService.MUSIC);
            string musicStateText = musicOn ? "on" : "off";
            _musicButton.Set(musicOn, $"Music {musicStateText}");
        }
    }
}