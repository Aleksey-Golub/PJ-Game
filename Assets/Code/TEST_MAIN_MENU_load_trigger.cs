using Code.Infrastructure;
using Code.Services;
using UnityEngine;

public class TEST_MAIN_MENU_load_trigger : MonoBehaviour
{
    private bool _isTriggered;
    private IGameStateMachine _stateMachine;

    private void Start()
    {
        _stateMachine = AllServices.Container.Single<IGameStateMachine>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isTriggered)
            return;

        if (collision.TryGetComponent<Player>(out Player player))
        {
            _stateMachine.Enter<MainMenuState>();
            _isTriggered = true;
        }
    }
}
