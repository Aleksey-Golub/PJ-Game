using Code.Data;
using Code.Services;
using System;

namespace Code.Infrastructure
{
    public class LoadAnalyticEventsState : IState
    {
        private readonly GameStateMachine _gameStateMachine;
        private readonly IAnalyticEventsService _analyticEventsService;
        private readonly ISaveLoadAnalyticService _saveLoadAnalyticService;

        public LoadAnalyticEventsState(
            GameStateMachine gameStateMachine,
            IAnalyticEventsService analyticEventsService,
            ISaveLoadAnalyticService saveLoadAnalyticService
            )
        {
            _gameStateMachine = gameStateMachine;
            _analyticEventsService = analyticEventsService;
            _saveLoadAnalyticService = saveLoadAnalyticService;
        }

        public void Enter()
        {
            try
            {
                LoadAnalyticEventsOrInitNew();
            }
            catch (Exception e)
            {
                Logger.LogError($"[LoadAnalyticEventsState] Exception on LoadAnalyticEventsOrInitNew: {e}");
            }

            InformAnalyticEventsReaders();

            _gameStateMachine.Enter<LoadAppSettingsState>();
        }

        public void Exit()
        {
        }

        private void LoadAnalyticEventsOrInitNew()
        {
            _analyticEventsService.AnalyticData =
              _saveLoadAnalyticService.LoadAnalytic()
              ?? NewAnalyticEvents();
        }

        private AnalyticData NewAnalyticEvents()
        {
            Logger.Log($"[LoadAnalyticEventsState] call NewAnalyticEvents()");

            var data = new AnalyticData();
            // set default data here

            return data;
        }

        private void InformAnalyticEventsReaders()
        {
            AnalyticData data = _analyticEventsService.AnalyticData;

            Metrika.Initialize(_saveLoadAnalyticService, data);
        }
    }
}