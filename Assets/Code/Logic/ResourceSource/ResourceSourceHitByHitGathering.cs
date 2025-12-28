using UnityEngine;

internal class ResourceSourceHitByHitGathering : ResourceSource
{
    [SerializeField][Range(1, 2)] private int _hitsToDropOnePortionOfResource;

    protected override bool DropConditionIsTrue() => _currentHitPoints % _hitsToDropOnePortionOfResource == 0;

    protected override void OnUpdate(float deltaTime)
    {
        // we cannot restore over max
        if (_currentHitPoints + _hitsToDropOnePortionOfResource > _hitPoints)
            return;

        if (IsSingleUse)
            return;

        _restorationTimer += deltaTime;

        // we restore hitsToDropOnePortionOfResource count together
        if (_restorationTimer >= _restoreTime * _hitsToDropOnePortionOfResource)
        {
            _restorationTimer = 0;
            RestoreHP(_hitsToDropOnePortionOfResource);
        }
    }

    protected override void Exhaust() { }
}
