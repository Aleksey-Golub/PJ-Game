using Code.Services;
using Code.UI.Services;

namespace Code.Infrastructure
{
    public class BootstrapState : IState
    {
        private const string INITIAL = "Initial";

        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly AllServices _services;

        public BootstrapState(GameStateMachine stateMachine, SceneLoader sceneLoader, AllServices services, ICoroutineRunner coroutineRunner, IUpdater updater)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _services = services;

            RegisterServices(coroutineRunner, updater);
        }

        public void Enter() =>
          _sceneLoader.Load(INITIAL, onLoaded: EnterLoadLevel);

        public void Exit()
        {
        }

        private void RegisterServices(ICoroutineRunner coroutineRunner, IUpdater updater)
        {
            _services.RegisterSingle<IUpdater>(updater);
            _services.RegisterSingle<IAssetProvider>(new AssetProvider());
            _services.RegisterSingle<ICoroutineRunner>(coroutineRunner);

            RegisterConfigService();
            RegisterInputService();
            _services.RegisterSingle<IPersistentProgressService>(new PersistentProgressService());
            RegisterAudioService(coroutineRunner);
            _services.RegisterSingle<IDropCountCalculatorService>(new DropCountCalculatorService(_services.Single<IPersistentProgressService>(), _services.Single<IConfigsService>()));

            _services.RegisterSingle<IPopupFactory>(new PopupFactory(_services.Single<IAudioService>(), _services.Single<IAssetProvider>()));
            _services.RegisterSingle<IResourceFactory>(new ResourceFactory(_services.Single<IAudioService>(), _services.Single<IAssetProvider>()));
            _services.RegisterSingle<IToolFactory>(new ToolFactory(_services.Single<IAudioService>(), _services.Single<IAssetProvider>()));
            _services.RegisterSingle<ITransitionalResourceFactory>(new TransitionalResourceFactory(_services.Single<IAudioService>(), _services.Single<IAssetProvider>()));

            RegisterResourceMergeService();
            _services.RegisterSingle<IUIFactory>(new UIFactory(
              _services.Single<IAssetProvider>(),
              _services.Single<IConfigsService>(),
              _services.Single<IPersistentProgressService>(),
              _services.Single<IAudioService>()
              ));

            _services.RegisterSingle<IUIMediator>(new UIMediator(_services.Single<IUIFactory>()));

            _services.RegisterSingle<IGameFactory>(new GameFactory(
              _services.Single<IAssetProvider>(),
              _services.Single<IConfigsService>(),
              _services.Single<IPersistentProgressService>(),
              _services.Single<IUIMediator>(),
              _services.Single<IAudioService>(),
              _services.Single<IInputService>(),
              _services.Single<IPopupFactory>(),
              _services.Single<ITransitionalResourceFactory>()
              ));

            _services.RegisterSingle<ISaveLoadService>(new SaveLoadService(
              _services.Single<IPersistentProgressService>(),
              _services.Single<IGameFactory>()));
        }

        private void RegisterResourceMergeService()
        {
            IResourceMergeService rms = new ResourceMergeService(_services.Single<IResourceFactory>());
            rms.Load();
            _services.RegisterSingle(rms);

            _services.Single<IUpdater>().Updatables.Add(rms);
        }

        private void RegisterAudioService(ICoroutineRunner coroutineRunner)
        {
            IAudioService audio = new AudioService(coroutineRunner);
            audio.Load();
            _services.RegisterSingle(audio);
        }

        private void RegisterInputService()
        {
            IInputService input = GetInputService();
            input.Init();
            _services.RegisterSingle<IInputService>(input);
        }

        private void RegisterConfigService()
        {
            IConfigsService configs = new ConfigsService();
            configs.Load();
            _services.RegisterSingle(configs);
        }

        private void EnterLoadLevel() =>
          _stateMachine.Enter<LoadProgressState>();

        private static IInputService GetInputService()
        {
#if UNITY_STANDALONE
            return new DesktopInput();
#else
            return new MobileInput();
#endif
        }
    }
}