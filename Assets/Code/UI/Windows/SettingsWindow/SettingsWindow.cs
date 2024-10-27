using Code.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    internal class SettingsWindow : WindowBase
    {
        [SerializeField] private ButtonSwitcher _soundsButton;
        [SerializeField] private Slider _soundSlider;
        [SerializeField] private ButtonSwitcher _musicButton;
        [SerializeField] private Slider _musicSlider;

        internal new void Construct(IAudioService audio)
        {
            base.Construct(audio);
        }

        internal void Open()
        {
            gameObject.SetActive(true);
            
            RefreshSounds();
            RefreshMusic();
            _soundSlider.normalizedValue = Audio.GetNormalizedVolume(AudioService.SFX);
            _musicSlider.normalizedValue = Audio.GetNormalizedVolume(AudioService.MUSIC);
        }

        public override void Close()
        {
            CloseSelf();
        }

        protected override void SubscribeUpdates()
        {
            _soundsButton.SubscribeUpdates();
            _musicButton.SubscribeUpdates();

            _soundsButton.OnClicked += OnSoundButtonClicked;
            _musicButton.OnClicked += OnMusicButtonClicked;

            _soundSlider.onValueChanged.AddListener(OnSoundSliderValueChanged);
            _musicSlider.onValueChanged.AddListener(OnMusicSliderValueChanged);
        }

        protected override void Cleanup()
        {
            base.Cleanup();

            _soundsButton.OnClicked -= OnSoundButtonClicked;
            _musicButton.OnClicked -= OnMusicButtonClicked;

            _soundSlider.onValueChanged.RemoveListener(OnSoundSliderValueChanged);
            _musicSlider.onValueChanged.RemoveListener(OnMusicSliderValueChanged);

            _soundsButton.Cleanup();
            _musicButton.Cleanup();
        }

        protected override void OnCloseButtonClicked()
        {
            base.OnCloseButtonClicked();
            CloseSelf();
        }

        private void CloseSelf() => gameObject.SetActive(false);

        private void OnSoundButtonClicked(ButtonSwitcher button)
        {
            Audio.SwitchMute(AudioService.SFX);
            RefreshSounds();
        }

        private void OnMusicButtonClicked(ButtonSwitcher button)
        {
            Audio.SwitchMute(AudioService.MUSIC);
            RefreshMusic();
        }

        private void OnSoundSliderValueChanged(float newValue)
        {
            Audio.SetNormalizedVolume(AudioService.SFX, _soundSlider.normalizedValue);
        }
        
        private void OnMusicSliderValueChanged(float newValue)
        {
            Audio.SetNormalizedVolume(AudioService.MUSIC, _musicSlider.normalizedValue);
        }

        private void RefreshSounds() => Refresh(AudioService.SFX, _soundsButton, _soundSlider, "Sounds");

        private void RefreshMusic() => Refresh(AudioService.MUSIC, _musicButton, _musicSlider, "Music");

        private void Refresh(string group, ButtonSwitcher switcher, Slider slider, string label)
        {
            bool isOn = !Audio.IsMuted(group);
            string stateText = isOn ? "on" : "off";
            switcher.Set(isOn, $"{label} {stateText}");
            slider.interactable = isOn;
        }
    }
}