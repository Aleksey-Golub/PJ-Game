using Code.Services;
using Code.UI.Services;
using System;
using System.Collections.Generic;

namespace Code.Infrastructure
{
  public class GameStateMachine : IGameStateMachine
  {
    private readonly Dictionary<Type, IExitableState> _states;
    private IExitableState _activeState;

    public GameStateMachine(SceneLoader sceneLoader, LoadingCurtain loadingCurtain, AllServices services, ICoroutineRunner coroutineRunner, IUpdater updater)
    {
      _states = new Dictionary<Type, IExitableState>
      {
        [typeof(BootstrapState)] = new BootstrapState(this, sceneLoader, services, coroutineRunner, updater),
        [typeof(LoadAppSettingsState)] = new LoadAppSettingsState(
            this, 
            services.Single<IAppSettingsService>(), 
            services.Single<ISaveLoadAppSettingsService>(),
            services.Single<IAudioService>()
            ),
        [typeof(LoadMainMenuState)] = new LoadMainMenuState(
            this,
            sceneLoader
            ),
        [typeof(MainMenuState)] = new MainMenuState(
            this
            ),
        [typeof(LoadProgressState)] = new LoadProgressState(
            this, 
            services.Single<IPersistentProgressService>(), 
            services.Single<IConfigsService>(), 
            services.Single<ISaveLoadService>()
            ),
        [typeof(LoadLevelState)] = new LoadLevelState(
            this, 
            sceneLoader, 
            loadingCurtain, 
            services.Single<IGameFactory>(), 
            services.Single<IResourceFactory>(), 
            services.Single<IEffectFactory>(), 
            services.Single<IPopupFactory>(), 
            services.Single<IToolFactory>(), 
            services.Single<ITransitionalResourceFactory>(), 
            services.Single<IPersistentProgressService>(), 
            services.Single<IUIFactory>(),
            services.Single<IUIMediator>(),
            services.Single<IAudioService>(),
            services.Single<IConfigsService>()
            ),
        [typeof(GameLoopState)] = new GameLoopState(
            this,
            services.Single<IGameFactory>(), 
            services.Single<IResourceFactory>(),
            services.Single<IEffectFactory>(),
            services.Single<IPopupFactory>(),
            services.Single<IToolFactory>(),
            services.Single<ITransitionalResourceFactory>()
            ),
      };
    }
    
    public void Enter<TState>() where TState : class, IState
    {
      IState state = ChangeState<TState>();
      state.Enter();
    }

    public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
    {
      TState state = ChangeState<TState>();
      state.Enter(payload);
    }

    private TState ChangeState<TState>() where TState : class, IExitableState
    {
      _activeState?.Exit();
      
      TState state = GetState<TState>();
      _activeState = state;
      
      return state;
    }

    private TState GetState<TState>() where TState : class, IExitableState => 
      _states[typeof(TState)] as TState;
  }
}