using System;
using GamePush;

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
            SubscribeOnGP_AdsEvents();
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
            GP_Ads.ShowSticky();
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
            GP_Ads.ShowPreloader();
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
            GP_Ads.ShowFullscreen();
#endif
        }

        public bool IsRewardedVideoReady()
        {
#if DEBUG && FAKE_ADS
            return true;
#else
            return GP_Ads.IsRewardedAvailable();
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
            GP_Ads.ShowRewarded(onRewardedReward: OnRewardedVideoFinished);
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

#if DEBUG && FAKE_ADS
#else
        private void SubscribeOnGP_AdsEvents()
        {
            GP_Ads.OnAdsStart += OnAdsStart;
            GP_Ads.OnStickyStart += OnStickyStart;
            GP_Ads.OnPreloaderStart += OnPreloaderStart;
            GP_Ads.OnFullscreenStart += OnFullscreenStart;
            GP_Ads.OnRewardedStart += OnRewardedStart;

            GP_Ads.OnAdsClose += OnAdsClose;
            GP_Ads.OnStickyClose += OnStickyClose;
            GP_Ads.OnPreloaderClose += OnPreloaderClose;
            GP_Ads.OnFullscreenClose += OnFullScreenClose;
            GP_Ads.OnRewardedClose += OnRewardedClose;
        }

        #region Ads Events Handlers
        private void OnAdsStart()
        {
            Logger.Log($"[AdsService] ADs start");

            AdsStart?.Invoke();
        }

        private void OnStickyStart()
        {
            Logger.Log($"[AdsService] Sticky start");

            StickyStart?.Invoke();
            AdsStart?.Invoke();
        }

        private void OnPreloaderStart()
        {
            Logger.Log($"[AdsService] Preloader start");

            PreloaderStart?.Invoke();
            AdsStart?.Invoke();
            AdsExceptStickyStart?.Invoke();
        }

        private void OnFullscreenStart()
        {
            Logger.Log($"[AdsService] Fullscreen start");

            FullscreenStart?.Invoke();
            AdsStart?.Invoke();
            AdsExceptStickyStart?.Invoke();
        }

        private void OnRewardedStart()
        {
            Logger.Log($"[AdsService] Rewarded start");

            RewardedStart?.Invoke();
            AdsStart?.Invoke();
            AdsExceptStickyStart?.Invoke();
        }

        private void OnAdsClose(bool success)
        {
            Logger.Log($"[AdsService] Ads close {success}");

            AdsClose?.Invoke(success);
        }

        private void OnStickyClose()
        {
            Logger.Log($"[AdsService] Sticky close");

            StickyClose?.Invoke();
            AdsClose?.Invoke(true);
        }

        private void OnPreloaderClose(bool success)
        {
            Logger.Log($"[AdsService] Preloader close {success}");

            PreloaderClose?.Invoke(success);
            AdsClose?.Invoke(success);
            AdsExceptStickyClose?.Invoke(success);
        }

        private void OnFullScreenClose(bool success)
        {
            Logger.Log($"[AdsService] Fullscreen close {success}");

            FullscreenClose?.Invoke(success);
            AdsClose?.Invoke(success);
            AdsExceptStickyClose?.Invoke(success);
        }

        private void OnRewardedClose(bool success)
        {
            Logger.Log($"[AdsService] Rewarded close {success}");

            RewardedClose?.Invoke(success);
            AdsClose?.Invoke(success);
            AdsExceptStickyClose?.Invoke(success);
        }
        #endregion
#endif
    }
}