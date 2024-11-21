using UnityEngine;

internal class BootsAdsObject : AdsObjectBase
{
    [SerializeField] private float _boostSpeed = 5f;
    [SerializeField] private float _boostTime = 180f;

    protected override void OnRewardedVideoEndSuccessfully()
    {
        base.OnRewardedVideoEndSuccessfully();

        Logger.Log($"[Boots] {gameObject.name} - do speed");
        Player.SpeedUp(_boostSpeed, _boostTime);
    }
}