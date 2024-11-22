using System;

namespace Code.Services
{
    public interface IAdsService : IService
    {
        event Action RewardedVideoReady;

        void Initialize();
        bool IsRewardedVideoReady();
        void ShowRewardedVideo(Action onVideoFinished);
        int GetRewardBasedOnInventory(ResourceType resourceType, int inInventory);
    }
}