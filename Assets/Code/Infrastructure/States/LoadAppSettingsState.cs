﻿using Code.Data;
using Code.Services;

namespace Code.Infrastructure
{
    public class LoadAppSettingsState : IState
    {
        private readonly GameStateMachine _gameStateMachine;
        private readonly IAppSettingsService _appSettingsService;
        private readonly ISaveLoadAppSettingsService _saveLoadAppSettingsService;
        private readonly IAudioService _audio;

        public LoadAppSettingsState(
            GameStateMachine gameStateMachine,
            IAppSettingsService appSettingsService,
            ISaveLoadAppSettingsService saveLoadAppSettingsService,
            IAudioService audio
            )
        {
            _gameStateMachine = gameStateMachine;
            _appSettingsService = appSettingsService;
            _saveLoadAppSettingsService = saveLoadAppSettingsService;
            _audio = audio;
        }

        public void Enter()
        {
            LoadAppSettingsOrInitNew();

            InformAppSettingsReaders();

            _gameStateMachine.Enter<LoadMainMenuState>();
        }

        public void Exit()
        {
        }

        private void LoadAppSettingsOrInitNew()
        {
            _appSettingsService.Settings =
              _saveLoadAppSettingsService.LoadAppSettings()
              ?? NewAppSettings();
        }

        private AppSettings NewAppSettings()
        {
            var appSettings = new AppSettings();
            appSettings.AudioSettings.DefaultNormalizedVolume = 0.15f;

            return appSettings;
        }

        private void InformAppSettingsReaders()
        {
            AppSettings appSettings = _appSettingsService.Settings;

            _audio.ReadAppSettings(appSettings);
            LService.ReadAppSettings(appSettings);
        }
    }
}