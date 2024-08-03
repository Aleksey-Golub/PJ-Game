using System;
using UnityEngine;

internal class ResourceSource : MonoBehaviour, IInteractable
{
    private const int PLAYER_DAMAGE = 1;

    [SerializeField] private Collider2D _collider2D;
    [SerializeField] private ResourceSourceView _view;

    [SerializeField] private Resource _resourcePrefab;

    [SerializeField] private ResourceConfig _resourceConfig;
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
        Logger.Log($"Interact with {gameObject.name} {Time.frameCount}");

        _currentHitPoints -= PLAYER_DAMAGE;
        _view.ShowHP(_currentHitPoints, _hitPoints);
        _view.ShowHitEffect();
        _view.PlayHitSound();

        if (_currentHitPoints <= 0)
        {
            Exhaust();
            DropResource();
            return;
        }

        _view.ShowHitAnimation();
    }

    private void DropResource()
    {
        var resource = Instantiate(_resourcePrefab, transform.position, Quaternion.identity);
        resource.Init(_resourceConfig);

        resource.MoveAfterDrop();
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
        _view.ShowHP(_currentHitPoints, _hitPoints);
    }
}
