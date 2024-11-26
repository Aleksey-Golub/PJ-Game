using System;

namespace Code.Services
{
    public class AdsService : IAdsService
    {
        public event Action RewardedVideoReady;

        private Action _onVideoFinished;

        public void Initialize()
        {
            Logger.Log($"[AdsService] initializing...");
        }

        public bool IsRewardedVideoReady()
        {
#if DEBUG && FAKE_LOADED_ADS
            return true;
#else
            //return Advertisement.IsReady(_placementId);
            return false;
#endif
        }

        public void ShowRewardedVideo(Action onVideoFinished)
        {
            _onVideoFinished = onVideoFinished;

            //Advertisement.Show(_placementId);

            OnRewardedVideoFinished();
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

        private void OnRewardedVideoFinished()
        {
            _onVideoFinished?.Invoke();
            _onVideoFinished = null;
        }
    }
}