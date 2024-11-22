using Code.Infrastructure;
using Code.Services;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    internal class ShowAdsButton : MonoBehaviour, IUpdatable
    {
        [SerializeField] private GameObject _self;
        [SerializeField] private TextMeshProUGUI _restoreTimerText;
        [SerializeField] private Button _button;
        [SerializeField] private Image _rewardIcon;
        [SerializeField] private TextMeshProUGUI _rewardAmountText;
        [Space]
        [SerializeField] private ResourceType _rewardResourceType = ResourceType.COIN;
        [SerializeField] private float _restoreTime = 60f;

        private IAdsService _adsService;
        private IConfigsService _configs;
        private IUpdater _updater;
        private Timer _restorationTimer;

        private Inventory _inventory;
        private int _rewardAmount;
        private float _timeLeftPrev;

        internal void Construct(IAdsService adsService, IConfigsService configs, IUpdater updater)
        {
            _adsService = adsService;
            _configs = configs;
            _updater = updater;
            _updater.Updatables.Add(this);

            _restorationTimer = new Timer(isElapsed: true);
        }

        internal void SubscribeUpdates()
        {
            _button.onClick.AddListener(OnClick);

            _adsService.RewardedVideoReady += OnRewardedVideoReady;
            _restorationTimer.Elapsed += OnRestorationTimerElapsed;
        }

        internal void Init(Inventory inventory)
        {
            _inventory = inventory;
            _rewardAmount = _adsService.GetRewardBasedOnInventory(_rewardResourceType, inInventory: inventory.Storage[_rewardResourceType]);

            _rewardAmountText.text = $"+{_rewardAmount}";
            _rewardIcon.sprite = _configs.GetConfigFor(_rewardResourceType).Sprite;

            ShowState();
        }

        private void OnDestroy()
        {
            _updater.Updatables.Remove(this);
        }

        internal void Cleanup()
        {
            _button.onClick.RemoveListener(OnClick);

            _adsService.RewardedVideoReady -= OnRewardedVideoReady;
            _restorationTimer.Elapsed -= OnRestorationTimerElapsed;
        }

        public void OnUpdate(float deltaTime)
        {
            _restorationTimer.OnUpdate(deltaTime);

            if (_restorationTimer.IsStarted)
            {
                float timeLeft = _restorationTimer.Duration - _restorationTimer.Passed;
                // optimization: prevent update text each frame
                if (_timeLeftPrev - timeLeft >= 1f)
                {
                    _timeLeftPrev = timeLeft;
                    // +1 becaus of float rounded donw
                    _restoreTimerText.text = $"{TimeSpan.FromSeconds(timeLeft + 1).ToString("mm':'ss")}";
                }
            }
        }

        private void OnClick()
        {
            _restorationTimer.Start(_restoreTime);
            _timeLeftPrev = _restoreTime;
            _restoreTimerText.text = $"{TimeSpan.FromSeconds(_restoreTime).ToString("mm':'ss")}";

            ShowState();
            _adsService.ShowRewardedVideo(() => _inventory.Add(_rewardResourceType, _rewardAmount));
        }

        private void OnRewardedVideoReady() => ShowState();
        private void OnRestorationTimerElapsed(Timer timer) => ShowState();

        private void ShowState()
        {
            if (!_restorationTimer.IsElapsed)
            {
                _self.gameObject.SetActive(false);
                _restoreTimerText.gameObject.SetActive(true);
            }
            else if (!_adsService.IsRewardedVideoReady())
            {
                _self.gameObject.SetActive(false);
                _restoreTimerText.gameObject.SetActive(false);
            }
            else
            {
                _self.gameObject.SetActive(true);
                _restoreTimerText.gameObject.SetActive(false);
            }
        }
    }
}
