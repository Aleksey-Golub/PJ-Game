using System;

namespace Code.Services
{
    public interface IAdsService : IService
    {
        event Action RewardedVideoReady;

        event Action AdsStart;
        event Action AdsExceptStickyStart;
        event Action StickyStart;
        event Action PreloaderStart;
        event Action FullscreenStart;
        event Action RewardedStart;

        event Action<bool> AdsClose;
        event Action<bool> AdsExceptStickyClose;
        event Action StickyClose;
        event Action<bool> PreloaderClose;
        event Action<bool> FullscreenClose;
        event Action<bool> RewardedClose;

        void Initialize();
        bool IsRewardedVideoReady();
        void ShowRewardedVideo(Action onVideoFinished);
        int GetRewardBasedOnInventory(ResourceType resourceType, int inInventory);
        void ShowSticky();
        void ShowPreloader();
        void ShowFullscreen();
    }
}