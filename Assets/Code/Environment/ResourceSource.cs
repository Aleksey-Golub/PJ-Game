using UnityEngine;

internal class ResourceSource : MonoBehaviour, IInteractable
{
    private const int PLAYER_DAMAGE = 1;

    [SerializeField] private Collider2D _collider2D;
    [SerializeField] private ResourceSourceView _view;
    [SerializeField] private int _hitPoints = 1;

    [SerializeField] private float _restoreTime = 10;
    private float _restorationTimer = 0;
    private int _currentHitPoints = 0;

    private bool IsDied => _currentHitPoints <= 0;

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        if (!IsDied)
            return;

        _restorationTimer += Time.deltaTime;

        if (_restorationTimer >= _restoreTime)
        {
            _restorationTimer = 0;
            Restore();
        }
    }

    private void Init()
    {
        RestoreHP();
    }

    public void Interact()
    {
        Logger.Log($"Interact with {gameObject.name}");

        _currentHitPoints -= PLAYER_DAMAGE;

        if (_currentHitPoints <= 0)
            Exhaust();
    }

    private void Exhaust()
    {
        _collider2D.enabled = false;
        _view.ShowExhaust();
    }

    private void Restore()
    {
        RestoreHP();
        _collider2D.enabled = true;
        _view.ShowWhole();
    }

    private void RestoreHP()
    {
        _currentHitPoints = _hitPoints;
    }
}
