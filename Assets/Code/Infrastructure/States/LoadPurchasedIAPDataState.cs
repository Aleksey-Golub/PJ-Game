using Code.Data;
using Code.Services;
using System;

namespace Code.Infrastructure
{
    public class LoadPurchasedIAPDataState : IState
    {
        private readonly GameStateMachine _gameStateMachine;
        private readonly IIAPService _iapService;
        private readonly ISaveLoadIAPDataService _saveLoadIAPDataService;

        public LoadPurchasedIAPDataState(
            GameStateMachine gameStateMachine,
            IIAPService iapService,
            ISaveLoadIAPDataService saveLoadIAPDataService
            )
        {
            _gameStateMachine = gameStateMachine;
            _iapService = iapService;
            _saveLoadIAPDataService = saveLoadIAPDataService;
        }

        public void Enter()
        {
            try
            {
                LoadPurchasedIAPDataOrInitNew();
            }
            catch (Exception e)
            {
                Logger.LogError($"[LoadPurchasedIAPDataState] Exception on LoadPurchasedIAPDataOrInitNew: {e}");
            }

            _iapService.Fetch();
            InformPurchasedIAPDataReaders();

            _gameStateMachine.Enter<LoadProgressState>();
        }

        public void Exit()
        {
        }

        private void LoadPurchasedIAPDataOrInitNew()
        {
            _iapService.PlayerIAPsData =
              _saveLoadIAPDataService.LoadIAPData()
              ?? NewPurchasedIAPData();
        }

        private PlayerIAPsData NewPurchasedIAPData()
        {
            Logger.Log($"[LoadPurchasedIAPDataState] call NewPurchasedIAPData()");

            var data = new PlayerIAPsData();
            // set default data here

            return data;
        }

        private void InformPurchasedIAPDataReaders()
        {
            PlayerIAPsData data = _iapService.PlayerIAPsData;

            // inform here
        }
    }
}