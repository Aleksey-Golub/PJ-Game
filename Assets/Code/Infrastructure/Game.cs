using Code.Services;

namespace Code.Infrastructure
{
    public class Game
    {
        private readonly IAudioService _audio;
        private readonly ITimeService _time;

        public readonly GameStateMachine StateMachine;

        public Game(ICoroutineRunner coroutineRunner, IUpdater updater, LoadingCurtain curtain)
        {
            var services = AllServices.Container;
            StateMachine = new GameStateMachine(new SceneLoader(coroutineRunner), curtain, services, coroutineRunner, updater);

            _audio = services.Single<IAudioService>();
            _time = services.Single<ITimeService>();

            var ads = services.Single<IAdsService>();
            ads.AdsExceptStickyStart += OnAdsExceptStickyStart;
            ads.AdsExceptStickyClose += OnAdsExceptStickyClose;
        }

        private void OnAdsExceptStickyStart()
        {
            _audio.PauseAll();
            _time.StopTime();
        }

        private void OnAdsExceptStickyClose(bool obj)
        {
            _audio.UnPauseAll();
            _time.ResumeTime();
        }
    }
}