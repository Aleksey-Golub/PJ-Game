using System;

#if DEBUG && FAKE_ADS
#else
using GamePush;
#endif

namespace Code.Services
{
    public class AdsService : IAdsService
    {
        private readonly IIAPService _iapService;
        private readonly IPersistentProgressService _progressService;

        public event Action RewardedVideoReady;

        public event Action AdsExceptStickyCalling;

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

        private Action _onRewardedReward;
        private bool _canStartShowFullscreenByTrigger;

        public bool IsAdsExceptStickyShowing => IsPreloaderShowing || IsFullscreenShowing || IsRewardedShowing;

        public bool IsStickyShowing
        {
            get
            {
#if DEBUG && FAKE_ADS
                return false;
#else
                return GP_Init.isReady && GP_Ads.IsStickyPlaying();
#endif
            }
        }

        public bool IsPreloaderShowing
        {
            get
            {
#if DEBUG && FAKE_ADS
                return false;
#else
                return GP_Init.isReady && GP_Ads.IsPreloaderPlaying();
#endif
            }
        }

        public bool IsFullscreenShowing
        {
            get
            {
#if DEBUG && FAKE_ADS
                return false;
#else
                return GP_Init.isReady && GP_Ads.IsFullscreenPlaying();
#endif
            }
        }

        public bool IsRewardedShowing
        {
            get
            {
#if DEBUG && FAKE_ADS
                return false;
#else
                return GP_Init.isReady && GP_Ads.IsRewardPlaying();
#endif
            }
        }

        public AdsService(IIAPService iapService, IPersistentProgressService progressService)
        {
            _iapService = iapService;
            _progressService = progressService;

            _iapService.Purchased += OnSomePurchased;
        }

        public void Initialize()
        {
            Logger.Log($"[AdsService] initializing...");

#if DEBUG && FAKE_ADS
#else
            SubscribeOnGP_AdsEvents();
#endif
            Logger.Log($"[AdsService] end initializing...");
        }

        public bool IsStickyAvailable()
        {
#if DEBUG && FAKE_ADS
            return true;
#else
            return GP_Ads.IsStickyAvailable();
#endif
        }

        public bool IsPreloaderAvailable()
        {
#if DEBUG && FAKE_ADS
            return true;
#else
            return GP_Ads.IsPreloaderAvailable();
#endif
        }

        public bool IsFullscreenAvailable()
        {
            //return false; //
#if DEBUG && FAKE_ADS
            return true;
#else
            return GP_Ads.IsFullscreenAvailable();
#endif
        }

        public bool IsRewardedAvailable()
        {
#if DEBUG && FAKE_ADS
            return true;
#else
            return GP_Ads.IsRewardedAvailable();
#endif
        }

        public void ShowSticky()
        {
            Logger.Log($"[AdsService] start ShowSticky()");

            if (!IsStickyAvailable())
            {
                Logger.LogWarning($"[AdsService] trying to show Sticky, but it is not available");
                return;
            }

            if (_iapService.IsPremiumBought())
            {
                Logger.LogWarning($"[AdsService] trying to show Sticky, but PREMIUM is bought");
                return;
            }

#if DEBUG && FAKE_ADS
            StickyStart?.Invoke();
            AdsStart?.Invoke();

            StickyClose?.Invoke();
            AdsClose?.Invoke(true);
#else
            GP_Ads.ShowSticky();
#endif
        }
        
        public void CloseSticky()
        {
            Logger.Log($"[AdsService] start CloseSticky()");

            if (!IsStickyAvailable())
            {
                Logger.LogWarning($"[AdsService] trying to close Sticky, but it is not available");
                return;
            }

#if DEBUG && FAKE_ADS
#else
            GP_Ads.CloseSticky();
#endif
        }

