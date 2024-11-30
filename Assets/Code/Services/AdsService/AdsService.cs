using System;

namespace Code.Services
{
    public class AdsService : IAdsService
    {
        public event Action RewardedVideoReady;

        public event Action AdsStart;
        public event Action AdsExceptStickyStart;
        public event Action StickyStart;
        public event Action PreloaderStart;
        public event Action FullscreenStart;
        public event Action RewardedStart;

        public event Action<bool> AdsClose;
        public event Action<bool> AdsExceptStickyClose;
        public event Action StickyClose;
        public event Action<bool> PreloaderClose;
        public event Action<bool> FullscreenClose;
        public event Action<bool> RewardedClose;

        private Action _onVideoFinished;

        public void Initialize()
        {
            Logger.Log($"[AdsService] initializing...");

#if DEBUG && FAKE_ADS
#else
#endif
            Logger.Log($"[AdsService] end initializing...");
        }

        public void ShowSticky()
        {
#if DEBUG && FAKE_ADS
            StickyStart?.Invoke();
            AdsStart?.Invoke();

            StickyClose?.Invoke();
            AdsClose?.Invoke(true);
#else
#endif
        }

        public void ShowPreloader()
        {
#if DEBUG && FAKE_ADS
            PreloaderStart?.Invoke();
            AdsStart?.Invoke();
            AdsExceptStickyStart?.Invoke();

            PreloaderClose?.Invoke(true);
            AdsClose?.Invoke(true);
            AdsExceptStickyClose?.Invoke(true);
#else
#endif
        }

        public void ShowFullscreen()
        {
#if DEBUG && FAKE_ADS
            FullscreenStart?.Invoke();
            AdsStart?.Invoke();
            AdsExceptStickyStart?.Invoke();

            FullscreenClose?.Invoke(true);
            AdsClose?.Invoke(true);
            AdsExceptStickyClose?.Invoke(true);
#else
#endif
        }

        public bool IsRewardedVideoReady()
        {
#if DEBUG && FAKE_ADS
            return true;
#else
            return false;
#endif
        }

        public void ShowRewardedVideo(Action onVideoFinished)
        {
            _onVideoFinished = onVideoFinished;

#if DEBUG && FAKE_ADS
            RewardedStart?.Invoke();
            AdsStart?.Invoke();
            AdsExceptStickyStart?.Invoke();

            OnRewardedVideoFinished("");

            RewardedClose?.Invoke(true);
            AdsClose?.Invoke(true);
            AdsExceptStickyClose?.Invoke(true);
#else
#endif
        }

        public int GetRewardBasedOnInventory(ResourceType resourceType, int inInventory)
        {
            return inInventory switch
            {
                < 250 => 25,
                < 500 => 50,
                < 750 => 75,
                _ => 100,
            };
        }

        private void OnRewardedVideoFinished(string rewardId)
        {
            _onVideoFinished?.Invoke();
            _onVideoFinished = null;

            RewardedVideoReady?.Invoke();
        }
        }
    }
}