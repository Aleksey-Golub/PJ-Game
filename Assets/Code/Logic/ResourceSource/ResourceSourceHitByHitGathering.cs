using UnityEngine;

internal class ResourceSourceHitByHitGathering : ResourceSource
{
    internal override void Interact()
    {
        //Logger.Log($"Interact with {gameObject.name} {Time.frameCount}");

        _currentHitPoints -= PLAYER_DAMAGE;
        _view.ShowHP(_currentHitPoints, _hitPoints);
        _view.ShowHitEffect();
        _view.PlayHitSound();

        DropResource();
        _view.ShowHitAnimation();
    }

    protected override void OnUpdate()
    {
        if (_currentHitPoints == _hitPoints)
            return;

        if (_restoreTime < 0)
            gameObject.SetActive(false);

        _restorationTimer += Time.deltaTime;

        if (_restorationTimer >= _restoreTime)
        {
            _restorationTimer = 0;
            RestoreHP(1);
        }
    }
}
