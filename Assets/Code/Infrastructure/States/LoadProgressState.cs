using Code.Data;
using Code.Services;

namespace Code.Infrastructure
{
    public class LoadProgressState : IState
    {
        private readonly GameStateMachine _gameStateMachine;
        private readonly IPersistentProgressService _progressService;
        private readonly IConfigsService _configs;
        private readonly ISaveLoadService _saveLoadProgress;

        public LoadProgressState(
            GameStateMachine gameStateMachine, 
            IPersistentProgressService progressService, 
            IConfigsService configs, 
            ISaveLoadService saveLoadProgress)
        {
            _gameStateMachine = gameStateMachine;
            _progressService = progressService;
            _configs = configs;
            _saveLoadProgress = saveLoadProgress;
        }

        public void Enter()
        {
            LoadProgressOrInitNew();

            _gameStateMachine.Enter<LoadLevelState, string>(_progressService.Progress.PlayerProgress.PositionOnLevel.Level);
        }

        public void Exit()
        {
        }

        private void LoadProgressOrInitNew()
        {
            _progressService.Progress =
              _saveLoadProgress.LoadProgress()
              ?? NewProgress();
        }

        private GameProgress NewProgress()
        {
            var progress = new GameProgress(initialLevel: "GameScene");

            // set some data here
            SetInitialUpgradeData(_configs, progress);

            return progress;
        }

        private static void SetInitialUpgradeData(IConfigsService configsService, GameProgress progress)
        {
            var upgradeItemsData = progress.PlayerProgress.UpgradeItemsProgress.UpgradeItemsData;
            int startUpgradeLevel = 0;

            foreach (IUpgradable config in configsService.UpgradablesConfigs)
                if (config.IsUpgradable)
                    upgradeItemsData.Dictionary.Add(config.ID, startUpgradeLevel);
        }
    }
}