using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.UI
{
    internal class UpgradeBoardView : MonoBehaviour
    {
        [SerializeField] private UpgradeItemView _prefab;
        [SerializeField] private Transform _content;
        [SerializeField] private TextMeshProUGUI _header;
        [SerializeField] private Button _closeButton;
        [SerializeField] private AudioClip _closeButtonClickedClip;

        private ConfigsService _configService;
        private PersistentProgressService _progressService;
        private Dictionary<string, UpgradeItemView> _views;
        private Action<string> _upgradeResourceCalback;

        internal Action<ToolType> UpgradeButtonClicked;

        internal void Coustruct(ConfigsService configService, PersistentProgressService progressService)
        {
            _views = new();
            _configService = configService;
            _progressService = progressService;
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

            Refresh();
        }

        internal void Refresh()
        {
            foreach (ToolConfig toolConfig in _configService.ToolsConfigs.Values)
            {
                if (!toolConfig.IsUpgradable)
                    continue;

                int maxLevel = toolConfig.UpgradeStaticDatas.Count;
                string MAX = "MAX";
                string itemId = toolConfig.ID;
                int currentLevel = _progressService.Progress.PlayerProgress.UpgradeItemsProgress.UpgradeItemsData.Dictionary[itemId];
                int nextLevel = currentLevel + 1;
                int nextLevelIndex = nextLevel - 1;
                
                string levelText = currentLevel >= maxLevel ? $"Lvl {MAX}" : $"Lvl {nextLevel}";
                if (currentLevel >= maxLevel)
                    nextLevelIndex = maxLevel - 1;

                string upgradeText = $"Drop {toolConfig.UpgradeStaticDatas[nextLevelIndex].Value * 100}%";
                string upgradeCostText = $"{toolConfig.UpgradeStaticDatas[nextLevelIndex].Cost}";
                bool showButton = currentLevel < maxLevel && currentLevel != 0;

                _views[toolConfig.ID].SetData(upgradeText, levelText, upgradeCostText, showButton);
            }
        }

        internal void Close()
        {
            _upgradeResourceCalback = null;
            gameObject.SetActive(false);
        }

        private void CloseFromUI()
        {
            AudioSource.PlayClipAtPoint(_closeButtonClickedClip, Camera.main.transform.position);
            Close();
        }

        private void FillViews()
        {
            foreach (ToolConfig toolConfig in _configService.ToolsConfigs.Values)
            {
                if (!toolConfig.IsUpgradable)
                    continue;

                var upgradeItemView = Instantiate(_prefab, _content);
                upgradeItemView.Construct();

                string id = toolConfig.ID;
                Sprite sprite = toolConfig.Sprite;
                
                upgradeItemView.Init(sprite, id);
                
                upgradeItemView.UpgradeButtonClicked += OnUpgradeButtonClicked;

                _views.Add(id, upgradeItemView);
                
            }
        }

        private void OnUpgradeButtonClicked(string itemID)
        {
            _upgradeResourceCalback?.Invoke(itemID);
        }
    }
}