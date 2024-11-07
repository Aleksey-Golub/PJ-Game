namespace Code.Infrastructure
{
    public class MainMenuState : IState
    {
        private readonly GameStateMachine _gameStateMachine;

        public MainMenuState(GameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }

        public void Enter()
        {
            _gameStateMachine.Enter<LoadProgressState>();
        }

        public void Exit()
        {
        }
    }
}