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
        private Dictionary<ToolType, UpgradeItemView> _views;
        private Action<ToolType> _upgradeResourceCalback;

        internal Action<ToolType> UpgradeButtonClicked;

        internal void Coustruct(ConfigsService configService)
        {
            _views = new();
            _configService = configService;
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

        internal void Open(Action<ToolType> upgradeResourceCalback)
        {
            _upgradeResourceCalback = upgradeResourceCalback;
            gameObject.SetActive(true);

            //Refresh(storage);
        }

        //internal void Refresh(IReadOnlyDictionary<ResourceType, int> storage)
        //{
        //    foreach (var pair in storage)
        //    {
        //        ResourceType type = pair.Key;

        //        if (_views.TryGetValue(type, out SellItemView resView))
        //        {
        //            int itemsCount = pair.Value;
        //            resView.SetData(itemsCount, itemsCount * _configService.ResourcesConfigs[type].Cost);
        //        }
        //    }
        //}

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

                int nextLevel = 2;
                int nextLevelIndex = nextLevel - 1;
                ToolType type = toolConfig.Type;
                Sprite sprite = toolConfig.Sprite;
                string upgradeText = $"Drop {toolConfig.UpgradeStaticDatas[nextLevelIndex].Value * 100}%";
                string levelText = $"Lvl {nextLevelIndex}";
                string upgradeCostText = $"{toolConfig.UpgradeStaticDatas[nextLevelIndex].Cost}";

                upgradeItemView.Init(sprite, upgradeText, levelText, upgradeCostText, type);
                upgradeItemView.UpgradeButtonClicked += OnUpgradeButtonClicked;

                _views.Add(type, upgradeItemView);
            }
        }

        private void OnUpgradeButtonClicked(ToolType type)
        {
            _upgradeResourceCalback?.Invoke(type);
        }
    }
}