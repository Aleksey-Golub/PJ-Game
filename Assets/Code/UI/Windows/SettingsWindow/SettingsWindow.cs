using Code.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    internal class SettingsWindow : WindowBase
    {
        [SerializeField] private TextMeshProUGUI _header;
        [SerializeField] private ButtonSwitcher _soundsButton;
        [SerializeField] private Slider _soundSlider;
        [SerializeField] private ButtonSwitcher _musicButton;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private SwitchableLabel _languageSwitchableLabel;
        [SerializeField] private Button _removeProgressBtn;
        [SerializeField] private Button _saveProgressBtn;

        private ISaveLoadAppSettingsService _saveLoadAppSettingsService;

        internal void Construct(IAudioService audio, ISaveLoadAppSettingsService saveLoadAppSettingsService)
        {
            base.Construct(audio);

            _saveLoadAppSettingsService = saveLoadAppSettingsService;

#if DEBUG
            _removeProgressBtn.gameObject.SetActive(true);
            _saveProgressBtn.gameObject.SetActive(true);
#else
            _removeProgressBtn.gameObject.SetActive(false);
            _saveProgressBtn.gameObject.SetActive(false);
#endif
        }

        internal void Open()
        {
            gameObject.SetActive(true);

            RefreshUI();

            _soundSlider.normalizedValue = Audio.GetNormalizedVolume(AudioService.SFX);
            _musicSlider.normalizedValue = Audio.GetNormalizedVolume(AudioService.MUSIC);
        }

        public override void Close()
        {
            CloseSelf();
            _saveLoadAppSettingsService.SaveAppSettings();
        }

        protected override void SubscribeUpdates()
        {
            _soundsButton.SubscribeUpdates();
            _musicButton.SubscribeUpdates();

            _soundsButton.OnClicked += OnSoundButtonClicked;
            _musicButton.OnClicked += OnMusicButtonClicked;

            _soundSlider.onValueChanged.AddListener(OnSoundSliderValueChanged);
            _musicSlider.onValueChanged.AddListener(OnMusicSliderValueChanged);

            _languageSwitchableLabel.SubscribeUpdates();
            _languageSwitchableLabel.LeftClicked += OnLanguageSwitchableLabelLeftClicked;
            _languageSwitchableLabel.RightClicked += OnLanguageSwitchableLabelRightClicked;

            _removeProgressBtn.onClick.AddListener(OnRemoveProgressButtonClick);
            _saveProgressBtn.onClick.AddListener(OnSaveProgressButtonClick);

            LService.LanguageChanged += RefreshUI;
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

            _languageSwitchableLabel.Cleanup();
            _languageSwitchableLabel.LeftClicked -= OnLanguageSwitchableLabelLeftClicked;
            _languageSwitchableLabel.RightClicked -= OnLanguageSwitchableLabelRightClicked;

            _removeProgressBtn.onClick.RemoveListener(OnRemoveProgressButtonClick);
            _saveProgressBtn.onClick.RemoveListener(OnSaveProgressButtonClick);

            LService.LanguageChanged -= RefreshUI;
        }

        protected override void OnCloseButtonClicked()
        {
            base.OnCloseButtonClicked();
            CloseSelf();

            _saveLoadAppSettingsService.SaveAppSettings();
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

        private void RefreshSounds() => Refresh(AudioService.SFX, _soundsButton, _soundSlider, LService.Localize("k_Sounds"));

        private void RefreshMusic() => Refresh(AudioService.MUSIC, _musicButton, _musicSlider, LService.Localize("k_Music"));

        private void Refresh(string group, ButtonSwitcher switcher, Slider slider, string label)
        {
            AudioGroupData data = Audio.GetData(group);
            bool isOn = !data.IsMuted;
            string stateText = isOn ? LService.Localize("k_on") : LService.Localize("k_off");
            switcher.Set(isOn, $"{label} {stateText}");
            slider.interactable = isOn;
            slider.SetValueWithoutNotify(data.LastNormalizedValue * slider.maxValue);
        }

        private void OnLanguageSwitchableLabelLeftClicked()
        {
            LService.LoadPreviousLanguage();
        }

        private void OnLanguageSwitchableLabelRightClicked()
        {
            LService.LoadNextLanguage();
        }

        private void RefreshUI()
        {
            _header.text = LService.Localize("k_Settings");
            _languageSwitchableLabel.Init(LService.Localize("k_language_name"));
            RefreshSounds();
            RefreshMusic();
        }

        private void OnRemoveProgressButtonClick()
        {
            PlayerPrefs.DeleteKey(SaveLoadService.PROGRESS_KEY);
        }

        private void OnSaveProgressButtonClick()
        {
            var saveLoadService = AllServices.Container.Single<ISaveLoadService>();
            saveLoadService.SaveProgress();
        }
    }
}