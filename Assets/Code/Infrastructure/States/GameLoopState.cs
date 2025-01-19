using Code.Services;
using Code.UI.Services;

namespace Code.Infrastructure
{
    public class GameLoopState : IState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly IGameFactory _gameFactory;
        private readonly IResourceFactory _resourceFactory;
        private readonly IEffectFactory _effectFactory;
        private readonly IPopupFactory _popupFactory;
        private readonly IToolFactory _toolFactory;
        private readonly ITransitionalResourceFactory _transitionalResourceFactory;
        private readonly IAdsService _adsService;
        private readonly IUIMediator _uiMediator;

        public GameLoopState(
            GameStateMachine stateMachine,
            IGameFactory gameFactory,
            IResourceFactory resourceFactory,
            IEffectFactory effectFactory,
            IPopupFactory popupFactory,
            IToolFactory toolFactory,
            ITransitionalResourceFactory transitionalResourceFactory,
            IAdsService adsService,
            IUIMediator uiMediator
            )
        {
            _stateMachine = stateMachine;
            _gameFactory = gameFactory;
            _resourceFactory = resourceFactory;
            _effectFactory = effectFactory;
            _popupFactory = popupFactory;
            _toolFactory = toolFactory;
            _transitionalResourceFactory = transitionalResourceFactory;
            _adsService = adsService;
            _uiMediator = uiMediator;
        }

        public void Enter()
        {
            PlatformLayer.SetGameReady();

            _adsService.AdsExceptStickyCalling += OnAdsExceptStickyCalling;
            _adsService.AdsExceptStickyClose += OnAdsExceptStickyClose;

            _adsService.ShowSticky();
            _adsService.ShowPreloader();
        }

        public void Exit()
        {
            _adsService.AdsExceptStickyCalling -= OnAdsExceptStickyCalling;
            _adsService.AdsExceptStickyClose -= OnAdsExceptStickyClose;

            _gameFactory.Cleanup();
            _resourceFactory.Cleanup();
            _effectFactory.Cleanup();
            _popupFactory.Cleanup();
            _toolFactory.Cleanup();
            _transitionalResourceFactory.Cleanup();
        }

        private void OnAdsExceptStickyCalling() => _uiMediator.OpenPopup(UIPopupId.AdWaiting);
        private void OnAdsExceptStickyClose(bool result) => _uiMediator.ClosePopup(UIPopupId.AdWaiting);
    }
}