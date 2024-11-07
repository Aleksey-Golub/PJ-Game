using Code.Services;

namespace Code.Infrastructure
{
    public interface IGameStateMachine : IService
    {
        void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>;
        void Enter<TState>() where TState : class, IState;
    }
}