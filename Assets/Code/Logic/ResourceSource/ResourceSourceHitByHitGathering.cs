using UnityEngine;

internal class ResourceSourceHitByHitGathering : ResourceSource
{
    protected override bool DropConditionIsTrue() => true;

    protected override void OnUpdate(float deltaTime)
    {
        if (_currentHitPoints == _hitPoints)
            return;

        if (IsSingleUse)
            return;

        _restorationTimer += deltaTime;

        if (_restorationTimer >= _restoreTime)
        {
            _restorationTimer = 0;
            RestoreHP(1);
        }
    }
}
