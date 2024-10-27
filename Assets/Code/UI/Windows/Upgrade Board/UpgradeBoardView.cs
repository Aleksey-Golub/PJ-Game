using Code.Services;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class UpgradeBoardView : MonoBehaviour
    {
        [SerializeField] private UpgradeItemView _prefab;
        [SerializeField] private Transform _content;
        [SerializeField] private TextMeshProUGUI _header;
        [SerializeField] private Button _closeButton;
        [SerializeField] private AudioClip _closeButtonClickedClip;

        private IConfigsService _configService;
        private IPersistentProgressService _progressService;
        private IAudioService _audio;
        
        private Dictionary<string, UpgradeItemView> _views;
        private Action<string> _upgradeResourceCalback;

        internal void Coustruct(IConfigsService configService, IPersistentProgressService progressService, IAudioService audio)
        {
            _views = new();
            _configService = configService;
            _progressService = progressService;
            _audio = audio;

            FillViews();

            _closeButton.onClick.AddListener(CloseFromUI);
        }

        private void OnDestroy()
        {
            _closeButton.onClick.RemoveListener(CloseFromUI);

            foreach (var view in _views.Values)
            {
                view.UpgradeButtonClicked -= OnUpgradeButtonClicked;
            }
        }

        internal void Open(Action<string> upgradeResourceCalback)
        {
            _upgradeResourceCalback = upgradeResourceCalback;
            gameObject.SetActive(true);

            _progressService.Progress.PlayerProgress.UpgradeItemsProgress.Changed += UpgradeItemsProgress_Changed;

            Refresh();
        }

        internal void Refresh()
        {
            foreach (IUpgradable config in _configService.UpgradablesConfigs)
            {
                if (!config.IsUpgradable)
                    continue;

                int maxLevel = config.GetMaxLevel();
                string MAX = "MAX";
                string itemId = config.ID;
                _progressService.Progress.PlayerProgress.UpgradeItemsProgress.TryGet(itemId, out int currentLevel);
                int nextLevel = currentLevel + 1;

                string levelText = currentLevel >= maxLevel ? $"Lvl {MAX}" : $"Lvl {nextLevel}";
                if (currentLevel >= maxLevel)
                    nextLevel = maxLevel;

                string upgradeText = GetUpgradeText(config, nextLevel);
                string upgradeCostText = $"{config.GetUpgradeData(nextLevel).Cost}";
                bool showButton = currentLevel < maxLevel && currentLevel != 0;

                _views[config.ID].SetData(upgradeText, levelText, upgradeCostText, showButton);
            }

            SortViews();

            void SortViews()
            {
                int index = 0;
                foreach (IUpgradable config in _configService.UpgradablesConfigs)
                {
                    string id = config.ID;

                    if (!config.IsUpgradable)
                        continue;

                    if (!_views[id].ShowButton)
                        continue;

                    _views[id].transform.SetSiblingIndex(index);
                    index++;
                }

                foreach (IUpgradable config in _configService.UpgradablesConfigs)
                {
                    string id = config.ID;

                    if (!config.IsUpgradable)
                        continue;

                    if (_views[id].ShowButton)
                        continue;

                    _views[id].transform.SetSiblingIndex(index);
                    index++;
                }
            }
        }

        private static string GetUpgradeText(IUpgradable config, int nextLevel)
        {
            switch (config.UpgradableType)
            {
                case UpgradableType.Tool:
                    return $"Drop: {config.GetUpgradeData(nextLevel).Value * 100}%";
                case UpgradableType.ResourceStorage:
                case UpgradableType.Converter:
                    return $"Capacity: {config.GetUpgradeData(nextLevel).Value}";
                case UpgradableType.None:
                default:
                    throw new NotImplementedException($"[UpgradeBoardView] Not implemented for {config.UpgradableType}");
            }
        }

        internal void Close()
        {
            _progressService.Progress.PlayerProgress.UpgradeItemsProgress.Changed -= UpgradeItemsProgress_Changed;

            _upgradeResourceCalback = null;
            gameObject.SetActive(false);
        }

        private void CloseFromUI()
        {
            _audio.PlaySfxAtUI(_closeButtonClickedClip);
            Close();
        }

        private void FillViews()
        {
            foreach (IUpgradable config in _configService.UpgradablesConfigs)
            {
                if (!config.IsUpgradable)
                    continue;

                var upgradeItemView = Instantiate(_prefab, _content);
                upgradeItemView.Construct(_audio);

                string id = config.ID;
                Sprite sprite = config.Sprite;
                
                upgradeItemView.Init(sprite, id);
                
                upgradeItemView.UpgradeButtonClicked += OnUpgradeButtonClicked;

                _views.Add(id, upgradeItemView);
                
            }
        }

        private void OnUpgradeButtonClicked(string itemID)
        {
            _upgradeResourceCalback?.Invoke(itemID);
        }

        private void UpgradeItemsProgress_Changed(string itemId, int newValue) => Refresh();
    }
}