        public void ShowPreloader()
        {
            Logger.Log($"[AdsService] start ShowPreloader()");

            if (!IsPreloaderAvailable())
            {
                Logger.LogWarning($"[AdsService] trying to show Preload, but it is not available");
                return;
            }

            if (_iapService.IsPremiumBought())
            {
                Logger.LogWarning($"[AdsService] trying to show Preload, but PREMIUM is bought");
                return;
            }

            AdsExceptStickyCalling?.Invoke();

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

        public bool TryStartShowFullscreenByTrigger()
        {
            Logger.Log($"[AdsService] start TryStartShowFullscreenByTrigger()");

            if (_canStartShowFullscreenByTrigger)
            {
                _canStartShowFullscreenByTrigger = false;
                ShowFullscreen();

                return true;
            }

            return false;
        }

        public void ShowFullscreen()
        {
            Logger.Log($"[AdsService] start ShowFullScreen()");

            if (!IsTutorialCompleted())
            {
                Logger.LogWarning($"[AdsService] trying to show Fullscreen, but tutorial is not completed");
                return;
            }

            if (!IsFullscreenAvailable())
            {
                Logger.LogWarning($"[AdsService] trying to show Fullscreen, but it is not available");
                return;
            }

            if (_iapService.IsPremiumBought())
            {
                Logger.LogWarning($"[AdsService] trying to show Fullscreen, but PREMIUM is bought");
                return;
            }

            AdsExceptStickyCalling?.Invoke();

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

        public void ShowRewardedVideo(Action onRewardedReward)
        {
            Logger.Log($"[AdsService] start ShowRewarded()");

            if (!IsRewardedAvailable())
            {
                Logger.LogWarning($"[AdsService] trying to show Rewarded, but it is not available");
                return;
            }

            _onRewardedReward = onRewardedReward;
            
            AdsExceptStickyCalling?.Invoke();

#if DEBUG && FAKE_ADS
            RewardedStart?.Invoke();
            AdsStart?.Invoke();
            AdsExceptStickyStart?.Invoke();

            OnRewardedVideoFinishedSuccessfully("");

            RewardedClose?.Invoke(true);
            AdsClose?.Invoke(true);
            AdsExceptStickyClose?.Invoke(true);
#else
            GP_Ads.ShowRewarded(onRewardedReward: OnRewardedVideoFinishedSuccessfully);
#endif
        }

        public int GetRewardBasedOnInventory(ResourceType resourceType, int inInventory)
        {
            return inInventory switch
            {
                < 250 => 100,
                < 500 => 125,
                < 750 => 150,
                _ => 200,
            };
        }

        private void OnRewardedVideoFinishedSuccessfully(string rewardId)
        {
            _onRewardedReward?.Invoke();
            _onRewardedReward = null;

            RewardedVideoReady?.Invoke();
        }

        private void OnSomePurchased(bool purchaseSuccessed)
        {
            if (_iapService.IsPremiumBought())
                CloseSticky();
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
            Logger.Log($"[AdsService] on ADs start");

            AdsStart?.Invoke();
        }

        private void OnStickyStart()
        {
            Logger.Log($"[AdsService] on Sticky start");

            StickyStart?.Invoke();
            AdsStart?.Invoke();
        }

        private void OnPreloaderStart()
        {
            Logger.Log($"[AdsService] on Preloader start");

            PreloaderStart?.Invoke();
            AdsStart?.Invoke();
            AdsExceptStickyStart?.Invoke();
        }

        private void OnFullscreenStart()
        {
            Logger.Log($"[AdsService] on Fullscreen start");

            FullscreenStart?.Invoke();
            AdsStart?.Invoke();
            AdsExceptStickyStart?.Invoke();
        }

        private void OnRewardedStart()
        {
            Logger.Log($"[AdsService] on Rewarded start");

            RewardedStart?.Invoke();
            AdsStart?.Invoke();
            AdsExceptStickyStart?.Invoke();
        }

        private void OnAdsClose(bool success)
        {
            Logger.Log($"[AdsService] on Ads close {success}");

            AdsClose?.Invoke(success);
        }

        private void OnStickyClose()
        {
            Logger.Log($"[AdsService] on Sticky close");

            StickyClose?.Invoke();
            AdsClose?.Invoke(true);
        }

        private void OnPreloaderClose(bool success)
        {
            Logger.Log($"[AdsService] on Preloader close {success}");

            PreloaderClose?.Invoke(success);
            AdsClose?.Invoke(success);
            AdsExceptStickyClose?.Invoke(success);
        }

        private void OnFullScreenClose(bool success)
        {
            Logger.Log($"[AdsService] on Fullscreen close {success}");

            FullscreenClose?.Invoke(success);
            AdsClose?.Invoke(success);
            AdsExceptStickyClose?.Invoke(success);

            if (success)
                Metrika.Event_AdsInterstitialSuccessed();
        }

        private void OnRewardedClose(bool success)
        {
            Logger.Log($"[AdsService] on Rewarded close {success}");

            RewardedClose?.Invoke(success);
            AdsClose?.Invoke(success);
            AdsExceptStickyClose?.Invoke(success);

            if (success)
                Metrika.Event_Ads_Rewarded_Successed();
        }

        private bool IsTutorialCompleted()
        {
            return _progressService.Progress.WorldProgress.LevelsDatasDictionary.Dictionary[Scenes.LEVEL_1_SCENE].TutorialProgress.IsCompleted;
        }

        public void SetCanStartShowFullscreenByTrigger()
        {
            Logger.Log($"[AdsService] SetCanStartShowFullscreenByTrigger()");

            _canStartShowFullscreenByTrigger = true;
        }

        #endregion
#endif
    }
}