using Code.Services;

namespace Code.Infrastructure
{
    public class GameLoopState : IState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly IGameFactory _gameFactory;
        private readonly IResourceFactory _resourceFactory;

        public GameLoopState(GameStateMachine stateMachine, IGameFactory gameFactory, IResourceFactory resourceFactory)
        {
            _stateMachine = stateMachine;
            _gameFactory = gameFactory;
            _resourceFactory = resourceFactory;
        }

        public void Enter()
        {
        }

        public void Exit()
        {
            _gameFactory.Cleanup();
            _resourceFactory.Cleanup();
        }
    }
}