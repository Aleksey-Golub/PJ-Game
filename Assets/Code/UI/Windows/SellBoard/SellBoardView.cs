using Code.Services;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class SellBoardView : MonoBehaviour
    {
        [SerializeField] private SellItemView _prefab;
        [SerializeField] private Transform _content;
        [SerializeField] private TextMeshProUGUI _header;
        [SerializeField] private Button _closeButton;
        [SerializeField] private AudioClip _closeButtonClickedClip;

        private IConfigsService _resourceConfigService;
        private IAudioService _audio;
        
        private Dictionary<ResourceType, SellItemView> _views;
        private Action<ResourceType> _sellResourceCalback;

        internal void Coustruct(IConfigsService configService, IAudioService audio)
        {
            _views = new();
            _resourceConfigService = configService;
            _audio = audio;
            LService.LanguageChanged += RefreshUI;

            FillViews();

            _closeButton.onClick.AddListener(CloseFromUI);
        }

        private void OnDestroy()
        {
            _closeButton.onClick.RemoveListener(CloseFromUI);

            foreach (var view in _views.Values)
            {
                view.SellButtonClicked -= OnSellButtonClicked;
            }

            LService.LanguageChanged -= RefreshUI;
        }

        internal void Open(IReadOnlyDictionary<ResourceType, int> storage, Action<ResourceType> sellResourceCalback)
        {
            _sellResourceCalback = sellResourceCalback;
            gameObject.SetActive(true);
            
            Refresh(storage);
            RefreshUI();
        }

        internal void Refresh(IReadOnlyDictionary<ResourceType, int> storage)
        {
            foreach (var pair in storage)
            {
                ResourceType type = pair.Key;

                if (_views.TryGetValue(type, out SellItemView resView))
                {
                    int itemsCount = pair.Value;
                    resView.SetData(itemsCount, itemsCount * _resourceConfigService.ResourcesConfigs[type].Cost);
                }
            }
        }

        internal void Close()
        {
            _sellResourceCalback = null;
            gameObject.SetActive(false);
        }

        private void CloseFromUI()
        {
            _audio.PlaySfxAtUI(_closeButtonClickedClip);
            Close();
        }

        private void FillViews()
        {
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                if (type is ResourceType.None)
                    continue;

                var config = _resourceConfigService.GetConfigFor(type);
                if (!config.Sellable)
                    continue;
                
                var resView = Instantiate(_prefab, _content);
                resView.Construct(_audio);

                var sprite = config.Sprite;
                resView.Init(sprite, 0, 0, type);
                resView.SellButtonClicked += OnSellButtonClicked;

                _views.Add(type, resView);
            }
        }

        private void OnSellButtonClicked(ResourceType resourceType)
        {
            _sellResourceCalback?.Invoke(resourceType);
        }

        private void RefreshUI()
        {
            _header.text = LService.Localize("k_Sell_Items");

            foreach (SellItemView view in _views.Values)
            {
                view.SetTexts(LService.Localize("k_Sell"));
            }
        }
    }
}